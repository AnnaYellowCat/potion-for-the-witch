using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    private ItemManager itemManager;

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
        public List<InventoryItemState> inventoryItems = new List<InventoryItemState>();
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

    [System.Serializable]
    public struct InventoryItemState
    {
        public string uniqueId;
        public int slotIndex;
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

        itemManager = FindFirstObjectByType<ItemManager>();
        if (itemManager == null)
        {
            GameObject itemManagerObj = new GameObject("ItemManager");
            itemManager = itemManagerObj.AddComponent<ItemManager>();
            DontDestroyOnLoad(itemManagerObj);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isRespawning)
        {
            StartCoroutine(RestoreSceneState(scene.name));
        }
    }

    private System.Collections.IEnumerator RestoreSceneState(string sceneName)
    {
        yield return new WaitForEndOfFrame();

        if (isRespawning)
        {
            RestorePlayerPosition();
        }

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

            if (itemManager != null)
            {
                itemManager.RestoreItemsStateFromGameState(this, sceneName);
                itemManager.ApplyItemsState();
            }

            Centipede[] enemies = FindObjectsByType<Centipede>(FindObjectsInactive.Include, FindObjectsSortMode.None);
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

        if (itemManager != null)
        {
            itemManager.SaveItemsStateToGameState(this, sceneName);
        }

        Centipede[] enemies = FindObjectsByType<Centipede>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.SaveState();

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

        ItemManager itemManager = FindFirstObjectByType<ItemManager>();
        if (itemManager != null)
        {
            itemManager.ResetForNewGame();
        }
    }
}