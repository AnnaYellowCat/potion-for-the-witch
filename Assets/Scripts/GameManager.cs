using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        if (sceneName == "Level1")
        {
            Destroy(GameObject.Find("Music Menu"));
        }
        else
        {
            Destroy(GameObject.Find("Music Game"));
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
