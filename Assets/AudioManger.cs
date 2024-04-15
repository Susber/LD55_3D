using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    public static AudioManger Instance;
    void Awake()
    {
        if (Instance == null)
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

    void PlaySoundGun()
    {
        PlaySound(0, 0.1f);
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
