using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoodHub.Core;

public class GameUIManager : Singleton<GameUIManager>
{

    //-----VARIABLES-----

    [Header("In Game UI")]

    [SerializeField] private Text liveScoreText;
    [SerializeField] private Text countdownText;

    [SerializeField] private Text lastWordText;
    [SerializeField] private Text lastWordDefinitionText;

    [SerializeField] private Text lastScoreText;

    [Header("Gameover Elements")]

    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private Text gameoverScreenScoreText;

    [Header("New Highscore Elements")]

    [SerializeField] private GameObject highscoreScreen;
    [SerializeField] private Text highscoreScoreText;
    [SerializeField] private InputField highscoreInputNameField;

    #region Inherited Methods

    private void Start()
    {
        SetScore(0);
        DisplayWord("");
        DisplayDefinition("");

        HideEndGameScreens();
    }

    #endregion

    #region Public Methods

    public void SetScore(int score)
    {
        liveScoreText.text = score.ToString();
    }

    public void DisplayWordScore(int wordScore)
    {
        lastScoreText.text = (wordScore >= 0 ? "+" : "") + wordScore.ToString();
    }

    public void SetTime(int secondsRemaining)
    {
        string minutes = "" + Mathf.FloorToInt(secondsRemaining / 60);
        string seconds = "" + secondsRemaining % 60;

        if (seconds.Length < 2)
        {
            seconds = "0" + seconds;
        }

        countdownText.text = minutes + ":" + seconds;
    }

    public void DisplayWord(string word)
    {
        lastWordText.text = word.ToUpper();
    }

    public void DisplayDefinition(string definition)
    {
        lastWordDefinitionText.text = '"' + definition.Trim('"') + '"';
    }

    public void DisplayGameoverDialog()
    {
        gameoverScreen.SetActive(true);
        gameoverScreenScoreText.text = liveScoreText.text;
    }

    public void DisplayHighscoreDialog()
    {
        highscoreScreen.SetActive(true);
        highscoreScoreText.text = liveScoreText.text;
    }

    public void HideEndGameScreens()
    {
        gameoverScreen.SetActive(false);
        highscoreScreen.SetActive(false);
    }

    #endregion

    #region Private Methods

    #endregion

    #region Event Handlers

    public void OnEndGameEarly()
    {
        GameTimer.Instance.StopClock();
    }

    public void OnReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }

    public void OnSubmitNewHighscore()
    {
        LeaderboardManager.Instance.AddScoreToLeaderboard(GameManager.Instance.gamemode, highscoreInputNameField.text, ScoreManager.Instance.GameScore);
        LeaderboardManager.Instance.SaveLeaderboardToPrefs();

        GameManager.Instance.ReturnToMenu();
    }

    #endregion

}

