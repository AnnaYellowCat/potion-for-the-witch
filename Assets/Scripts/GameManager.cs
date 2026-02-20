using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
        if (sceneNumber == 5)
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
