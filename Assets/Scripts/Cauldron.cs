using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cauldron : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private GameObject hintPanel;

    private GameStateManager gameState;

    private void Start()
    {
        Transform hintTransform = transform.Find("Hint");
        if (hintTransform != null)
        {
            hintPanel = hintTransform.gameObject;
            hintPanel.SetActive(false);
        }

        gameState = FindFirstObjectByType<GameStateManager>();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (gameState != null)
            {
                Hero hero = FindFirstObjectByType<Hero>();
                if (hero != null)
                {
                    gameState.SaveGameState(hero, SceneManager.GetActiveScene().name);
                }
            }

            Hero.Instance.isOver = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
        }
    }
}