using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodHub.Core;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{

    public delegate void OnPlayerScored(int scoreAmount);
    public delegate void OnPlayerFailed();
    public delegate void OnPlayerAttempted();

    public event OnPlayerScored PlayerScored;
    public event OnPlayerFailed PlayerFailed;
    public event OnPlayerAttempted PlayerAttempted;

    public int GameScore { get; private set; }

    #region Inherited Methods

    private void Start()
    {
        ResetCurrentGameScore();
    }

    #endregion

    #region Public Methods

    public void ResetCurrentGameScore()
    {
        GameScore = 0;
    }

    public void IncreaseGameScore(int amount)
    {
        GameScore += amount;
        GameUIManager.Instance.SetScore(GameScore);
        GameUIManager.Instance.DisplayWordScore(amount);

        if (GameManager.Instance.gamemode == Gamemode.SURVIVAL)
        {
            GameTimeManager.Instance.AddTimeToClock(amount);
        }
    }

    public void DecreaseGameScore(int amount)
    {
        GameScore = Mathf.Clamp(GameScore - amount, 0, int.MaxValue);
        GameUIManager.Instance.SetScore(GameScore);
        GameUIManager.Instance.DisplayWordScore(amount * -1);

        if (GameManager.Instance.gamemode == Gamemode.SURVIVAL)
        {
            GameTimeManager.Instance.AddTimeToClock(amount * -1);
        }
    }

    public void ResetGameScore()
    {
        GameScore = 0;
    }

    #endregion
}

