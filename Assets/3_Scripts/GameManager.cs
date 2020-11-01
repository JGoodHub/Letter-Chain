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
    public static string CLEAR_PREFS = "clearPrefs_2";
}

public class GameManager : Singleton<GameManager>
{
    public Gamemode gamemode;

    #region Inherited Methods

    private void Awake()
    {
        base.Awake();

        if (PlayerPrefs.GetInt(Statics.CLEAR_PREFS, 0) == 0)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt(Statics.CLEAR_PREFS, 1);
        }

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
        MusicManager.Instance.SetTrack("Main Menu", 1f);

        GameUIManager.Instance.HideEndGameScreens();

        SceneManager.LoadScene(0);

        CurtainController.Instance.RaiseCurtain();

        BannerAdManager.Instance.SetBannerState(true, 1.5f);
    }

    #endregion

    #region Coroutines

    public IEnumerator StartGameCoroutine()
    {
        BannerAdManager.Instance.SetBannerState(false);

        MusicManager.Instance.SetTrack("Game", 1f);
        CurtainController.Instance.LowerCurtain();

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(1);

        yield return null;

        ScoreManager.Instance.ResetCurrentGameScore();

        yield return new WaitForSeconds(1f);

        GameTimeManager.Instance.StartClock();
    }

    #endregion

}
