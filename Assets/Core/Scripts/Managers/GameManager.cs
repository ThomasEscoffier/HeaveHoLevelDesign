using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    public Player PlayerPrefab = null;
    public LevelSelector LevelSelectorPrefab = null;
    public DebugMode DebugModePrefab = null;
    public PaintManager PaintManagerPrefab = null;
    public MusicManager MusicManagerPrefab = null;
    public AmbianceManager AmbianceManagerPrefab = null;
    public ControllerTypesManager ControllerTypesManagerPrefab = null;
    public CharacterResources CharacterAssets = null;

    bool isDebugModeOn = false;
    ChainsManager chainsManager = null;
    LevelSelector levelSelectorManagerInstance = null;
    DebugMode debugModeInstance = null;
    LocalizationManager localization = null;
    PaintManager paintManager = null;
    MusicManager musicManager = null;
    AmbianceManager ambianceManager = null;
    BaseSaveManager saveManager = null;
    ScoreManager scoreManager = null;
    MetricsManager metricsManager = null;
    ControllerTypesManager controllerTypesManager = null;

    public NewCharacterPresets Presets = null;
    CharacterSelection selection = null;

    protected GameManager() { }

    public int NbMaxPlayers = 4;

    List<Player> players = new List<Player>();
    protected Rewired.Player SystemInput = null;

    const string selectionMenuName = "SelectionMenu";
    const string mainMenuName = "MainMenu";
    const string titleMenuName = "Title";
    bool isInSelectionMenu = false;
    bool isInMainMenu = false;
    bool isTitle = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        ListenSystemId();
        ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnected;
        ReInput.ControllerConnectedEvent += OnControllerConnected;

        chainsManager = new ChainsManager();
        metricsManager = new MetricsManager();
        localization = new LocalizationManager();
        localization.Init();
        levelSelectorManagerInstance = Instantiate(LevelSelectorPrefab, transform);
        musicManager = Instantiate(MusicManagerPrefab, transform);
        ambianceManager = Instantiate(AmbianceManagerPrefab, transform);
        controllerTypesManager = Instantiate(ControllerTypesManagerPrefab, transform);
        saveManager = new SaveManager();
        
        saveManager.Load();
        scoreManager = new ScoreManager();

        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        
        lastJoyStickCount = ReInput.controllers.joystickCount;
    }

    private int lastJoyStickCount;
    private bool waitingFrameForControllerConnectCheck;

    void ListenSystemId()
    {
        SystemInput = ReInput.players.SystemPlayer;
    }

    void OnControllerPreDisconnected(ControllerStatusChangedEventArgs args)
    {
        if (!isInMainMenu && !isTitle && !isInSelectionMenu)
            return;

        foreach (Rewired.Player inputPlayer in ReInput.players.Players)
        {
            if (GetPlayerFromId(inputPlayer.id) == null)
                continue;

            foreach (Joystick joystick in inputPlayer.controllers.Joysticks)
            {
                if (joystick.id == args.controllerId)
                {
                    if (isInSelectionMenu)
                    {
                        inputPlayer.controllers.maps.SetMapsEnabled(true, "Menu", "AssistedControlLeft");
                    }
                    RemoveInputPlayer(inputPlayer);
                    return;
                }
            }
        }
    }

    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        foreach (Rewired.Player inputPlayer in ReInput.players.Players)
        {
            if (GetPlayerFromId(inputPlayer.id) != null)
                continue;

            foreach (Joystick joystick in inputPlayer.controllers.Joysticks)
            {
                if (joystick.id == args.controllerId)
                {
                    if (isInSelectionMenu || isTitle || isInMainMenu)
                    {
                        inputPlayer.controllers.maps.SetMapsEnabled(true, "Menu", "AssistedControlLeft");
                    }
                    else
                    {
                        inputPlayer.controllers.maps.SetMapsEnabled(true, "Character", "AssistedControlLeft");
                    }
                    return;
                }
            }
        }
    }

    void CheckForPlayerRemove()
    {
        for (var index = players.Count - 1; index >= 0; index--)
        {
            var player = players[index];
            var controls = player.GetControls();
            var input = controls.GetPlayerInput();
            if ( controls.GetPlayerInput().controllers.joystickCount == 0)
            {
                RemoveInputPlayer(input);
            }
            else
            {
                controls.SetCurrentLayout(PlayerControls.eLayout.ASSISTED);
                controls.SetMapEnabled(true, "Character");
            }
        }
    }

    private void RemoveInputPlayer(Rewired.Player inputPlayer)
    {
        Player player;
        if (isInSelectionMenu)
        {
            foreach (PlayerMenuSelection menu in selection.PlayersSelections)
            {
                player = menu.GetCurrentPlayer();
                if (inputPlayer.id == player.GetControls().GetPlayerId())
                {
                    menu.Leave(true);
                    break;
                }
            }
        }
        else
        {
            player = GetPlayerFromId(inputPlayer.id);
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager == null)
            {
                return;
            }
            levelManager.LevelRules.PlayerLeaves(player);
            player.GetCurrentCharacter()?.Die(false);
            players.Remove(player);
            Destroy(player.gameObject);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (SystemInput.GetButtonDown("ActivateDebugMode"))
        {
            if (!isDebugModeOn)
            {
                if (debugModeInstance == null)
                {
                    debugModeInstance = Instantiate(DebugModePrefab, transform);
                }
                else
                {
                    debugModeInstance.gameObject.SetActive(true);
                }
                isDebugModeOn = true;
            }
            else
            {
                foreach (Player player in players)
                {
                    if (player.GetCurrentCharacter() && player.GetCurrentCharacter().LeftHand)
                    {
                        player.GetCurrentCharacter().LeftHand.debugBlockHand = false;
                    }

                    if (player.GetCurrentCharacter() && player.GetCurrentCharacter().RightHand)
                    {
                        player.GetCurrentCharacter().RightHand.debugBlockHand = false;
                    }
                }
                debugModeInstance.gameObject.SetActive(false);
                isDebugModeOn = false;
            }
        }
#endif
        if (players.Count < NbMaxPlayers)
        {
            CheckForNewPlayers();
        }
    }

    void CheckForNewPlayers()
    {
        if (isInSelectionMenu || isInMainMenu || isTitle
            || levelSelectorManagerInstance.GetCurrentGameMode() == LevelSelector.eGameMode.SOLO || levelSelectorManagerInstance.GetCurrentLevel() == null
            || levelSelectorManagerInstance.GetCurrentLevel().LevelType == LevelProperties.eLevelType.OTHER || levelSelectorManagerInstance.GetCurrentLevel().LevelType == LevelProperties.eLevelType.MINIGAME)
        {
            return;
        }

        foreach (Rewired.Player playerInput in ReInput.players.GetPlayers())
        {
            if (GetPlayerFromId(playerInput.id) == null && GetEnterGameButton(playerInput))
            {
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                if (levelManager == null)
                {
                    return;
                }
                Player player = Instantiate(PlayerPrefab);
                player.OrderInGame = GetUnusedOrderInGame();
                player.GetControls().ListenPlayerInput(playerInput.id);
                player.GetControls().SetCurrentLayout(PlayerControls.eLayout.ASSISTED);
                if (playerInput.controllers.hasKeyboard)
                {
                    player.GetControls().SetControlsMode(PlayerControls.eControlsMode.KEYBOARD);
                }
                player.GetControls().SetMapEnabled(true, "Character");
                AddPlayer(player);
                player.SetCharacterOutfit(Character.LoadOutfitFromPreset(Presets.GetRandomOutfit(saveManager.GameSaveData.UnlockedOutfits)));
                levelManager.LevelRules.InitPlayer(player);
            }
        }
    }

    bool GetEnterGameButton(Rewired.Player playerInput)
    {
        return playerInput.GetButtonDown(RewiredConsts.Action.Character_EnterGame);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInSelectionMenu = scene.name.Contains(selectionMenuName);
        isInMainMenu = scene.name == mainMenuName;
        isTitle = scene.name.Contains(titleMenuName);
        if (isInSelectionMenu)
        {
            selection = FindObjectOfType<CharacterSelection>();
        }
        chainsManager.ClearChains();
    }

    public bool GetIsDebugModeOn()
    {
        return isDebugModeOn;
    }

    public NewCharacterPresets GetCharactersPresets()
    {
        return Presets;
    }

    public bool GetIsInSelection()
    {
        return isInSelectionMenu;
    }

    public CharacterSelection GetSelectionMenu()
    {
        return selection;
    }

    public ChainsManager GetChainsManager()
    {
        return chainsManager;
    }

    public LevelSelector GetLevelSelector()
    {
        return levelSelectorManagerInstance;
    }

    public LocalizationManager GetLocalizationManager()
    {
        return localization;
    }

    public PaintManager GetPaintManager()
    {
        if (paintManager == null)
        {
            paintManager = Instantiate(PaintManagerPrefab, transform);
        }
        return paintManager;
    }

    public MusicManager GetMusicManager()
    {
        return musicManager;
    }

    public AmbianceManager GetAmbianceManager()
    {
        return ambianceManager;
    }

    public ControllerTypesManager GetControllerTypesManager()
    {
        return controllerTypesManager;
    }

    public BaseSaveManager GetSaveManager()
    {
        return saveManager;
    }

    public ScoreManager GetScoreManager()
    {
        return scoreManager;
    }

    public MetricsManager GetMetricsManager()
    {
        return metricsManager;
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void RemovePlayer(Player player, bool destroy = false)
    {
        players.Remove(player);
        if (destroy)
        {
            Destroy(player.gameObject);
        }
    }

    public void KeepSmallerId()
    {
        if (players.Count < 2)
            return;

        List<Player> playersToRemove = new List<Player>();
        Player playerToKeep = null;
        foreach (Player player in players)
        {
            if (playerToKeep == null || player.GetControls().GetPlayerId() < playerToKeep.GetControls().GetPlayerId())
            {
                playerToKeep = player;
            }
        }
        foreach (Player player in players)
        {
            if (player != playerToKeep)
            {
                playersToRemove.Add(player);
            }
        }
        foreach (Player player in playersToRemove)
        {
            RemovePlayer(player);
        }
    }

    public void RemoveAllPlayers()
    {
        foreach (Player player in players)
        {
            Destroy(player.gameObject);
        }
        players.Clear();
    }

    public bool DoesPlayerIdExists(int id)
    {
        foreach (Player player in players)
        {
            if (player.GetControls().GetPlayerId() == id)
            {
                return true;
            }
        }
        return false;
    }

    public Player GetPlayerFromId(int id)
    {
        foreach (Player player in players)
        {
            if (player.GetControls().GetPlayerId() == id)
            {
                return player;
            }
        }
        return null;
    }

    public Player GetPlayerFromOrder(int order)
    {
        foreach (Player player in players)
        {
            if (player.OrderInGame == order)
            {
                return player;
            }
        }
        return null;
    }

    public bool AreAllPlayersDead()
    {
        foreach (Player player in players)
        {
            if (player.GetCurrentCharacter() != null)
            {
                return false;
            }
        }
        return true;
    }

    public int GetNbPlayersAlive()
    {
        int nbPlayers = 0;
        foreach (Player player in players)
        {
            if (player.GetCurrentCharacter() != null)
            {
                nbPlayers++;
            }
        }
        return nbPlayers;
    }

    public int GetUnusedPlayerIdInGame()
    {
        bool isUsed = false;

        foreach (Rewired.Player rewiredPlayer in ReInput.players.GetPlayers())
        {
            for (int i = 0; i < players.Count; ++i)
            {
                if (rewiredPlayer.id == players[i].GetControls().GetPlayerId())
                {
                    isUsed = true;
                    break;
                }
            }
            if (!isUsed)
            {
                return rewiredPlayer.id;
            }
            isUsed = false;
        }
        return -1;
    }

    public int GetUnusedOrderInGame()
    {
        int newOrder = 0;
        bool noUnusedOrder = true;

        for (int i = 0; i < players.Count; ++i)
        {
            foreach (Player player in players)
            {
                if (player.OrderInGame == newOrder)
                {
                    newOrder++;
                    noUnusedOrder = false;
                    break;
                }
            }
        }
        if (noUnusedOrder)
        {
            newOrder++;
        }
        return newOrder;
    }

    public bool HasAtLeastOneAssistedControl()
    {
        foreach (Player player in players)
        {
            if (player.GetControls().GetCurrentLayout() == PlayerControls.eLayout.ASSISTED)
            {
                return true;
            }
        }
        return false;
    }
}
