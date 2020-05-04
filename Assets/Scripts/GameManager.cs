using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoodHub.Core;

public class GameManager : Singleton<GameManager> {

    [Header("Rank Settings")]
    [SerializeField] private int[] rankTimeLimits = new int[3];
    [SerializeField] private int[] rankVowelMinimums = new int[3];
    public int CurrentRank { get; private set; }

    [Header("Debug")]
    public bool clearPrefs;

    //Game state
    private bool gameRunning;
    private float timeRemaining;
    IEnumerator countdownCoroutine;

    #region Inherited Methods

    void Start() {
        if (clearPrefs) {
            PlayerPrefs.DeleteAll();
        }

        CurrentRank = PlayerPrefs.GetInt("previousRank", 0);
    }

    #endregion

    #region Public Methods

    public void IncrementRank() {
        CurrentRank = (CurrentRank + 1) % 3;
        PlayerPrefs.SetInt("previousRank", CurrentRank);
    }

    public int GetVowelMinimum() {
        return rankVowelMinimums[CurrentRank];
    }

    public void EndGame() {
        StartCoroutine(EndGameCoroutine());
    }

    public void ReturnToMenu() {
        StartCoroutine(ReturnToMenuCoroutine());
    }

    #endregion

    #region Coroutines

    public IEnumerator StartGameCoroutine() {
        CurtainController.Instance.LowerCurtain();
        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        while (asyncLoad.isDone == false) {
            yield return null;
        }

        TileController.Instance.RegenerateTileBoard();
        ScoreManager.Instance.ResetCurrentGameScore();

        CurtainController.Instance.RaiseCurtain();
        UIManager.Instance.SetTime(rankTimeLimits[CurrentRank]);

        yield return new WaitForSeconds(1f);

        timeRemaining = rankTimeLimits[CurrentRank];
        countdownCoroutine = CountdownCoroutine();
        StartCoroutine(countdownCoroutine);
    }

    IEnumerator CountdownCoroutine() {
        gameRunning = true;
        while (gameRunning) {
            yield return new WaitForSeconds(1f);

            timeRemaining -= 1f;
            if (timeRemaining <= 0) {
                StartCoroutine(EndGameCoroutine());
            } else {
                UIManager.Instance.SetTime(Mathf.RoundToInt(timeRemaining));
            }
        }
    }

    IEnumerator EndGameCoroutine() {
        StopCoroutine(countdownCoroutine);
        CurtainController.Instance.LowerCurtain();

        yield return new WaitForSeconds(1f);

        if (ScoreManager.Instance.IsScoreOnLeaderboard(CurrentRank, ScoreManager.Instance.GameScore)) {
            UIManager.Instance.DisplayHighscoreDialog();
        } else {
            UIManager.Instance.DisplayGameoverDialog();
        }
    }

    IEnumerator ReturnToMenuCoroutine() {
        UIManager.Instance.HideEndGameScreens();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        while (asyncLoad.isDone == false) {
            yield return null;
        }

        CurtainController.Instance.RaiseCurtain();

    }

    #endregion

}
