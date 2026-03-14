using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    private GameManager Manag;
    private GameStateManager gameState;
    [SerializeField] public GameObject interactPrompt;
    [SerializeField] public string sceneName;
    [SerializeField] public string spawnPointTag = "Respawn";
    private bool isPlayerInZone = false;

    void Start()
    {
        Manag = FindFirstObjectByType<GameManager>();
        gameState = FindFirstObjectByType<GameStateManager>();

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            TransitionToNextLevel();
        }
    }

    private void TransitionToNextLevel()
    {
        Hero hero = FindFirstObjectByType<Hero>();
        if (hero != null && gameState != null)
        {
            gameState.SaveGameState(hero, SceneManager.GetActiveScene().name);

            gameState.SaveCurrentSceneState();

            gameState.nextSpawnPointTag = spawnPointTag;
            gameState.isRespawning = true;
        }

        ItemManager itemManager = FindFirstObjectByType<ItemManager>();

        Manag.LoadScene(sceneName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}