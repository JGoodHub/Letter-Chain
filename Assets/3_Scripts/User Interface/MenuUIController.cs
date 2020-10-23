using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoodHub.Core;

public class MenuUIController : Singleton<MenuUIController>
{

    [Header("Menu Animator")]
    public Animator slidingMenuAnimator;

    [Header("Gamemode Elements")]
    public Text newGameGamemodeText;
    public Text selectedGamemodeText;
    public Button[] selectGamemodeButtons = new Button[3];

    [Header("Leaderboard Elements")]
    public Text nicknamesText;
    public Text scoresText;
    public Button[] viewGamemodeButtons = new Button[3];

    #region Inherited Methods

    private void Start()
    {
        OnDisplayScoreForGamemode(GameManager.Instance.gamemode);
        SetGamemodeUI(GameManager.Instance.gamemode);
    }

    #endregion

    #region Public Methods

    public void OpenTargetMenu(int targetMenu)
    {
        slidingMenuAnimator.SetInteger("targetMenu", targetMenu);
    }

    public void OnSetGamemode(int gamemodeIndex)
    {
        GameManager.Instance.SetGamemode(gamemodeIndex);
        SetGamemodeUI(GameManager.Instance.gamemode);
    }

    private void SetGamemodeUI(Gamemode mode)
    {
        int modeIndex = (int)mode;

        selectGamemodeButtons[0].interactable = modeIndex != 0;
        selectGamemodeButtons[1].interactable = modeIndex != 1;
        selectGamemodeButtons[2].interactable = modeIndex != 2;

        switch (mode)
        {
            case Gamemode.TIME_TRIAL:
                newGameGamemodeText.text = "Time Trial";
                selectedGamemodeText.text = "Time Trial";
                break;
            case Gamemode.SURVIVAL:
                newGameGamemodeText.text = "Survival";
                selectedGamemodeText.text = "Survival";
                break;
            case Gamemode.SANDBOX:
                newGameGamemodeText.text = "Sandbox";
                selectedGamemodeText.text = "Sandbox";
                break;
        }
    }

    public void OnDisplayScoreForGamemode(int modeIndex)
    {
        OnDisplayScoreForGamemode((Gamemode)modeIndex);
    }

    public void OnDisplayScoreForGamemode(Gamemode mode)
    {
        int modeIndex = (int)mode;

        LeaderboardManager.ScoreEntry[] modeScores = LeaderboardManager.Instance.GetAllScores(mode);
        string nicknameOutput = "";
        string scoresOutput = "";

        for (int i = 0; i < modeScores.Length; i++)
        {
            nicknameOutput += modeScores[i].nickname + "\n";
            scoresOutput += modeScores[i].score + "\n";
        }

        nicknamesText.text = nicknameOutput;
        scoresText.text = scoresOutput;

        viewGamemodeButtons[0].interactable = modeIndex != 0;
        viewGamemodeButtons[1].interactable = modeIndex != 1;
        viewGamemodeButtons[2].interactable = modeIndex != 2;
    }

    public void OnStartGame()
    {
        GameManager.Instance.StartGame();
    }

    #endregion

}

