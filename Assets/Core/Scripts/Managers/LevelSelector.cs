using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

    public enum eGameMode
    {
        COOP,
        VERSUS,
        SOLO,
        NONE,
    }

    eGameMode currentGameMode;

    WorldProperties[] worldsProperties;
    WorldProperties miniGameWorld;
    List<LevelProperties> miniGames = new List<LevelProperties>();
    //public UnlockOrder WorldUnlockOrder;
    [Tooltip("Number of levels before chaging difficulty. Ex : 3, the difficulty will change on the 3rd level")]
    public int NbStepsChangeDifficulty = 3;
    [Tooltip("Last level before Score recap")]
    public int EndStep = 1;

    public string LastEasyWorldName = "";
    public string[] WorldsToFinish;
    public List<WorldProperties> ExcludedWorlds = new List<WorldProperties>();

    bool isRandomMode = false;
    int currentStep = 0; // Number of levels played in current world
    bool hasPlayedMiniGame = false;
    bool isTokenAcquired = false;

    LevelProperties previousLevel = null;
    RunProperties previousRun = null;
    LevelProperties currentLevel = null;
    WorldProperties currentworld = null;
    RunProperties currentRun = null;

    int currentLevelIndex = -1;
    int currentDifficulty = 0;
    List<int> availableLevelsIndexes = new List<int>();
    Queue<LevelProperties> levelsToLoad = new Queue<LevelProperties>();

    void Start()
    {
        if (worldsProperties == null)
        {
            worldsProperties = Resources.LoadAll<WorldProperties>("Worlds/");
        }
    }

    public void SetGameMode(eGameMode mode)
    {
        currentGameMode = mode;
    }

    public eGameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }

    public WorldProperties[] GetWorldProperties()
    {
        return worldsProperties;
    }

    public WorldProperties GetCurrentWorld()
    {
        return currentworld;
    }

    public RunProperties GetCurrentRun()
    {
        return currentRun;
    }

    public int GetCurrentDifficulty()
    {
        return currentDifficulty;
    }

    public int GetCurrentStep()
    {
        return currentStep;
    }

    public bool GetIsTokenAcquired()
    {
        return isTokenAcquired;
    }

    public void SetIsTokenAcquired()
    {
        isTokenAcquired = true;
    }

    public bool GetHasPlayedMiniGame()
    {
        return hasPlayedMiniGame;
    }

    public LevelProperties GetCurrentLevel()
    {
        if (currentLevel == null)
        {
            currentLevel = GetLevelPropertiesFromName(SceneManager.GetActiveScene().name);
        }
        return currentLevel;
    }

    public LevelProperties GetLevelPropertiesFromName(string levelName)
    {
        if (worldsProperties == null)
            return null;

        for (int i = 0; i < worldsProperties.Length; ++i)
        {
            for (int j = 0; j < worldsProperties[i].runs.Length; ++j)
            {
                for (int k = 0; k < worldsProperties[i].runs[j].properties.Count; ++k)
                {
                    if (worldsProperties[i].runs[j].properties[k].LevelName == levelName)
                    {
                        return worldsProperties[i].runs[j].properties[k];
                    }
                }
            }
        }
        return null;
    }

    public WorldProperties GetWorldPropertiesFromLevelName(string levelName)
    {
        for (int i = 0; i < worldsProperties.Length; ++i)
        {
            for (int j = 0; j < worldsProperties[i].runs.Length; ++j)
            {
                for (int k = 0; k < worldsProperties[i].runs[j].properties.Count; ++k)
                {
                    if (worldsProperties[i].runs[j].properties[k].LevelName == levelName)
                    {
                        return worldsProperties[i];
                    }
                }
            }
        }
        return null;
    }

    public WorldProperties GetWorldPropertiesFromName(string worldName)
    {
        for (int i = 0; i < worldsProperties.Length; ++i)
        {
            if (worldsProperties[i].WorldName == worldName)
            {
                return worldsProperties[i];
            }
        }
        return null;
    }

    public WorldProperties GetWorldPropertiesFromRun(RunProperties run)
    {
        for (int i = 0; i < worldsProperties.Length; ++i)
        {
            for (int j = 0; j < worldsProperties[i].runs.Length; ++j)
            {
                if (worldsProperties[i].runs[j] == run)
                {
                    return worldsProperties[i];
                }
            }
        }
        return null;
    }

    //To use only for tests, if level is in multiple runs, the run found may not be compatible with current number of players
    public RunProperties GetRunPropertiesFromLevel(LevelProperties level)
    {
        for (int i = 0; i < worldsProperties.Length; ++i)
        {
            for (int j = 0; j < worldsProperties[i].runs.Length; ++j)
            {
                for (int k = 0; k < worldsProperties[i].runs[j].properties.Count; k++)
                {
                    if (worldsProperties[i].runs[j].properties[k] == level)
                    {
                        return worldsProperties[i].runs[j];
                    }
                }
            }
        }
        return null;
    }

    #region Random Mode

    List<int> GetAvailableLevelIndexesFromStep(int step)
    {
        List<LevelProperties.eLevelType> levelTypesAvailable = GetLevelTypesForNextLevel(step);
        List<int> bestIndexesForNextLevel;
        int easierDifficulty = currentDifficulty;

        //Boss and Special levels don't require specific difficulty so we use an aproximation
        if (levelTypesAvailable.Contains(LevelProperties.eLevelType.BOSS) || levelTypesAvailable.Contains(LevelProperties.eLevelType.MINIGAME) || levelTypesAvailable.Contains(LevelProperties.eLevelType.SCROLLING))
        {
            bestIndexesForNextLevel = GetAvailableLevelIndexesFromDifficulty(levelTypesAvailable, currentDifficulty);
            while (bestIndexesForNextLevel.Count == 0 && easierDifficulty >= 0)
            {
                easierDifficulty--;
                bestIndexesForNextLevel = GetAvailableLevelIndexesFromDifficultyStrict(levelTypesAvailable, easierDifficulty);
            }

            if (bestIndexesForNextLevel.Count != 0)
            {
                return bestIndexesForNextLevel;
            }
            else //ERROR there is no level of this type, fallback 
            {
                Debug.LogError("No level type " + levelTypesAvailable[0].ToString() + " in world " + currentworld.WorldName);
                levelTypesAvailable.Clear();
                levelTypesAvailable.Add(LevelProperties.eLevelType.NORMAL);
                easierDifficulty = currentDifficulty;
            }
        }

        //Regular levels
        bestIndexesForNextLevel = GetAvailableLevelIndexesFromDifficultyStrict(levelTypesAvailable, currentDifficulty);

        while (bestIndexesForNextLevel.Count == 0)
        {
            easierDifficulty--;
            bestIndexesForNextLevel = GetAvailableLevelIndexesFromDifficultyStrict(levelTypesAvailable, easierDifficulty);
        }
        return bestIndexesForNextLevel;
    }

    List<int> GetAvailableLevelIndexesFromDifficulty(List<LevelProperties.eLevelType> levelTypesAvailable, int difficulty)
    {
        List<int> bestIndexesForNextLevel = new List<int>();
        bool hasAssistedControls = GameManager.Instance.HasAtLeastOneAssistedControl();

        for (int i = 0; i < availableLevelsIndexes.Count; ++i)
        {
            if (currentRun.properties[availableLevelsIndexes[i]].Difficulty <= difficulty && levelTypesAvailable.Contains(currentRun.properties[availableLevelsIndexes[i]].LevelType))
            {
                bestIndexesForNextLevel.Add(availableLevelsIndexes[i]);
            }
        }

        return bestIndexesForNextLevel;
    }

    List<int> GetAvailableLevelIndexesFromDifficultyStrict(List<LevelProperties.eLevelType> levelTypesAvailable, int difficulty)
    {
        List<int> bestIndexesForNextLevel = new List<int>();
        bool hasAssistedControls = GameManager.Instance.HasAtLeastOneAssistedControl();

        for (int i = 0; i < availableLevelsIndexes.Count; ++i)
        {
            if (currentRun.properties[availableLevelsIndexes[i]].Difficulty == difficulty && levelTypesAvailable.Contains(currentRun.properties[availableLevelsIndexes[i]].LevelType))
            {
                bestIndexesForNextLevel.Add(availableLevelsIndexes[i]);
            }
        }

        return bestIndexesForNextLevel;
    }

    List<LevelProperties.eLevelType> GetLevelTypesForNextLevel(int step)
    {
        return new List<LevelProperties.eLevelType>() { LevelProperties.eLevelType.NORMAL };
    }

    public void SelectRandomNextLevel(string levelName = "", bool manageStates = true)
    {
        isRandomMode = true;

        //Players have reached the end of the current world
        if (manageStates && levelsToLoad.Count == 0 && currentStep == EndStep)
        {
            currentStep = 0;
            isTokenAcquired = false;
            hasPlayedMiniGame = false;
            //UnlockNextWorlds();
            //LoadWorldDifficultyScene();
            LoadMainScene();
            return;
        }

        //** MOSTLY FOR TESTING

        //There is no more level to load
        if (levelsToLoad.Count > 0)
        {
            LoadSpecificLevel(levelsToLoad.Dequeue());
            return;
        }

        //Current case when testing new levels
        if (currentRun == null)
        {
            SelectNextWorld();
            return;
        }

        if (levelName.Equals(""))
        {
            List<int> bestIndexesForNextLevel = GetAvailableLevelIndexesFromStep(currentStep);
            if (bestIndexesForNextLevel.Count == 0)
            {
                currentStep = 0;
                isTokenAcquired = false;
                hasPlayedMiniGame = false;
                LoadMainScene();
                return;
            }
            currentLevelIndex = bestIndexesForNextLevel[Random.Range(0, bestIndexesForNextLevel.Count)];
        }
        else
        {
            for (int i = 0; i < currentRun.properties.Count; ++i)
            {
                if (currentRun.properties[i].LevelName == levelName)
                {
                    currentLevelIndex = i;
                    break;
                }
            }
        }

        if (currentRun.properties[currentLevelIndex].LevelType != LevelProperties.eLevelType.OTHER)
        {
            currentStep++; // Add step if not Tutorial or Intermission
            isTokenAcquired = false;
        }

        availableLevelsIndexes.Remove(currentLevelIndex);


        LevelProperties levelToLoadAfter = currentRun.properties[currentLevelIndex]; // currentLevelIndex is modified after that so store it there

        //Add outro
        if (currentRun.properties[currentLevelIndex].LevelToLoadAfter != "")
        {
            levelsToLoad.Enqueue(GetLevelPropertiesFromName(levelToLoadAfter.LevelToLoadAfter));
        }

        if (currentRun.properties[currentLevelIndex].LevelToLoadBefore != "" && currentRun.properties[currentLevelIndex].LevelToLoadBefore != SceneManager.GetActiveScene().name)
        {
            SelectRandomNextLevel(currentRun.properties[currentLevelIndex].LevelToLoadBefore, false);
            levelsToLoad.Enqueue(levelToLoadAfter);
        }
        else
        {
            SceneManager.LoadScene(currentRun.properties[currentLevelIndex].LevelName, LoadSceneMode.Single);
            /*if (!currentRun.properties[currentLevelIndex].IsUsingInteractiveMusic)
            {
                GameManager.Instance.GetMusicManager().PlayMusic(currentRun.properties[currentLevelIndex].Music);
            }
            else
            {
                //GameManager.Instance.GetMusicManager().PlayWorldMusic(currentworld.WorldName, currentStep - 1);
                GameManager.Instance.GetMusicManager().SetIsDynamic(true);
            }*/
        }
    }

    public void SelectNextWorld(string nextWorldName = "", bool useRandomMode = true)
    {
        isRandomMode = useRandomMode;
        if (nextWorldName == "")
        {
            List<WorldProperties> worldsAvailable = new List<WorldProperties>();
            worldsAvailable.AddRange(worldsProperties);
            currentworld = worldsAvailable[Random.Range(0, worldsAvailable.Count)];
        }
        else
        {
            for (int i = 0; i < worldsProperties.Length; ++i)
            {
                if (worldsProperties[i].WorldName == nextWorldName)
                {
                    currentworld = worldsProperties[i];
                    break;
                }
            }
        }

        currentLevelIndex = -1;
        if (isRandomMode)
        {
            InitAvailableLevelsFromWorld(currentworld);
            SelectRandomNextLevel();
        }
        else
        {
            SelectNextLevel();
        }
    }

    #endregion

    #region Load Specific Scenes

    public void LoadMainScene()
    {
        GameManager.Instance.GetAmbianceManager().StopCurrentSnapshot();
        GameManager.Instance.GetAmbianceManager().StopCurrentAmbiance();
        currentStep = 0;
        isTokenAcquired = false;
        hasPlayedMiniGame = false;
        levelsToLoad.Clear();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        List<Player> toRemove = new List<Player>();
        foreach (Player player in GameManager.Instance.GetPlayers())
        {
            //In case the player controller is still vibrating while loading main menu
            if (player.GetCurrentCharacter() != null)
            {
                player.GetCurrentCharacter().StopVibration();
            }
            //If player disconnected during run we delete it
            if (player.GetControls().GetControlsMode() == PlayerControls.eControlsMode.CONTROLLER && player.GetControls().GetNbJoysticks() == 0)
            {
                toRemove.Add(player);
            }
        }
        foreach (Player player in toRemove)
        {
            //Sets map Menu to enabled to allow reconnect later
            player.GetControls().SetMapEnabled(true, "Menu");
            GameManager.Instance.RemovePlayer(player, true);
        }

        GameManager.Instance.GetMusicManager().PlayMusic(GameManager.Instance.GetMusicManager().DefaultMusic);
        GameManager.Instance.GetAmbianceManager().StartSnapshot("Menu");
    }

    public void LoadSelectionMenuScene()
    {
        GameManager.Instance.GetAmbianceManager().StopCurrentSnapshot();
        GameManager.Instance.GetAmbianceManager().StopCurrentAmbiance();
        currentStep = 0;
        isTokenAcquired = false;
        hasPlayedMiniGame = false;
        levelsToLoad.Clear();
        SceneManager.LoadScene("SelectionMenu", LoadSceneMode.Single);
        GameManager.Instance.GetMusicManager().PlayMusic(GameManager.Instance.GetMusicManager().DefaultMusic);
        GameManager.Instance.GetAmbianceManager().StartSnapshot("Menu");
    }

    public void LoadSelectionMenuSoloScene()
    {
        GameManager.Instance.GetAmbianceManager().StopCurrentSnapshot();
        GameManager.Instance.GetAmbianceManager().StopCurrentAmbiance();
        currentStep = 0;
        isTokenAcquired = false;
        hasPlayedMiniGame = false;
        levelsToLoad.Clear();
        SceneManager.LoadScene("SelectionMenuSolo", LoadSceneMode.Single);
        GameManager.Instance.GetMusicManager().PlayMusic(GameManager.Instance.GetMusicManager().DefaultMusic);
        GameManager.Instance.GetAmbianceManager().StartSnapshot("Menu");
    }

    public void LoadSpecificLevel(string levelName)
    {
        currentworld = GetWorldPropertiesFromLevelName(levelName);
        SetRun(levelName);

        if (currentworld != null)
        {
            //InitAvailableLevelsFromWorld(currentworld);
            if (isRandomMode)
            {
                SelectRandomNextLevel(levelName);
            }
            else
            {
                SelectNextLevel(levelName);
            }
        }
        else
        {
            SceneManager.LoadScene(levelName, LoadSceneMode.Single); // Load level that is not inside a world
            currentStep++;
            isTokenAcquired = false;
        }
    }

    public void LoadPreviousLevel()
    {
        if (previousLevel == null)
            return;

        currentLevel = previousLevel;
        currentRun = previousRun;
        SceneManager.LoadScene(currentLevel.LevelName, LoadSceneMode.Single);
        levelsToLoad.Clear();
    }

    public void LoadPreviousRun() // Use when restarting from a minigame
    {
        if (previousRun == null)
            return;

        currentRun = previousRun;
        levelsToLoad.Clear();

        RestartRun();
    }

    void SetRun(string levelName, bool checkNbPlayers = true)
    {
        if (currentworld == null)
            return;

        if (currentGameMode == eGameMode.COOP)
        {
            foreach (var run in currentworld.runs)
            {
                if ((!checkNbPlayers || run.NbPlayerMin <= GameManager.Instance.GetPlayers().Count && run.NbPlayerMax >= GameManager.Instance.GetPlayers().Count)
                                                                             && run.properties.Find(k => k.LevelName == levelName) != null)
                {
                    currentRun = run;
                    break;
                }
            }
        }
        else
        {
            int smallestRun = 999;
            foreach (var run in currentworld.runs)
            {
                if (run.NbPlayerMax < smallestRun)
                {
                    smallestRun = run.NbPlayerMax;
                }
            }

            foreach (var run in currentworld.runs)
            {
                if (run.NbPlayerMax == smallestRun && run.properties.Find(k => k.LevelName == levelName) != null)
                {
                    currentRun = run;
                    break;
                }
            }
        }
    }

    public void LoadSpecificLevel(LevelProperties levelProperties)
    {
        if (levelProperties.LevelToLoadBefore != "" && levelProperties.LevelToLoadBefore != SceneManager.GetActiveScene().name)
        {
            if (isRandomMode)
            {
                SelectRandomNextLevel(levelProperties.LevelToLoadBefore, false);
            }
            else
            {
                SelectNextLevel(levelProperties.LevelToLoadBefore);
            }
            levelsToLoad.Enqueue(levelProperties);
        }
        else
        {
            currentworld = GetWorldPropertiesFromLevelName(levelProperties.LevelName);
            currentLevel = levelProperties;
            SetRun(currentLevel.LevelName, false);
            SceneManager.LoadScene(levelProperties.LevelName, LoadSceneMode.Single);
        }
    }

    #endregion

    public void SetNextWorld(string worldName)
    {
        for (int i = 0; i < worldsProperties.Length; ++i)
        {
            if (worldsProperties[i].WorldName == worldName)
            {
                currentworld = worldsProperties[i];
                break;
            }
        }
    }

    public void StartRun()
    {
        RunProperties run = null;

        //Get right run depending on current number of players
        if (currentGameMode == eGameMode.COOP)
        {
            for (int i = 0; i < currentworld.runs.Length; ++i)
            {
                var currentWorldRun = currentworld.runs[i];
                if (currentWorldRun.NbPlayerMin <= GameManager.Instance.GetPlayers().Count && currentWorldRun.NbPlayerMax >= GameManager.Instance.GetPlayers().Count)
                {
                    run = currentWorldRun;
                    break;
                }
            }
        }
        else
        {
            int lessPlayersRequired = 99;

            if (currentworld != null)
            {
                for (int i = 0; i < currentworld.runs.Length; ++i)
                {
                    var currentWorldRun = currentworld.runs[i];
                    if (currentWorldRun.NbPlayerMax < lessPlayersRequired)
                    {
                        run = currentWorldRun;
                        lessPlayersRequired = currentWorldRun.NbPlayerMax;
                    }
                }
            }
        }

        if (run != null)
        {
            StartRun(run);
        }
    }

    public void StartRun(string worldName)
    {
        currentworld = GetWorldPropertiesFromName(worldName);
        StartRun();
    }

    public void StartRun(WorldProperties world)
    {
        currentworld = world;
        StartRun();
    }

    public void StartRun(RunProperties run)
    {
        currentStep = 0;
        isTokenAcquired = false;
        GameManager.Instance.GetMetricsManager().ResetPlayerMetrics();
        SetNextWorld(GetWorldPropertiesFromRun(run).WorldName);
        currentRun = run;

        GameManager.Instance.GetMetricsManager().StartRunMetric();
        SelectNextLevel();
    }

    public void RestartRun()
    {
        hasPlayedMiniGame = false;
        StartRun(currentRun);
    }

    public int GetLevelIndexInRun(RunProperties run, LevelProperties level)
    {
        for (int i = 0; i < run.properties.Count; ++i)
        {
            if (run.properties[i] == level)
            {
                return i;
            }
        }
        return -1;
    }

    public void SelectNextLevel(string levelToLoad = "")
    {
        isRandomMode = false;

        //There is no more level to load
        if (levelsToLoad.Count > 0)
        {
            LoadSpecificLevel(levelsToLoad.Dequeue());
            return;
        }

        if (levelToLoad.Equals(""))
        {
            if (currentStep >= currentRun.properties.Count)
            {
                currentStep = 0;
                isTokenAcquired = false;
                hasPlayedMiniGame = false;
                //UnlockNextWorlds();
                LoadMainScene();
                return;
            }
            currentLevel = currentRun.properties[currentStep];
        }
        else
        {
            for (int i = 0; i < currentRun.properties.Count; ++i)
            {
                if (currentRun.properties[i].LevelName == levelToLoad)
                {
                    currentLevel = currentRun.properties[i];
                    break;
                }
            }
        }

        if (currentLevel.LevelType != LevelProperties.eLevelType.OTHER && currentLevel.LevelType != LevelProperties.eLevelType.MINIGAME)
        {
            currentStep++; // Add step if not Tutorial or Intermission or Minigame
            isTokenAcquired = false;
        }

        LevelProperties levelToLoadAfter = currentLevel;

        if (!string.IsNullOrEmpty(currentLevel.LevelToLoadBefore) && currentLevel.LevelToLoadBefore != SceneManager.GetActiveScene().name)
        {
            SelectNextLevel(currentLevel.LevelToLoadBefore);
            levelsToLoad.Enqueue(levelToLoadAfter);
            if (!string.IsNullOrEmpty(levelToLoadAfter.LevelToLoadAfter))
            {
                levelsToLoad.Enqueue(GetLevelPropertiesFromName(levelToLoadAfter.LevelToLoadAfter));
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(levelToLoadAfter.LevelToLoadAfter))
            {
                levelsToLoad.Enqueue(GetLevelPropertiesFromName(levelToLoadAfter.LevelToLoadAfter));
            }

            //load scene
            SceneManager.LoadScene(currentLevel.LevelName, LoadSceneMode.Single);
        }
    }

    void InitAvailableLevelsFromWorld(WorldProperties world)
    {
        availableLevelsIndexes.Clear();
        if (world.WorldName.Equals(""))
            return;

        currentworld = world;
        for (int i = 0; i < currentworld.runs.Length; ++i)
        {
            for (int j = 0; j < currentRun.properties.Count; ++j)
            {
                availableLevelsIndexes.Add(j);
            }
        }
    }

    public void SetDifficulty(int difficulty)
    {
        currentDifficulty = difficulty;
    }

    /*void AdjustDifficulty(int step)
    {
        if (step % NbStepsChangeDifficulty == 0) // every three levels
            currentDifficulty++;
    }*/

    /*void SetStartingDifficulty() // Gets minimum difficulty in the current world
    {
        int difficulty = int.MaxValue;

        for (int i = 0; i < currentworld.properties.Count; ++i)
        {
            if (currentworld.properties[i].Difficulty < difficulty)
            {
                difficulty = currentworld.properties[i].Difficulty;
            }
        }
        currentDifficulty = difficulty;
    }*/

    /*void UnlockNextWorlds() // Unlock new worlds at the end of a session
    {
        NextWorldUnlock currentUnlock = WorldUnlockOrder.UnlockableWorlds.Find(i => i.World.WorldName == currentworld.WorldName && i.World.Difficulty == currentDifficulty);
        foreach (UnlockableWorld world in currentUnlock.UnlockedWorlds)
        {
            GameManager.Instance.GetSaveManager().UnlockWorld(world);
        }
        GameManager.Instance.GetSaveManager().Save();
    }*/

    public bool GetIsRandomMode()
    {
        return isRandomMode;
    }

    public WorldProperties GetNextWorld()
    {
        WorldProperties nextWorld = null;
        for (int i = 0; i < worldsProperties.Length; ++i)
        {
            if (worldsProperties[i] != currentworld && ExcludedWorlds.Find(j => j.WorldName == worldsProperties[i].WorldName) == null
                && worldsProperties[i].OrderInMenu > currentworld.OrderInMenu && (nextWorld == null || worldsProperties[i].OrderInMenu < nextWorld.OrderInMenu))
            {
                nextWorld = worldsProperties[i];
            }
        }
        return nextWorld;
    }
}
