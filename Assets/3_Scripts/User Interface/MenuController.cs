using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoodHub.Core;

public class MenuController : Singleton<MenuController> {

    [Header("Menu Animator")]
    public Animator slidingMenuAnimator;

    [Header("Rank Elements")]
    public GameObject rankSelector;
    public Text rankText;
    private int[] selectorPositions = new int[] { 140, 0, -140 };

    [Header("Leaderboard Elements")]
    public Text nicknamesText;
    public Text scoresText;
    public Button[] viewRankButtons = new Button[3];

    #region Inherited Methods

    private void Start() {
        DisplayScoreForRank(GameManager.Instance.CurrentRank);
        SetRankUI(GameManager.Instance.CurrentRank);
    }

    #endregion

    #region Public Methods

    public void OpenTargetMenu(int targetMenu) {
        slidingMenuAnimator.SetInteger("targetMenu", targetMenu);
    }

    public void IncrementRankUI() {
        GameManager.Instance.IncrementRank();
        SetRankUI(GameManager.Instance.CurrentRank);
    }

    private void SetRankUI(int rank) {
        RectTransform selectorTransform = (RectTransform)rankSelector.transform;
        selectorTransform.anchoredPosition = new Vector2(selectorTransform.anchoredPosition.x, selectorPositions[rank]);

        switch (rank) {
            case 0:
                rankText.text = "EASY";
                break;
            case 1:
                rankText.text = "MEDIUM";
                break;
            case 2:
                rankText.text = "HARD";
                break;
        }
    }

    public void DisplayScoreForRank(int rank) {
        ScoreManager.ScoreEntry[] scoresForRank = ScoreManager.Instance.GetAllScoresForRank(rank);
        string nicknameOutput = "";
        string scoresOutput = "";

        for (int i = 0; i < scoresForRank.Length; i++) {
            nicknameOutput += scoresForRank[i].nickname + "\n";
            scoresOutput += scoresForRank[i].score + "\n";
        }

        nicknamesText.text = nicknameOutput;
        scoresText.text = scoresOutput;

        foreach (Button levelButton in viewRankButtons) {
            levelButton.interactable = true;
        }

        viewRankButtons[rank].interactable = false;
    }

    public void StartGame() {
        GameManager.Instance.StartCoroutine(GameManager.Instance.StartGameCoroutine());
    }

    #endregion

}

