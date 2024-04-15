using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeInTime = 0.1f;

    private float targetVolume = 0.8f;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        StartCoroutine(FadeIn(audioSource, fadeInTime));
    }

    System.Collections.IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float startVolume = 0.01f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
