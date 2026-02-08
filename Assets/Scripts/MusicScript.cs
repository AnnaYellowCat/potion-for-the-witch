using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    private static MusicController inctance;
    
    private void Awake()
    {
        if (inctance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            inctance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}

