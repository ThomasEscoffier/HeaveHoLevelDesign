using System.Collections.Generic;

public class ScoreManager {

    [System.Serializable]
    public class LevelScore
    {
        public string LevelName = "";
        public float Score = 0;
        public int NbPlayers = 0;
        public bool IsTimeScore = false;

        public LevelScore(string levelName, float score, int nbPlayers, bool isTimeScore)
        {
            LevelName = levelName;
            Score = score;
            NbPlayers = nbPlayers;
            IsTimeScore = isTimeScore;
        }
    }

    [System.Serializable]
    public class LevelsBestScores
    {
        public List<LevelScore> LevelScores = new List<LevelScore>();
    }

    public float PreviousStartTime = 0f;

    public bool GetAreAllRunsDone()
    {
        foreach (string worldName in GameManager.Instance.GetLevelSelector().WorldsToFinish)
        {
            LevelScore worldScoreCoop = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresCoop.LevelScores.Find(i => i.LevelName == worldName);
            if (worldScoreCoop == null)
            {
                LevelScore worldScoreSolo = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresSolo.LevelScores.Find(i => i.LevelName == worldName);
                if (worldScoreSolo == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void AddNewScore(string levelName, float score, int nbPlayers, bool isTimeScore, LevelSelector.eGameMode gameMode)
    {
        switch (gameMode)
        {
            case LevelSelector.eGameMode.COOP:
                {
                    LevelScore levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestScoresCoop.LevelScores.Find(i => i.LevelName == levelName);
                    if (levelScore != null)
                    {
                        levelScore.Score = score;
                    }
                    else
                    {
                        GameManager.Instance.GetSaveManager().GameSaveData.BestScoresCoop.LevelScores.Add(new LevelScore(levelName, score, nbPlayers, isTimeScore));
                    }
                    break;
                }
            case LevelSelector.eGameMode.SOLO:
                {
                    LevelScore levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestScoresSolo.LevelScores.Find(i => i.LevelName == levelName);
                    if (levelScore != null)
                    {
                        levelScore.Score = score;
                    }
                    else
                    {
                        GameManager.Instance.GetSaveManager().GameSaveData.BestScoresSolo.LevelScores.Add(new LevelScore(levelName, score, 1, isTimeScore));
                    }
                    break;
                }
            default:
                break;
        }

        GameManager.Instance.GetSaveManager().HasToSave = true;
    }

    public void AddNewRunScore(string worldName, float score, int nbPlayers, LevelSelector.eGameMode gameMode)
    {
        switch (gameMode)
        {
            case LevelSelector.eGameMode.COOP:
                {
                    LevelScore levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresCoop.LevelScores.Find(i => i.LevelName == worldName);
                    if (levelScore != null)
                    {
                        levelScore.Score = score;
                    }
                    else
                    {
                        GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresCoop.LevelScores.Add(new LevelScore(worldName, score, nbPlayers, true));
                    }
                    break;
                }
            case LevelSelector.eGameMode.SOLO:
                {
                    LevelScore levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresSolo.LevelScores.Find(i => i.LevelName == worldName);
                    if (levelScore != null)
                    {
                        levelScore.Score = score;
                    }
                    else
                    {
                        GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresSolo.LevelScores.Add(new LevelScore(worldName, score, nbPlayers, true));
                    }
                    break;
                }
            default:
                break;
        }
        GameManager.Instance.GetSaveManager().HasToSave = true;
    }

    public bool IsBetterScore(string levelName, float score, LevelSelector.eGameMode gameMode)
    {
        LevelScore levelScore = null;
        switch (gameMode)
        {
            case LevelSelector.eGameMode.COOP:
                {
                    levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestScoresCoop.LevelScores.Find(i => i.LevelName == levelName);
                    break;
                }
            case LevelSelector.eGameMode.SOLO:
                {
                    levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestScoresSolo.LevelScores.Find(i => i.LevelName == levelName);
                    break;
                }
        }

        return (levelScore == null || (levelScore.IsTimeScore && score < levelScore.Score || !levelScore.IsTimeScore && score > levelScore.Score));
    }

    public bool IsBetterRunScore(string worldName, float score, int nbPlayers, LevelSelector.eGameMode gameMode)
    {
        LevelScore levelScore = null;
        switch (gameMode)
        {
            case LevelSelector.eGameMode.COOP:
                {
                    levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresCoop.LevelScores.Find(i => i.LevelName == worldName && i.NbPlayers == nbPlayers);
                    break;
                }
            case LevelSelector.eGameMode.SOLO:
                {
                    levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresSolo.LevelScores.Find(i => i.LevelName == worldName);
                    break;
                }
        }

        return (levelScore == null || (levelScore.IsTimeScore && score < levelScore.Score || !levelScore.IsTimeScore && score > levelScore.Score));
    }

    public float GetScoreFromWorld(string levelName, LevelSelector.eGameMode gameMode)
    {
        LevelScore levelScore = null;
        switch (gameMode)
        {
            case LevelSelector.eGameMode.COOP:
                {
                    levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresCoop.LevelScores.Find(i => i.LevelName == levelName);
                    break;
                }
            case LevelSelector.eGameMode.SOLO:
                {
                    levelScore = GameManager.Instance.GetSaveManager().GameSaveData.BestRunScoresSolo.LevelScores.Find(i => i.LevelName == levelName);
                    break;
                }
        }
        return levelScore == null ? 0 : levelScore.Score;
    }
}
