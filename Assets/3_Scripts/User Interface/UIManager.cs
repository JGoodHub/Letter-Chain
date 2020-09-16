using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoodHub.Core;

public class UIManager : Singleton<UIManager> {

    //-----VARIABLES-----

    [Header("In Game UI")]

    [SerializeField] private Text liveScoreText;
    [SerializeField] private Text countdownText;

    [SerializeField] private Text lastWordText;
    [SerializeField] private Text lastWordDefinitionText;

    [Header("Gameover Elements")]

    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private Text gameoverScreenScoreText;

    [Header("New Highscore Elements")]

    [SerializeField] private GameObject highscoreScreen;
    [SerializeField] private Text highscoreScoreText;
    [SerializeField] private InputField highscoreInputNameField;

    #region Inherited Methods

    private void Start() {
        SetScore(0);
        DisplayWord("");
        DisplayDefinition("");

        HideEndGameScreens();
    }

    #endregion

    #region Public Methods

    public void SetScore(int value) {
        liveScoreText.text = value + "";
    }

    public void SetTime(int secondsRemaining) {
        string minutes = "" + Mathf.FloorToInt(secondsRemaining / 60);
        string seconds = "" + secondsRemaining % 60;

        if (seconds.Length < 2) {
            seconds = "0" + seconds;
        }

        countdownText.text = minutes + ":" + seconds;
    }

    public void DisplayWord(string word) {
        lastWordText.text = word.ToUpper();
    }

    public void DisplayDefinition(string definition) {
        lastWordDefinitionText.text = '"' + definition.Trim('"') + '"';
    }

    public void DisplayGameoverDialog() {
        gameoverScreen.SetActive(true);
        gameoverScreenScoreText.text = liveScoreText.text;
    }

    public void DisplayHighscoreDialog() {
        highscoreScreen.SetActive(true);
        highscoreScoreText.text = liveScoreText.text;
    }

    public void HideEndGameScreens() {
        gameoverScreen.SetActive(false);
        highscoreScreen.SetActive(false);
    }

    #endregion

    #region Private Methods

    #endregion

    #region Event Handlers

    public void EndGameEarly() {
        GameManager.Instance.EndGame();
    }

    public void ReturnToMenu() {
        GameManager.Instance.ReturnToMenu();
    }

    public void SubmitNewHighscore() {
        ScoreManager.Instance.SaveScoreToPrefs(GameManager.Instance.CurrentRank, highscoreInputNameField.text, ScoreManager.Instance.GameScore);

        GameManager.Instance.ReturnToMenu();
    }

    #endregion

}

