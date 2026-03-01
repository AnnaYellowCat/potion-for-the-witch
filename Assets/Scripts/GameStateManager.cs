using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;

    public Vector2 playerPosition;
    public double playerHealth;
    public int collectedItems;
    public int correctItems;
    public bool isRespawning = false;
    public string lastSceneName;
    public string nextSpawnPointTag = "";

    [System.Serializable]
    public class SceneState
    {
        public Dictionary<string, bool> chests = new Dictionary<string, bool>();
        public Dictionary<string, EnemyState> enemies = new Dictionary<string, EnemyState>();
        public Dictionary<string, bool> items = new Dictionary<string, bool>();
    }

    [System.Serializable]
    public struct EnemyState
    {
        public int health;
        public Vector3 position;
        public Vector3 direction;
        public bool flipX;
        public bool isDead;
    }

    public Dictionary<string, SceneState> scenesState = new Dictionary<string, SceneState>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1" && !isRespawning)
        {
            StartNewGame();
        }

        if (isRespawning)
        {
            StartCoroutine(RestoreSceneState(scene.name));
        }
    }

    private System.Collections.IEnumerator RestoreSceneState(string sceneName)
    {
        yield return new WaitForEndOfFrame();

        RestorePlayerPosition();

        if (scenesState.ContainsKey(sceneName))
        {
            SceneState sceneState = scenesState[sceneName];

            Unboxing[] chests = FindObjectsByType<Unboxing>(FindObjectsSortMode.None);
            foreach (var chest in chests)
            {
                string objectName = chest.gameObject.name;
                if (sceneState.chests.ContainsKey(objectName) && sceneState.chests[objectName])
                {
                    chest.SetOpened();
                }
            }

            GameObject[] sceneItems = GameObject.FindGameObjectsWithTag("Item");
            GameObject[] chestItems = GameObject.FindGameObjectsWithTag("ChestItem");

            List<GameObject> allItems = new List<GameObject>();
            allItems.AddRange(sceneItems);
            allItems.AddRange(chestItems);


            foreach (var item in allItems)
            {
                string itemName = item.name;

                Unboxing parentChest = item.GetComponentInParent<Unboxing>();
                if (parentChest != null)
                {
                    itemName = parentChest.gameObject.name + "_" + item.name;
                }

                if (sceneState.items.ContainsKey(itemName) && !sceneState.items[itemName])
                {
                    item.SetActive(false);
                }
                else if (!sceneState.items.ContainsKey(itemName))
                {

                    item.SetActive(false);
                }
            }

            Centipede[] enemies = FindObjectsByType<Centipede>(FindObjectsSortMode.None);

            foreach (var enemy in enemies)
            {
                string objectName = enemy.gameObject.name;

                if (sceneState.enemies.ContainsKey(objectName))
                {
                    EnemyState state = sceneState.enemies[objectName];

                    if (state.isDead)
                    {
                        enemy.gameObject.SetActive(false);
                        enemy.isDeath = true;
                    }
                    else
                    {
                        enemy.gameObject.SetActive(true);
                        enemy.transform.position = state.position;
                        enemy.SetLives(state.health);
                        enemy.RestoreMovement(state.direction, state.flipX);
                        enemy.isDeath = false;
                    }
                }
                else
                {
                    enemy.gameObject.SetActive(false);
                    enemy.isDeath = true;
                }
            }
        }
        else
        {

            Centipede[] enemies = FindObjectsByType<Centipede>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(true);
                enemy.SetLives(3);
                enemy.isDeath = false;
            }

            GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");
            foreach (var item in allItems)
            {
                item.SetActive(true);
            }
        }

        isRespawning = false;
        nextSpawnPointTag = "";

    }

    public void SaveCurrentSceneState()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;


        if (!scenesState.ContainsKey(sceneName))
        {
            scenesState[sceneName] = new SceneState();
        }

        SceneState sceneState = scenesState[sceneName];
        sceneState.chests.Clear();
        sceneState.enemies.Clear();
        sceneState.items.Clear();

        Unboxing[] chests = FindObjectsByType<Unboxing>(FindObjectsSortMode.None);

        foreach (var chest in chests)
        {
            string objectName = chest.gameObject.name;
            sceneState.chests[objectName] = chest.IsOpened;
        }

        GameObject[] sceneItems = GameObject.FindGameObjectsWithTag("Item");
        GameObject[] chestItems = GameObject.FindGameObjectsWithTag("ChestItem");


        foreach (var item in sceneItems)
        {
            string itemName = item.name;
            bool exists = item.activeInHierarchy;
            sceneState.items[itemName] = exists;
        }

        foreach (var item in chestItems)
        {
            Unboxing parentChest = item.GetComponentInParent<Unboxing>();
            string itemName = parentChest != null ? parentChest.gameObject.name + "_" + item.name : item.name;
            bool exists = item.activeInHierarchy;
            sceneState.items[itemName] = exists;
        }

        Centipede[] enemies = FindObjectsByType<Centipede>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                enemy.SaveState();
            }

            string objectName = enemy.gameObject.name;
            EnemyState state = new EnemyState
            {
                health = enemy.currentLives,
                position = enemy.transform.position,
                direction = enemy.GetDirection(),
                flipX = enemy.GetFlipX(),
                isDead = enemy.isDeath
            };
            sceneState.enemies[objectName] = state;

        }

    }

    private void RestorePlayerPosition()
    {
        Hero hero = FindFirstObjectByType<Hero>();
        if (hero != null)
        {
            if (!string.IsNullOrEmpty(nextSpawnPointTag))
            {
                GameObject spawnPoint = GameObject.FindGameObjectWithTag(nextSpawnPointTag);
                if (spawnPoint != null)
                {
                    hero.transform.position = spawnPoint.transform.position;
                }
                else
                {
                    hero.transform.position = playerPosition;
                }
            }
            else
            {
                hero.transform.position = playerPosition;
            }

            hero.SetHealth(playerHealth);
            hero.amount = collectedItems;
            hero.amountRight = correctItems;

        }
    }

    public void SaveGameState(Hero hero, string currentScene)
    {
        if (hero != null)
        {
            playerPosition = hero.transform.position;
            playerHealth = hero.GetCurrentHealth();
            collectedItems = hero.amount;
            correctItems = hero.amountRight;
            lastSceneName = currentScene;

            SaveCurrentSceneState();
        }
    }

    public void StartNewGame()
    {
        isRespawning = false;
        scenesState.Clear();
        playerHealth = 5;
        collectedItems = 0;
        correctItems = 0;
        nextSpawnPointTag = "";
    }
}