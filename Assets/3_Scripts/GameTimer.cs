using GoodHub.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : Singleton<GameTimer>
{

    private const int TIME_ALLOCATION = 12;
    private float timeRemaining;

    [SerializeField] private GameObject addTimeEffectPrefab;

    private void Start()
    {
        GameUIManager.Instance.SetTime(TIME_ALLOCATION);
        timeRemaining = TIME_ALLOCATION;
    }

    public void StartClock()
    {
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);

            timeRemaining -= 1f;


            if (Mathf.RoundToInt(timeRemaining) % 1 == 0 && GameManager.Instance.gamemode == Gamemode.SANDBOX)
            {
                AddTimeToClock(120);
            }

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
