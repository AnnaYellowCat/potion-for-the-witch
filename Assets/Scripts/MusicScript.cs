using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    private static MusicController instance;
    public AudioSource audioSource;

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip defeatMusic;
    [SerializeField] private AudioClip deathMusic;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1" || scene.name == "Level2")
        {
            ChangeMusic(gameMusic);
        }
        else if (scene.name == "Menu")
        {
            ChangeMusic(menuMusic);
        }
        else if (scene.name == "WinFull")
        {
            ChangeMusic(winMusic);
        }
        else if (scene.name == "Defeat")
        {
            ChangeMusic(defeatMusic);
        }
        else if (scene.name == "Death")
        {
            ChangeMusic(deathMusic);
        }
    }

    private void ChangeMusic(AudioClip newClip)
    {
        if (audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}