using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void RestartGame()
        {
            GameStateManager gsm = FindFirstObjectByType<GameStateManager>();
            if (gsm != null)
            {
                gsm.StartNewGame();
            }

            SceneManager.LoadScene("Level1");
        }

    public void ExitGame()
    {
        Application.Quit();
    }
}
