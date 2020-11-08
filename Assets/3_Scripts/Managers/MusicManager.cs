using GoodHub.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [Serializable]
    public class MusicTrack
    {
        public string name = "Unamed Track";
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;

        public static MusicTrack Find(MusicTrack[] tracks, string name)
        {
            for (int i = 0; i < tracks.Length; i++)
            {
                if (tracks[i].name == name)
                    return tracks[i];
            }

            return null;
        }
    }

    public MusicTrack[] tracks;
    private MusicTrack activeTrack;

    private AudioSource audioSource;

    public string startingTrack;
    public float startFadeInTime = 3f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        activeTrack = MusicTrack.Find(tracks, startingTrack);

        audioSource.clip = activeTrack.clip;
        audioSource.volume = activeTrack.volume;
        audioSource.Play();

        audioSource.volume = 0;
        StartCoroutine(FadeInCoroutine(startFadeInTime));
    }

    public void SetTrack(string name, float fadeDuration)
    {
        MusicTrack track = MusicTrack.Find(tracks, name);

        if (track.clip == null)
            return;

        StartCoroutine(ChangeTrackCoroutine(track, fadeDuration));
    }

    private IEnumerator ChangeTrackCoroutine(MusicTrack newTrack, float fadeDuration)
    {
        if (fadeDuration <= 0)
        {
            activeTrack = newTrack;
            audioSource.clip = newTrack.clip;
            audioSource.Play();

            yield break;
        }
        else
        {
            StartCoroutine(FadeOutCoroutine(fadeDuration));

            yield return new WaitForSeconds(fadeDuration);

            activeTrack = newTrack;
            audioSource.clip = newTrack.clip;
            audioSource.Play();

            StartCoroutine(FadeInCoroutine(fadeDuration));
        }
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float volumeChangePerSecond = activeTrack.volume / Mathf.Clamp(duration, 0.1f, float.MaxValue);

        while (audioSource.volume < activeTrack.volume)
        {
            audioSource.volume += volumeChangePerSecond * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float volumeChangePerSecond = activeTrack.volume / Mathf.Clamp(duration, 0.1f, float.MaxValue);

        while (audioSource.volume > 0)
        {
            audioSource.volume -= volumeChangePerSecond * Time.deltaTime;
            yield return null;
        }
    }

}
