using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class AudioController
{
    public static IEnumerator FadeOutFadeIn(AudioSource audioSource,AudioClip newAudioClip, float FadeTime, float timeToPlay)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
        GameManager.singleton.StartCoroutine(FadeIn(audioSource,newAudioClip, FadeTime, timeToPlay));

    }
    public static IEnumerator FadeIn(AudioSource audioSource, AudioClip newAudioClip, float FadeTime, float time)
    {
        audioSource.clip = newAudioClip;
        audioSource.time = time;
        audioSource.Play();
        audioSource.volume = 0f;
        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
    }
}