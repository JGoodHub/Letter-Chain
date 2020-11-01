using GoodHub.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : Singleton<GameTimeManager>
{

    private const int TIME_ALLOCATION = 60;
    private float timeRemaining;

    [SerializeField] private GameObject addTimeEffectPrefab;

    private AudioSource audioSource;
    public AudioClip clockTick;
    public AudioClip alarmBell;

    private bool paused = false;

    private void Start()
    {
        if (GameManager.Instance.gamemode == Gamemode.SANDBOX)
            timeRemaining = 599;
        else
            timeRemaining = TIME_ALLOCATION;

        GameUIManager.Instance.SetTime(Mathf.RoundToInt(timeRemaining));

        audioSource = GetComponent<AudioSource>();
    }

    public void StartClock()
    {
        GameUIManager.Instance.SetTime(Mathf.RoundToInt(timeRemaining));

        if (GameManager.Instance.gamemode != Gamemode.SANDBOX)
            StartCoroutine(StartClockCoroutine());
    }

    IEnumerator StartClockCoroutine()
    {
        while (timeRemaining > 0)
        {
            float timeLastFrame = timeRemaining;

            yield return null;

            timeRemaining -= Time.deltaTime;

            if (Mathf.Ceil(timeRemaining) == Mathf.Floor(timeLastFrame) && timeRemaining <= 9)
                audioSource.PlayOneShot(clockTick, 1f - (timeRemaining / 9f) + 0.2f);

            if (timeRemaining <= 0)
            {
                audioSource.PlayOneShot(alarmBell, 1f);

                GameUIManager.Instance.SetTime(0);
                StopClock();
            }
            else
            {
                GameUIManager.Instance.SetTime(Mathf.CeilToInt(timeRemaining));
            }

            while (paused)
                yield return null;
        }
    }

    public void AddTimeToClock(int amount)
    {
        timeRemaining += amount;
        GameUIManager.Instance.SetTime(Mathf.RoundToInt(timeRemaining));

        GameObject addTimeEffectObject = Instantiate(addTimeEffectPrefab);
        AddTimeEffect addTImeEffect = addTimeEffectObject.GetComponent<AddTimeEffect>();

        addTImeEffect.Initalise(amount);
    }

    public void StopClock()
    {
        StartCoroutine(StopClockCoroutine());
    }

    IEnumerator StopClockCoroutine()
    {
        CurtainController.Instance.LowerCurtain();

        yield return new WaitForSeconds(1f);

        if (LeaderboardManager.Instance.IsScoreOnLeaderboard(GameManager.Instance.gamemode, ScoreManager.Instance.GameScore))
        {
            GameUIManager.Instance.DisplayHighscoreDialog();

            yield return new WaitForSeconds(0.5f);

            ScoreManager.Instance.FireConfetti();

        }
        else
        {
            GameUIManager.Instance.DisplayGameoverDialog();
        }
    }

    public void PauseGame()
    {
        paused = true;
    }

    public void ResumeGame()
    {
        paused = false;
    }
}
