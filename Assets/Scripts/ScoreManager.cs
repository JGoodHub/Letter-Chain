using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodHub.Core;

public class ScoreManager : Singleton<ScoreManager> {

    public struct ScoreEntry {
        public string nickname;
        public int score;

        public ScoreEntry(string _nickname, int _score) {
            nickname = _nickname;
            score = _score;
        }
    }

    public int GameScore { get; private set; }
    private ScoreEntry[,] scores;

    #region Inherited Methods

    public void Start() {
        LoadScoresFromPrefs();
    }

    #endregion

    #region Public Methods

    public void ResetCurrentGameScore() {
        GameScore = 0;
    }

    public void IncreaseScore(int amount) {
        GameScore += amount;
        UIManager.Instance.SetScore(GameScore);
    }

    public void DecreaseScore(int amount) {
        GameScore = Mathf.Clamp(GameScore - amount, 0, int.MaxValue);
        UIManager.Instance.SetScore(GameScore);
    }

    public ScoreEntry GetHighestScoreForRank(int rank) {
        SortScores(rank);
        return scores[rank, 0];
    }

    public ScoreEntry GetLowestScoreForRank(int rank) {
        SortScores(rank);
        return scores[rank, scores.GetLength(1) - 1];
    }

    public ScoreEntry[] GetAllScoresForRank(int rank) {
        if (scores == null) {
            LoadScoresFromPrefs();
        }

        ScoreEntry[] scoresForRank = new ScoreEntry[scores.GetLength(1)];

        for (int position = 0; position < scoresForRank.Length; position++) {
            scoresForRank[position] = scores[rank, position];
        }

        return scoresForRank;
    }

    public bool IsScoreOnLeaderboard(int rank, int score) {
        if (score > GetLowestScoreForRank(rank).score) {
            return true;
        } else {
            return false;
        }
    }

    public void SaveScoreToPrefs(int rank, string nickname, int score) {
        if (rank < scores.GetLength(0) && IsScoreOnLeaderboard(rank, score)) {
            //Update the local variable for the leaderboard
            int newScorePosition = 0;
            for (int position = 0; position < scores.GetLength(1); position++) {
                if (score > scores[rank, position].score) {
                    newScorePosition = position;
                    break;
                }
            }

            for (int position = scores.GetLength(1) - 1; position > newScorePosition; position--) {
                scores[rank, position] = scores[rank, position - 1];
            }

            //Trim the name to fit in the leaderboard
            nickname = nickname.Length > 10 ? nickname.Substring(0, 10) : nickname;

            scores[rank, newScorePosition].nickname = nickname;
            scores[rank, newScorePosition].score = score;

            SortScores(rank);

            //Update the player pref copy of the leaderboard
            string leaderboardString = "";
            for (int position = 0; position < scores.GetLength(1); position++) {
                leaderboardString += scores[rank, position].nickname + ",";
            }

            for (int position = 0; position < scores.GetLength(1); position++) {
                leaderboardString += scores[rank, position].score + ",";
            }

            leaderboardString.TrimEnd(',');
            PlayerPrefs.SetString("leaderboard" + rank, leaderboardString);
        }
    }

    #endregion

    #region Private Methods

    private void LoadScoresFromPrefs() {
        scores = new ScoreEntry[3, 6];
        for (int rank = 0; rank < 3; rank++) {
            string leaderboardString = PlayerPrefs.GetString("leaderboard" + rank, "Unclaimed,Unclaimed,Unclaimed,Unclaimed,Unclaimed,Unclaimed,0,0,0,0,0,0");
            //leaderboardString = "Mark,Karen,Molly,Dave,Jess,Finn,146,98,78,44,43,20";
            string[] leaderboardStringSplit = leaderboardString.Split(',');

            for (int position = 0; position < scores.GetLength(1); position++) {
                try {
                    ScoreEntry entry = new ScoreEntry(leaderboardStringSplit[position], int.Parse(leaderboardStringSplit[position + 6]));
                    scores[rank, position] = entry;
                } catch (System.Exception e) { }
            }

            SortScores(rank);
        }
    }

    private void SortScores(int rank) {
        for (int i = 0; i < scores.GetLength(1); i++) {
            for (int j = 0; j < scores.GetLength(1) - 1; j++) {
                if (scores[rank, j].score < scores[rank, j + 1].score) {
                    ScoreEntry temp = scores[rank, j];
                    scores[rank, j] = scores[rank, j + 1];
                    scores[rank, j + 1] = temp;
                }
            }
        }
    }

    #endregion

}

