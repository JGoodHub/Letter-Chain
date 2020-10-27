using GoodHub.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [Serializable]
    public struct MusicTrack
    {
        public string name;
        public AudioClip clip;

        public static MusicTrack Find(MusicTrack[] entries, string name)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].name == name)
                    return entries[i];
            }

            return default;
        }
    }

    public MusicTrack[] tracks;

    private AudioSource audioSource;

    public string startingTrack;
    public bool fadeInOnStart;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = MusicTrack.Find(tracks, startingTrack).clip;
        audioSource.Play();

        if (fadeInOnStart)
        {
            audioSource.volume = 0;
            StartCoroutine(FadeInCoroutine(3f));
        }
    }

    public void SetTrack(string name, float fadeDuration)
    {
        MusicTrack track = MusicTrack.Find(tracks, name);

        if (track.clip == null)
            return;

        StartCoroutine(ChangeTrackCoroutine(track.clip, fadeDuration));
    }

    private IEnumerator ChangeTrackCoroutine(AudioClip newTrack, float fadeDuration)
    {
        if (fadeDuration <= 0)
        {
            audioSource.clip = newTrack;
            audioSource.Play();
            yield break;
        }
        else
        {
            StartCoroutine(FadeOutCoroutine(fadeDuration));

            yield return new WaitForSeconds(fadeDuration);

            audioSource.clip = newTrack;
            audioSource.Play();

            StartCoroutine(FadeInCoroutine(fadeDuration));
        }
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float volumeChangePerSecond = 1f / Mathf.Clamp(duration, 0.1f, float.MaxValue);

        while (audioSource.volume < 1)
        {
            audioSource.volume += volumeChangePerSecond * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float volumeChangePerSecond = 1f / Mathf.Clamp(duration, 0.1f, float.MaxValue);

        while (audioSource.volume > 0)
        {
            audioSource.volume -= volumeChangePerSecond * Time.deltaTime;
            yield return null;
        }
    }

}
