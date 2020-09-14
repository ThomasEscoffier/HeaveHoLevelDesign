using System.Collections.Generic;

public class MetricsManager
{
    public class RunMetrics
    {
        public int NbTokenObtained = 0;
        public int NbTokenObtainedMinigame = 0;
        public List<float> LevelTimes = new List<float>();
    }

    public class PlayerMetrics
    {
        public Player CorrespondingPlayer;
        public int NbDeath = 0;

        public PlayerMetrics(Player player)
        {
            CorrespondingPlayer = player;
        }
    }

    RunMetrics currentWorldMetrics = null;
    List<PlayerMetrics> playersMetrics = new List<PlayerMetrics>();

    public RunMetrics GetCurrentRunMetrics()
    {
        return currentWorldMetrics;
    }

    public List<PlayerMetrics> GetAllCurrentPlayersMetrics()
    {
        return playersMetrics;
    }

    public void StartRunMetric()
    {
        currentWorldMetrics = new RunMetrics();
    }

    public void AddToken(int nbToken)
    {
        if (currentWorldMetrics == null)
        {
            StartRunMetric();
        }
        currentWorldMetrics.NbTokenObtained += nbToken;
    }

    public void AddTokenMinigame(int nbToken)
    {
        if (currentWorldMetrics == null)
        {
            StartRunMetric();
        }
        currentWorldMetrics.NbTokenObtainedMinigame += nbToken;
    }

    public void AddLevelTime(float levelTime)
    {
        if (currentWorldMetrics == null)
        {
            StartRunMetric();
        }
        currentWorldMetrics.LevelTimes.Add(levelTime);
    }

    public PlayerMetrics GetMetricsFromPlayer(Player player)
    {
        return playersMetrics.Find(i => i.CorrespondingPlayer == player);
    }

    PlayerMetrics AddNewPlayer(Player player)
    {
        PlayerMetrics playerMetrics = new PlayerMetrics(player);
        playersMetrics.Add(playerMetrics);
        return playerMetrics;
    }

    public void AddDeathToPlayerMetrics(Player player)
    {
        PlayerMetrics playerInfo = playersMetrics.Find(i => i.CorrespondingPlayer == player);
        if (playerInfo == null)
        {
            playerInfo = AddNewPlayer(player);
        }
        playerInfo.NbDeath++;
    }

    public PlayerMetrics GetPlayerWithMostDeaths()
    {
        PlayerMetrics metrics = null;
        int mostDeaths = 0;
        foreach (PlayerMetrics playerMetrics in playersMetrics)
        {
            if (playerMetrics.NbDeath > mostDeaths)
            {
                metrics = playerMetrics;
            }
        }
        return metrics;
    }

    public void ResetPlayerMetrics()
    {
        playersMetrics.Clear();
    }
}
