using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoodHub.Core;


public enum Gamemode
{
    TIME_TRIAL,
    SURVIVAL,
    SANDBOX
}

public class Statics
{
    public static string GAMEMODE = "gamemode";
}

public class GameManager : Singleton<GameManager>
{

    public Gamemode gamemode;

    #region Inherited Methods

    void Start()
    {
        gamemode = (Gamemode)PlayerPrefs.GetInt(Statics.GAMEMODE, 0);
    }

    #endregion

    #region Public Methods


    public void SetGamemode(int gamemodeIndex)
    {
        gamemode = (Gamemode)gamemodeIndex;
        PlayerPrefs.SetInt(Statics.GAMEMODE, gamemodeIndex);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    public void ReturnToMenu()
    {
        GameUIManager.Instance.HideEndGameScreens();

        SceneManager.LoadScene(0);

        CurtainController.Instance.RaiseCurtain();
    }

    #endregion

    #region Coroutines

    public IEnumerator StartGameCoroutine()
    {
        CurtainController.Instance.LowerCurtain();

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(1);

        yield return null;

        ScoreManager.Instance.ResetCurrentGameScore();

        yield return new WaitForSeconds(1f);

        GameTimer.Instance.StartClock();
    }

    #endregion

}
