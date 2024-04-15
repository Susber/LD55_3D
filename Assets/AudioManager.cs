using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySoundGun();
        }
    }

    public void PlaySoundGun()
    {
        PlaySound(0, 0.1f);
    }

    public void PlaySoundExplosion()
    {
        PlaySound(1, 0.3f);
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
