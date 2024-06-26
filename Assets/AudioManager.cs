using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Register the SceneLoaded callback
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Make sure to unregister the callback to prevent memory leaks
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This function is called every time a scene is loaded
        Debug.Log("Loaded scene: " + scene.name);

        // You can now call any function depending on the scene name
        if (scene.name == "Menu")
        {
            PlaySoundDestroyEnemy();
            FadeToMenuMusic();
        }
        if (scene.name == "SampleScene")
        {
            FadeToGameMusic();
        }
        if (scene.name == "WinMenu")
        {
            AudioManager.Instance.EndMusic();
            AudioManager.Instance.PlaySoundEnd();
        }
        if (scene.name == "LooseMenu")
        {
            AudioManager.Instance.EndMusic();
        }
    }

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    public AudioSource menuMusic;
    public AudioSource gameMusic;

    private float fadeTime = 1.0f; // Duration of the fade

    void Start()
    {
        // Set both music tracks to loop
        menuMusic.loop = true;
        gameMusic.loop = true;

        // Start with menu music muted and game music playing
        menuMusic.volume = 0;
        gameMusic.volume = 0;
        menuMusic.Play();
        gameMusic.Play();
    }


    // Call this to fade to menu music
    public void FadeToMenuMusic()
    {
        StartCoroutine(FadeAudio(gameMusic, false));
        StartCoroutine(FadeAudio(menuMusic, true));
    }

    // Call this to fade to game music
    public void FadeToGameMusic()
    {
        StartCoroutine(FadeAudio(menuMusic, false));
        StartCoroutine(FadeAudio(gameMusic, true));
    }

    public void EndMusic()
    {
        StartCoroutine(FadeAudio(menuMusic, false));
        StartCoroutine(FadeAudio(gameMusic, false));
    }

    IEnumerator FadeAudio(AudioSource audioSource, bool fadeIn)
    {
        float startVolume = fadeIn ? 0 : audioSource.volume;
        float endVolume = fadeIn ? 0.4f : 0;
        float currentTime = 0;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, currentTime / fadeTime);
            yield return null;
        }

        if (!fadeIn)
        {
            audioSource.volume = 0; // Ensure volume is set to 0 after fading out
        }
        else
        {
            audioSource.volume = 0.4f; // Ensure volume is set to max after fading in
        }
    }

    public void PlaySoundGun()
    {
        PlaySound(0, 0.4f);
    }

    public void PlaySoundExplosion()
    {
        PlaySound(1, 0.4f);
    }

    public void PlaySoundDestroyEnemy()
    {
        int randomNumber = UnityEngine.Random.Range(2, 6);
        PlaySound(randomNumber, 0.4f);
    }

    public void PlaySoundHit()
    {
        int randomNumber = UnityEngine.Random.Range(7, 10);
        PlaySound(randomNumber, 0.4f);
    }

    public void PlaySoundExp()
    {
        int randomNumber = UnityEngine.Random.Range(1, 12);
        if(randomNumber < 11)
            PlaySound(11, 0.1f);
        else if (randomNumber < 12)
            PlaySound(12, 0.1f);
        else
            PlaySound(13, 0.1f);
    }

    public void PlaySoundGotHit()
    {
        PlaySound(14, 0.3f);
    }

    public void PlaySoundDeath()
    {
        PlaySound(15, 0.3f);
    }
    public void PlaySoundEnd()
    {
        PlaySound(16, 0.8f);
    }

    public void PlaySoundPentagram()
    {
        PlaySound(17, 0.3f);
    }


    // Function to play a specific sound from the array
    public void PlaySound(int index, float volume)
    {
        // Check if the index is within the bounds of the array
        if (index < 0 || index >= audioClips.Length) return;

        // Create a temporary GameObject for playing the sound
        GameObject soundGameObject = new GameObject("TempAudio");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        // Assign the clip from the array based on the index
        audioSource.clip = audioClips[index];
        audioSource.volume = volume;
        audioSource.Play();

        // Destroy the GameObject after the clip finishes playing
        Destroy(soundGameObject, audioClips[index].length);
    }
}
