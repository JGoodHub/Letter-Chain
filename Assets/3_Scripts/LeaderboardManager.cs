using GoodHub.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : Singleton<LeaderboardManager>
{

    public struct ScoreEntry
    {
        public string nickname;
        public int score;

        public ScoreEntry(string _nickname, int _score)
        {
            nickname = _nickname;
            score = _score;
        }
    }

    private const string TIME_TRIAL_LEADERBOARD_KEY = "timeTrialLeaderboard";
    private const string SURVIVAL_LEADERBOARD_KEY = "survivalLeaderboard";
    private const string SANDBOX_LEADERBOARD_KEY = "sandboxLeaderboard";
    private const int LEADERBOARD_LEN = 6;

    private Dictionary<Gamemode, List<ScoreEntry>> leaderboard = new Dictionary<Gamemode, List<ScoreEntry>>();

    #region Inherited Methods

    public void Start()
    {
        LoadLeaderboardsFromPrefs();
    }

    #endregion

    #region Public Methods

    public ScoreEntry GetHighscoreForMode(Gamemode gamemode)
    {
        return leaderboard[gamemode][0];
    }

    public ScoreEntry GetLowscoreForMode(Gamemode gamemode)
    {
        return leaderboard[gamemode][LEADERBOARD_LEN - 1];
    }

    public ScoreEntry[] GetAllScores(Gamemode gamemode)
    {
        return leaderboard[gamemode].ToArray();
    }

    public bool IsScoreOnLeaderboard(Gamemode gamemode, int score)
    {
        if (score > GetLowscoreForMode(gamemode).score)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddScoreToLeaderboard(Gamemode gamemode, string nickname, int score)
    {
        if (IsScoreOnLeaderboard(gamemode, score) == true)
        {
            for (int i = 0; i < LEADERBOARD_LEN; i++)
            {
                if (score > leaderboard[gamemode][i].score)
                {
                    leaderboard[gamemode].Insert(i, new ScoreEntry(nickname.Length > 10 ? nickname.Substring(0, 10) : nickname, score));
                    break;
                }
            }

            leaderboard[gamemode].RemoveAt(LEADERBOARD_LEN);
        }
    }

    public void SaveLeaderboardToPrefs()
    {
        string timeLeaderboardString = "";
        string survivalLeaderboardString = "";
        string sandboxLeaderboardString = "";

        for (int i = 0; i < LEADERBOARD_LEN; i++)
        {
            timeLeaderboardString += leaderboard[Gamemode.TIME_TRIAL][i].nickname + "," + leaderboard[Gamemode.TIME_TRIAL][i].score + ",";
            survivalLeaderboardString += leaderboard[Gamemode.SURVIVAL][i].nickname + "," + leaderboard[Gamemode.SURVIVAL][i].score + ",";
            sandboxLeaderboardString += leaderboard[Gamemode.SANDBOX][i].nickname + "," + leaderboard[Gamemode.SANDBOX][i].score + ",";
        }

        PlayerPrefs.SetString(TIME_TRIAL_LEADERBOARD_KEY, timeLeaderboardString.Trim(','));
        PlayerPrefs.SetString(SURVIVAL_LEADERBOARD_KEY, survivalLeaderboardString.Trim(','));
        PlayerPrefs.SetString(SANDBOX_LEADERBOARD_KEY, sandboxLeaderboardString.Trim(','));
    }

    #endregion

    #region Private Methods

    private void LoadLeaderboardsFromPrefs()
    {
        // Clear any exisiting scores
        leaderboard.Clear();

        // Initialise the new highscores
        leaderboard.Add(Gamemode.TIME_TRIAL, new List<ScoreEntry>(LEADERBOARD_LEN));
        leaderboard.Add(Gamemode.SURVIVAL, new List<ScoreEntry>(LEADERBOARD_LEN));
        leaderboard.Add(Gamemode.SANDBOX, new List<ScoreEntry>(LEADERBOARD_LEN));

        // Load the highscores as strings from the prefences or uncliamed slots if not
        string timeTrialLeaderboardString = PlayerPrefs.GetString(TIME_TRIAL_LEADERBOARD_KEY, "Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0");
        string survivalLeaderboardString = PlayerPrefs.GetString(SURVIVAL_LEADERBOARD_KEY, "Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0");
        string sandboxLeaderboardString = PlayerPrefs.GetString(SANDBOX_LEADERBOARD_KEY, "Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0,Unclaimed,0");

        // Split the highscore strings into tokens
        string[] timeTrialLeaderboardTokens = timeTrialLeaderboardString.Split(',');
        string[] survivalLeaderboardTokens = survivalLeaderboardString.Split(',');
        string[] sandboxLeaderboardTokens = sandboxLeaderboardString.Split(',');

        // Create the score entrys from the tokens
        for (int i = 0; i < LEADERBOARD_LEN * 2; i += 2)
        {
            leaderboard[Gamemode.TIME_TRIAL].Add(new ScoreEntry(timeTrialLeaderboardTokens[i], int.Parse(timeTrialLeaderboardTokens[i + 1])));
            leaderboard[Gamemode.SURVIVAL].Add(new ScoreEntry(survivalLeaderboardTokens[i], int.Parse(survivalLeaderboardTokens[i + 1])));
            leaderboard[Gamemode.SANDBOX].Add(new ScoreEntry(sandboxLeaderboardTokens[i], int.Parse(sandboxLeaderboardTokens[i + 1])));
        }

    }

    #endregion

}
