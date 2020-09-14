using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour {

    public GameManager GameManagerPrefab = null;
    public Tutorial Tutorial;

    public PlayerMenuSelection[] PlayersSelections;

    SoundModule soundModule;

    bool hasStartedCountdown = false;

    public int MinimumPlayerNBToStart = 1;

    public Animator AnimCount = null;

    public GameObject MicrosoftButton;
    public GameObject SonyButton;
    public GameObject NintendoButton;
    public GameObject KeyButton;
    public GameObject BackRoot;

    public bool debugUseLevelName = false;
    [Tooltip("For testing purposes : put the name of the level you want to test")]
    public string NextLevelName = "";
    public bool debugUseWorldName = false;
    public string NextWorldName = "";

    public bool debugUseGameMode = false;
    public LevelSelector.eGameMode debugGameMode = LevelSelector.eGameMode.COOP;

    Sprite[] hairSprites;
    Sprite[] glassesSprites;
    Sprite[] facialFeaturesSprites;
    Sprite[] armSprites;
    Sprite[] forearmSprites;
    Sprite[] handSprites;
    Sprite[] handClosedSprites;
    Sprite[] handPointSprites;
    Sprite[] backSprites;
    RuntimeAnimatorController[] personalityAnims;

    bool isLoading = false;

    void Awake ()
    {
        if (GameManager.Instance == null)
        {
            Instantiate(GameManagerPrefab);
        }

        soundModule = GetComponent<SoundModule>();

        for (int i = 0; i < PlayersSelections.Length; ++i)
        {
            PlayersSelections[i].SetCharacterSelection(this);
        }
        AnimCount.gameObject.SetActive(false);

        SetCorrespondingButtons();

        if (Tutorial != null)
        {
            if (debugUseLevelName)
            {
                Tutorial.SetNextLevel(NextLevelName);
            }
            else if (debugUseWorldName)
            {
                Tutorial.SetNextWorld(NextWorldName);
            }
        }

        LoadCharactersAssets();
    }

    void Start()
    {
        GameManager.Instance.GetMusicManager().PlayMusic(GameManager.Instance.GetMusicManager().DefaultMusic);

        if (debugUseGameMode)
        {
            GameManager.Instance.GetLevelSelector().SetGameMode(debugGameMode);
        }

        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.SOLO)
        {
            if (GameManager.Instance.GetPlayers().Count > 0)
            {
                PlayersSelections[0].PlayerId = GameManager.Instance.GetPlayers()[0].GetControls().GetPlayerId(); // At this point only one player should be left in the list
                PlayersSelections[0].SetPlayer(GameManager.Instance.GetPlayers()[0], true);
            }
        }
        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.SOLO)
        {
            MinimumPlayerNBToStart = 1;
        }
        else
        {
            for (int i = 0; i < PlayersSelections.Length; ++i)
            {
                if (i < GameManager.Instance.GetPlayers().Count)
                {
                    PlayerMenuSelection menu = GetPlayerMenuFromOrderInGame(GameManager.Instance.GetPlayers()[i].OrderInGame);
                    menu.SetPlayer(GameManager.Instance.GetPlayers()[i], true);
                }
            }
            MinimumPlayerNBToStart = 2;
        }

        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            var maps = player.controllers.maps;
            maps.SetAllMapsEnabled(false);
            maps.SetMapsEnabled(true, "Menu", "AssistedControlLeft");
            maps.SetMapsEnabled(true, "Character", "AssistedControlLeft");
        }

#if UNITY_SWITCH && !UNITY_EDITOR
        nn.hid.NpadJoy.StartLrAssignmentMode();
#endif
    }

    private void OnDestroy()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        nn.hid.NpadJoy.StopLrAssignmentMode();
#endif
    }

    void LoadCharactersAssets()
    {
        hairSprites = GameManager.Instance.CharacterAssets.HairSprites;
        glassesSprites = GameManager.Instance.CharacterAssets.GlassesSprites;
        facialFeaturesSprites = GameManager.Instance.CharacterAssets.FacialFeaturesSprites;
        armSprites = GameManager.Instance.CharacterAssets.ArmSprites;
        forearmSprites = GameManager.Instance.CharacterAssets.ForearmSprites;
        handSprites = GameManager.Instance.CharacterAssets.HandSprites;
        handClosedSprites = GameManager.Instance.CharacterAssets.HandClosedSprites;
        handPointSprites = GameManager.Instance.CharacterAssets.HandPointSprites;
        backSprites = GameManager.Instance.CharacterAssets.BackSprites;
        personalityAnims = GameManager.Instance.CharacterAssets.PersonalityAnims;
    }

    public SoundModule GetSoundModule()
    {
        return soundModule;
    }

    bool DidPlayerPressJoin(Rewired.Player player)
    {
#if UNITY_SWITCH
        return player.GetButtonDown("JoinR") && player.GetButton("JoinL") ||
               player.GetButton("JoinR") && player.GetButtonDown("JoinL");
#else
        bool isNintendo = player.controllers.joystickCount > 0 && GameManager.Instance.GetControllerTypesManager().GetIsControllerNintendoFromJoystick(player.controllers.Joysticks[0]);
        return (isNintendo && player.GetButton(RewiredConsts.Action.Character_GrabLeft) && player.GetButton(RewiredConsts.Action.Character_GrabRight))
            || player.GetButtonDown(RewiredConsts.Action.Menu_Confirm);
#endif
    }
    
    void Update()
    {
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (!isLoading && !GameManager.Instance.DoesPlayerIdExists(player.id) && DidPlayerPressJoin(player))
            {
                int indexMenu = GetIndexNextEmptyPlayerMenu();
                if (!IsPlayerAlreadyInSelection(player.id) && indexMenu != -1)
                {
                    CreatePlayer(PlayersSelections[indexMenu], player.id);
                    soundModule.PlayOneShot("Confirm");
                }
                
            }
            if (player.GetButtonDown("Cancel"))
            {
                GoBackToMainMenu();
            }
        }

        if (hasStartedCountdown)
        {
            if (!IsReadyToPlay())
            {
                StopCountdown();
            }
            else
            {
                if (AnimCount.GetCurrentAnimatorStateInfo(0).IsName("EndCounting"))
                {
                    Resources.UnloadUnusedAssets();
                    if (Tutorial != null)
                    {
                        BackRoot.SetActive(false);
                        Tutorial.StartTuto();
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        SceneManager.LoadScene(NextLevelName, LoadSceneMode.Single);
                    }
                }
            }
        }
        else
        {
            if (IsReadyToPlay())
            {
                StartCountdown();
            }
        }
    }

    public bool SetUnusedId(Player player)
    {
        int playerId = GetUnusedPlayerId();
        if (playerId == -1)
        {
            return false;
        }
        player.ChangePlayerId(playerId);
        return true;
    }

    public void PutPlayerInNextEmptyPlayerMenu(Player player, bool isPlayerAlreadySetted = false)
    {
        int indexMenu = GetIndexNextEmptyPlayerMenu();
        if (!IsPlayerAlreadyInSelection(player.GetControls().GetPlayerId()) && indexMenu != -1)
        {
            PlayersSelections[indexMenu].SetPlayer(player, isPlayerAlreadySetted);
        }
    }

    int GetUnusedPlayerId()
    {
        bool isUsed = false;
        foreach (Rewired.Player rewiredPlayer in ReInput.players.GetPlayers())
        {
            for (int i = 0; i < PlayersSelections.Length; ++i)
            {
                Player player = PlayersSelections[i].GetCurrentPlayer();
                if (player != null && player.GetControls().GetPlayerId() == rewiredPlayer.id)
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

    public PlayerMenuSelection GetPlayerMenuFromId(int id)
    {
        foreach (PlayerMenuSelection menu in PlayersSelections)
        {
            if (menu.PlayerId == id)
            {
                return menu;
            }
        }
        return null;
    }

    public PlayerMenuSelection GetPlayerMenuFromOrderInGame(int orderInGame)
    {
        foreach (PlayerMenuSelection menu in PlayersSelections)
        {
            if (menu.PlayerId == orderInGame)
            {
                return menu;
            }
        }
        return null;
    }

    public Player CreatePlayer(PlayerMenuSelection menuSelection, int playerId)
    {
        Player player = Instantiate(GameManager.Instance.PlayerPrefab);
        player.ChangePlayerId(playerId);
        menuSelection.SetPlayer(player);
        return player;
    }

    int GetIndexNextEmptyPlayerMenu()
    {
        for (int i = 0; i < PlayersSelections.Length; ++i)
        {
            if (PlayersSelections[i].GetIsInit() && PlayersSelections[i].GetCurrentPlayer() == null)
            {
                return i;
            }
        }
        return -1;
    }

    bool IsPlayerAlreadyInSelection(int id)
    {
        for (int i = 0; i < PlayersSelections.Length; ++i)
        {
            Player currentPlayer = PlayersSelections[i].GetCurrentPlayer();
            if (currentPlayer != null && currentPlayer.GetControls().GetPlayerId() == id)
            {
                return true;
            }
        }
        return false;
    }

    bool IsReadyToPlay()
    {
        int nbPlayerReady = 0;
        for (int i = 0; i < PlayersSelections.Length; ++i)
        {
            if (PlayersSelections[i].GetCurrentState() == PlayerMenuSelection.eSelectionState.READY)
            {
                nbPlayerReady++;
            }
            else if (PlayersSelections[i].GetCurrentState() == PlayerMenuSelection.eSelectionState.SELECTION)
                //|| PlayersSelections[i].GetCurrentState() == PlayerMenuSelection.eSelectionState.CONTROLS)
            {
                return false;
            }
        }
        if (nbPlayerReady >= MinimumPlayerNBToStart)
        {
            return true;
        }
        return false;
    }

    public void StartCountdown()
    {
        AnimCount.gameObject.SetActive(true);
        soundModule.PlayEvent("Count");
        hasStartedCountdown = true;
    }

    public void StopCountdown()
    {
        AnimCount.gameObject.SetActive(false);
        soundModule.StopEvent("Count");
        hasStartedCountdown = false;
    }

    public Sprite GetHairSprite(string spriteName)
    {
        for (int i = 0; i < hairSprites.Length; ++i)
        {
            if (hairSprites[i].name == spriteName)
            {
                return hairSprites[i];
            }
        }
        return null;
    }

    public Sprite GetGlassesSprite(string spriteName)
    {
        for (int i = 0; i < glassesSprites.Length; ++i)
        {
            if (glassesSprites[i].name == spriteName)
            {
                return glassesSprites[i];
            }
        }
        return null;
    }

    public Sprite GetFacialFeaturesSprite(string spriteName)
    {
        for (int i = 0; i < facialFeaturesSprites.Length; ++i)
        {
            if (facialFeaturesSprites[i].name == spriteName)
            {
                return facialFeaturesSprites[i];
            }
        }
        return null;
    }

    public Sprite GetArmSprite(string spriteName)
    {
        for (int i = 0; i < armSprites.Length; ++i)
        {
            if (armSprites[i].name == spriteName)
            {
                return armSprites[i];
            }
        }
        return null;
    }

    public Sprite GetForearmSprite(string spriteName)
    {
        for (int i = 0; i < forearmSprites.Length; ++i)
        {
            if (forearmSprites[i].name == spriteName)
            {
                return forearmSprites[i];
            }
        }
        return null;
    }

    public Sprite GetHandSprite(string spriteName)
    {
        for (int i = 0; i < handSprites.Length; ++i)
        {
            if (handSprites[i].name == spriteName)
            {
                return handSprites[i];
            }
        }
        return null;
    }

    public Sprite GetHandClosedSprite(string spriteName)
    {
        for (int i = 0; i < handClosedSprites.Length; ++i)
        {
            if (handClosedSprites[i].name == spriteName)
            {
                return handClosedSprites[i];
            }
        }
        return null;
    }

    public Sprite GetHandPointSprite(string spriteName)
    {
        for (int i = 0; i < handPointSprites.Length; ++i)
        {
            if (handPointSprites[i].name == spriteName)
            {
                return handPointSprites[i];
            }
        }
        return null;
    }

    public Sprite GetBackSprite(string spriteName)
    {
        for (int i = 0; i < backSprites.Length; ++i)
        {
            if (backSprites[i].name == spriteName)
            {
                return backSprites[i];
            }
        }
        return null;
    }

    public RuntimeAnimatorController GetPersonalityAnim(string personalityName)
    {
        for (int i = 0; i < personalityAnims.Length; ++i)
        {
            if (personalityAnims[i].name == personalityName)
            {
                return personalityAnims[i];
            }
        }
        return null;
    }

    /*public bool IsColorAlreadySelected(PlayerMenuSelection menu, CharacterPresets.ColorPreset colorPreset)
    {
        foreach (PlayerMenuSelection playerSelection in PlayersSelections)
        {
            if (playerSelection != menu && playerSelection.GetCurrentState() != PlayerMenuSelection.eSelectionState.WAITING
                && Presets.GetColorPreset(playerSelection.ColorSelection.Options[playerSelection.ColorSelection.Index]).IsSame(colorPreset))
            {
                return true;
            }
        }
        return false;
    }*/

    public void GoBackToMainMenu()
    {
        foreach (PlayerMenuSelection selection in PlayersSelections)
        {
            if (selection.GetCurrentState() != PlayerMenuSelection.eSelectionState.WAITING)
                return;
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        isLoading = true;
    }

    void SetCorrespondingButtons()
    {
        Rewired.Player playerWithJoystick = null;
        foreach (var player in ReInput.players.GetPlayers())
        {
            if (player.controllers.joystickCount != 0)
            {
                playerWithJoystick = player;
                break;
            }
        }

        ControllerTypesManager.eControllerType type = ControllerTypesManager.eControllerType.KEYBOARD;
        if (playerWithJoystick != null)
        {
            type = GameManager.Instance.GetControllerTypesManager().GetControllerTypeFromJoystick(playerWithJoystick.controllers.Joysticks[0] as Joystick);
        }

        switch (type)
        {
            case ControllerTypesManager.eControllerType.KEYBOARD:
                {
                    MicrosoftButton.SetActive(false);
                    SonyButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    KeyButton.SetActive(true);
                    break;
                }
            case ControllerTypesManager.eControllerType.XBOX360:
            case ControllerTypesManager.eControllerType.XBOXONE:
                {
                    MicrosoftButton.SetActive(true);
                    SonyButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    KeyButton.SetActive(false);
                    break;
                }
            case ControllerTypesManager.eControllerType.JOYCON_DUAL:
            case ControllerTypesManager.eControllerType.JOYCON_LEFT:
            case ControllerTypesManager.eControllerType.JOYCON_RIGHT:
            case ControllerTypesManager.eControllerType.JOYCON_PRO:
            case ControllerTypesManager.eControllerType.JOYCON_HANDHELD:
                {
                    MicrosoftButton.SetActive(false);
                    SonyButton.SetActive(false);
                    NintendoButton.SetActive(true);
                    KeyButton.SetActive(false);
                    break;
                }
            case ControllerTypesManager.eControllerType.BITPRO8:
            case ControllerTypesManager.eControllerType.PS4:
            default:
                {
                    SonyButton.SetActive(true);
                    MicrosoftButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    KeyButton.SetActive(false);
                    break;
                }
        }
    }
}
