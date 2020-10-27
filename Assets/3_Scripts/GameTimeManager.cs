using GoodHub.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : Singleton<GameTimeManager>
{

    private const int TIME_ALLOCATION = 12;
    private float timeRemaining;

    [SerializeField] private GameObject addTimeEffectPrefab;

    private AudioSource audioSource;
    public AudioClip clockTick;

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
            yield return new WaitForSeconds(1f);

            timeRemaining -= 1f;

            if (timeRemaining <= 0)
            {
                StopClock();
            }
            else
            {
                GameUIManager.Instance.SetTime(Mathf.RoundToInt(timeRemaining));
            }
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

        Debug.Log(GameManager.Instance.gamemode);

        if (LeaderboardManager.Instance.IsScoreOnLeaderboard(GameManager.Instance.gamemode, ScoreManager.Instance.GameScore))
        {
            GameUIManager.Instance.DisplayHighscoreDialog();
        }
        else
        {
            GameUIManager.Instance.DisplayGameoverDialog();
        }
    }
}
