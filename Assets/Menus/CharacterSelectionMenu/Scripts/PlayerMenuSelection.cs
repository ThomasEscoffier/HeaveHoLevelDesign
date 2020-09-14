using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using Rewired.Integration.UnityUI;

public class PlayerMenuSelection : MonoBehaviour
{
    public RewiredStandaloneInputModule InputModule;
    public MultipleEventSystem CurrentEventSystem;

    public CharacterPresetTryOn CharacterModelPrefab;
    public CharacterPresetTryOn CharacterModel;

    CharacterSelection characterSelection;
    Character.Outfit currentOutfit;

    public int PlayerId = 0;

    Player currentPlayer;
    Rewired.Player PlayerInput = null;

    public GameObject[] SonyButtons;
    public GameObject[] MicrosoftButtons;
    public GameObject[] NintendoButtons;
    public GameObject[] KeyboardButtons;

    public GameObject Selection;
    public GameObject Controls;
    public GameObject WaitingForPlayer;
    public GameObject Ready;

    public ArrowSelection PresetSelection;
    public ArrowSelection ColorSelection;
    public ArrowSelection HairSelection;
    public ArrowSelection GlassesSelection;
    public ArrowSelection FacialFeaturesSelection;
    public ArrowSelection ArmsSelection;
    public ArrowSelection PersonalitySelection;
    public ArrowSelection AssistedSelection;

    public Text RemoveButtonText;
    public Image[] RemoveButtonsImage;
    public Color RemoveTextNormalColor = Color.white;
    public Color RemoveTextDisabledColor = Color.grey;
    bool canRemove = false;
    bool changePersonality = true;

    bool hasAlreadyChangedPanelThisframe = false;

    public GameObject FirstSelectedSelection;
    public List<ControlsButton> ButtonsSelectedControls;
    public AAccessory AssistedAccessoryLeft = null;
    public AAccessory AssistedAccessoryRight = null;

    bool isInit = false;

    public enum eSelectionState
    {
        WAITING,
        SELECTION,
        //CONTROLS,
        READY,
    }

    eSelectionState currentState = eSelectionState.WAITING;

    public eSelectionState GetCurrentState()
    {
        return currentState;
    }

    public void SetCharacterSelection(CharacterSelection newCharacterSelection)
    {
        characterSelection = newCharacterSelection;
    }

    public void SetPlayer(Player player, bool alreadySetted = false)
    {
        currentPlayer = player;
        ListenToId(currentPlayer.GetControls().GetPlayerId());
        currentPlayer.GetControls().SetCurrentLayout(PlayerControls.eLayout.DEFAULT);
        if (ReInput.players.GetPlayer(currentPlayer.GetControls().GetPlayerId()).controllers.hasKeyboard)
        {
            currentPlayer.GetControls().SetControlsMode(PlayerControls.eControlsMode.KEYBOARD);
        }
        currentPlayer.OrderInGame = transform.GetSiblingIndex();
        WaitingForPlayer.SetActive(false);
        Selection.SetActive(true);
        CurrentEventSystem.SetSelectedGameObject(FirstSelectedSelection);

        CharacterModel.gameObject.SetActive(true);
        currentState = eSelectionState.SELECTION;
        hasAlreadyChangedPanelThisframe = true;

        if (!alreadySetted)
        {
            GameManager.Instance.AddPlayer(currentPlayer);
            ChangePersonality(currentOutfit.Personality);
            ChangeAssisted(AssistedSelection);
        }
        else
        {
            currentOutfit = player.GetCurrentCharacterOutfit();
            if (currentOutfit.BodyColor == new Color(0f, 0f, 0f, 0f))
            {
                ApplyColorPresetToOutfit(GameManager.Instance.GetCharactersPresets().GetColorPreset(ColorSelection.Options[ColorSelection.Index]));
                ChangePersonality(PersonalitySelection, false);
            }
            else
            {
                ChangePersonality(player.GetCurrentPersonality());
                RefreshSelections();
                CharacterModel.LoadOutfit(currentOutfit);
                ChangeAssisted(currentPlayer.GetIsAssisted());
            }
        }
    }

    public void ListenToId(int newId)
    {
        PlayerInput = ReInput.players.GetPlayer(newId);
        int[] playerIds = new int[1];
        playerIds[0] = newId;
        InputModule.RewiredPlayerIds = playerIds;
        SetCorrespondingButtons();

        PresetSelection.ListenPlayerId(newId);
        ColorSelection.ListenPlayerId(newId);
        HairSelection.ListenPlayerId(newId);
        GlassesSelection.ListenPlayerId(newId);
        FacialFeaturesSelection.ListenPlayerId(newId);
        ArmsSelection.ListenPlayerId(newId);
        PersonalitySelection.ListenPlayerId(newId);
        AssistedSelection.ListenPlayerId(newId);

        CharacterModel.ListenPlayerId(newId);

        if (currentPlayer != null)
        {
            if (currentState == eSelectionState.READY)
            {
                currentPlayer.GetControls().SetMapEnabled(true, "Character");
            }
            else
            {
                currentPlayer.GetControls().SetMapEnabled(false, "Character");
            }
        }
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public RewiredStandaloneInputModule GetCurrentInputModule()
    {
        return InputModule;
    }

    void SetCorrespondingButtons()
    {
        ControllerTypesManager.eControllerType type;

        if (PlayerInput.controllers.joystickCount > 0)
        {
            type = GameManager.Instance.GetControllerTypesManager().GetControllerTypeFromJoystick(PlayerInput.controllers.Joysticks[0] as Joystick);
        }
        else if (PlayerInput.controllers.hasKeyboard)
        {
            type = ControllerTypesManager.eControllerType.KEYBOARD;
        }
        else
        {
            if (ReInput.players.GetPlayers()[0].controllers.joystickCount > 0)
            {
                type = GameManager.Instance.GetControllerTypesManager().GetControllerTypeFromJoystick(ReInput.players.GetPlayers()[0].controllers.Joysticks[0] as Joystick);
            }
            else
            {
                type = ControllerTypesManager.eControllerType.KEYBOARD;
            }
        }
        switch (type)
        {
            case ControllerTypesManager.eControllerType.KEYBOARD:
                {
                    for (int i = 0; i < MicrosoftButtons.Length; ++i)
                    {
                        MicrosoftButtons[i].SetActive(false);
                        SonyButtons[i].SetActive(false);
                        NintendoButtons[i].SetActive(false);
                        KeyboardButtons[i].SetActive(true);
                    }
                    break;
                }
            case ControllerTypesManager.eControllerType.XBOX360:
            case ControllerTypesManager.eControllerType.XBOXONE:
                {
                    for (int i = 0; i < MicrosoftButtons.Length; ++i)
                    {
                        MicrosoftButtons[i].SetActive(true);
                        SonyButtons[i].SetActive(false);
                        NintendoButtons[i].SetActive(false);
                        KeyboardButtons[i].SetActive(false);
                    }
                    break;
                }
            case ControllerTypesManager.eControllerType.JOYCON_DUAL:
            case ControllerTypesManager.eControllerType.JOYCON_LEFT:
            case ControllerTypesManager.eControllerType.JOYCON_RIGHT:
            case ControllerTypesManager.eControllerType.JOYCON_PRO:
            case ControllerTypesManager.eControllerType.JOYCON_HANDHELD:
                {
                    for (int i = 0; i < NintendoButtons.Length; ++i)
                    {
                        MicrosoftButtons[i].SetActive(false);
                        SonyButtons[i].SetActive(false);
                        NintendoButtons[i].SetActive(true);
                        KeyboardButtons[i].SetActive(false);
                    }
                    break;
                }
            case ControllerTypesManager.eControllerType.BITPRO8:
            case ControllerTypesManager.eControllerType.PS4:
            default:
                {
                    for (int i = 0; i < SonyButtons.Length; ++i)
                    {
                        SonyButtons[i].SetActive(true);
                        MicrosoftButtons[i].SetActive(false);
                        NintendoButtons[i].SetActive(false);
                        KeyboardButtons[i].SetActive(false);
                    }
                    break;
                }
        }
    }

    void Awake()
    {
        ListenToId(PlayerId);

        Selection.SetActive(false);
        Controls.SetActive(false);
        WaitingForPlayer.SetActive(true);
        Ready.SetActive(false);
        CharacterModel.SetIsStatic(true);
        CharacterModel.gameObject.SetActive(false);

        PresetSelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        PresetSelection.OnSelected.AddListener(UpdateRemoveButton);

        ColorSelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        ColorSelection.OnSelected.AddListener(UpdateRemoveButton);

        HairSelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        HairSelection.OnSelected.AddListener(UpdateRemoveButton);

        GlassesSelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        GlassesSelection.OnSelected.AddListener(UpdateRemoveButton);

        FacialFeaturesSelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        FacialFeaturesSelection.OnSelected.AddListener(UpdateRemoveButton);

        ArmsSelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        ArmsSelection.OnSelected.AddListener(UpdateRemoveButton);

        PersonalitySelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        PersonalitySelection.OnSelected.AddListener(UpdateRemoveButton);

        AssistedSelection.SetCurrentEventSystem(CurrentEventSystem, InputModule);
        AssistedSelection.OnSelected.AddListener(UpdateRemoveButton);

        FillPresetLists();

        SetCorrespondingButtons();
        InitCharacters();
        isInit = true;
    }

    public void InitCharacters()
    {
        SetCharacterNaked();
        RandomColor();
        PersonalitySelection.Index = Random.Range(0, PersonalitySelection.Options.Count);
        ChangePersonality(PersonalitySelection, false);
    }

    public bool GetIsInit()
    {
        return isInit;
    }

    void Update()
    {
        switch (currentState)
        {
            case eSelectionState.SELECTION:
                {
                    if (PlayerInput.GetButtonDown(RewiredConsts.Action.Menu_Special))
                    {
                        RandomCharacter();
                    }
                    else if (!hasAlreadyChangedPanelThisframe && PlayerInput.GetButtonDown(RewiredConsts.Action.Menu_Confirm))
                    {
                        ConfirmOutfit();
                    }
                    else if (!hasAlreadyChangedPanelThisframe && PlayerInput.GetButtonDown(RewiredConsts.Action.Menu_Cancel))
                    {
                        Leave();
                    }
                    else if (PlayerInput.GetButtonDown(RewiredConsts.Action.Menu_Remove))
                    {
                        if (canRemove)
                        {
                            Remove();
                        }
                    }
                    break;
                }
            /*case eSelectionState.CONTROLS:
                if (!hasAlreadyChangedPanelThisframe && PlayerInput.GetButtonDown(RewiredConsts.Action.Menu_Cancel))
                {
                    ReturnToSelection();
                }
                break;*/
            case eSelectionState.READY:
                {
                    if (!hasAlreadyChangedPanelThisframe && PlayerInput.GetButtonDown(RewiredConsts.Action.Menu_Cancel))
                    {
                        Cancel();
                    }
                    break;
                }
            case eSelectionState.WAITING:
            default:
                break;
        }
        hasAlreadyChangedPanelThisframe = false;
    }

    bool HasPlayerJoystick()
    {
        return PlayerInput.controllers.joystickCount > 0;
    }

    public void FillPresetLists()
    {
        ArrowSelection firstSelected = null;
        List<string> hairNames = new List<string>();
        List<string> glassesNames = new List<string>();
        List<string> facialFeaturesNames = new List<string>();
        List<string> armsNames = new List<string>();

        PresetSelection.ClearOptions();
        List<string> presetsNames = new List<string>();
        foreach (string characterName in GameManager.Instance.GetSaveManager().GameSaveData.UnlockedOutfits)
        {
            CharacterPreset character = GameManager.Instance.GetCharactersPresets().GetCharacterPreset(characterName);
            presetsNames.Add(character.Name);
            if (!hairNames.Contains(character.HairName))
            {
                hairNames.Add(character.HairName);
            }
            if (!glassesNames.Contains(character.GlassesName))
            {
                glassesNames.Add(character.GlassesName);
            }
            if (!facialFeaturesNames.Contains(character.FacialFeaturesName))
            {
                facialFeaturesNames.Add(character.FacialFeaturesName);
            }
            if (!armsNames.Contains(character.Arms.Name))
            {
                armsNames.Add(character.Arms.Name);
            }
        }
        presetsNames.Sort();
        PresetSelection.AddOptions(presetsNames);
        PresetSelection.SetIsDisabled(PresetSelection.Options.Count <= 1);
        if (firstSelected == null && PresetSelection.Options.Count > 1)
        {
            firstSelected = PresetSelection;
        }

        ColorSelection.ClearOptions();
        List<string> colorsNames = new List<string>();
        foreach (ColorPreset color in GameManager.Instance.GetCharactersPresets().GetColors())
        {
            if (!string.IsNullOrEmpty(color.Name))
            {
                colorsNames.Add(color.Name);
            }
        }
        colorsNames.Sort();
        ColorSelection.AddOptions(colorsNames);
        ColorSelection.SetIsDisabled(ColorSelection.Options.Count == 0);
        if (firstSelected == null && ColorSelection.Options.Count != 0)
        {
            firstSelected = ColorSelection;
        }

        ArmsSelection.ClearOptions();
        armsNames.Sort();
        ArmsSelection.AddOptions(armsNames);
        ArmsSelection.SetIsDisabled(ArmsSelection.Options.Count <= 1);
        if (firstSelected == null && ArmsSelection.Options.Count > 1)
        {
            firstSelected = ArmsSelection;
        }

        List<string> assetNames = GameManager.Instance.GetCharactersPresets().HairDefault;
        HairSelection.ClearOptions();
        for (int i = 0; i < assetNames.Count; ++i)
        {
            hairNames.Add(assetNames[i]);
        }
        hairNames.Sort();
        HairSelection.AddOptions(hairNames);
        HairSelection.SetIsDisabled(HairSelection.Options.Count <= 1);
        if (firstSelected == null && HairSelection.Options.Count > 1)
        {
            firstSelected = HairSelection;
        }

        GlassesSelection.ClearOptions();
        assetNames = GameManager.Instance.GetCharactersPresets().GlassesDefault;
        for (int i = 0; i < assetNames.Count; ++i)
        {
            glassesNames.Add(assetNames[i]);
        }
        glassesNames.Sort();
        GlassesSelection.AddOptions(glassesNames);
        GlassesSelection.SetIsDisabled(GlassesSelection.Options.Count <= 1);
        if (firstSelected == null && GlassesSelection.Options.Count > 1)
        {
            firstSelected = GlassesSelection;
        }

        FacialFeaturesSelection.ClearOptions();
        assetNames = GameManager.Instance.GetCharactersPresets().FacialFeaturesDefault;
        for (int i = 0; i < assetNames.Count; ++i)
        {
            facialFeaturesNames.Add(assetNames[i]);
        }
        facialFeaturesNames.Sort();
        FacialFeaturesSelection.AddOptions(facialFeaturesNames);
        FacialFeaturesSelection.SetIsDisabled(FacialFeaturesSelection.Options.Count <= 1);
        if (firstSelected == null && FacialFeaturesSelection.Options.Count > 1)
        {
            firstSelected = FacialFeaturesSelection;
        }

        //if nothing is available, last option always has On/Off to chose
        if (firstSelected == null)
        {
            firstSelected = AssistedSelection;
        }

        FirstSelectedSelection = firstSelected.gameObject;
        CurrentEventSystem.SetSelectedGameObject(firstSelected.gameObject);
    }

    public void ChangeCharacterPreset(ArrowSelection dropDown)
    {
        Color previousColor = currentOutfit.BodyColor;
        ApplyPresetToOutfit(GameManager.Instance.GetCharactersPresets().GetCharacterPreset(dropDown.Options[dropDown.Index]), changePersonality);
        if (currentOutfit.BodyColor == new Color(0f, 0f, 0f, 1f))
        {
            if (previousColor != Color.clear)
            {
                currentOutfit.BodyColor = previousColor;
            }
            else
            {
                RandomColor();
            }
        }
        CharacterModel.LoadOutfit(currentOutfit);
        RefreshSelections();
        changePersonality = true;
        /*if (characterSelection.IsColorAlreadySelected(this, currentSet.BodyColor))
        {
            RandomColor();
        }*/
    }

    public void ChangeColor(ArrowSelection dropDown)
    {
        /*if (characterSelection.IsColorAlreadySelected(this, characterSelection.GetPresets().GetColorPreset(dropDown.Options[dropDown.Index])))
        {
            if (dropDown.GetIsLastCommandDirectionLeft())
            {
                dropDown.Index--;
            }
            else
            {
                dropDown.Index++;
            }
            if (dropDown.Index >= dropDown.Options.Count)
            {
                dropDown.Index = 0;
            }
            else if (dropDown.Index < 0)
            {
                dropDown.Index = dropDown.Options.Count - 1;
            }
            ChangeColor(dropDown);
        }
        else
        {*/
        ApplyColorPresetToOutfit(GameManager.Instance.GetCharactersPresets().GetColorPreset(dropDown.Options[dropDown.Index]));
        CharacterModel.LoadOutfit(currentOutfit);
        //}
    }

    public void ChangeHair(ArrowSelection dropDown)
    {
        if (dropDown.Options[dropDown.Index].Equals(""))
        {
            currentOutfit.HairSprite = null;
            CharacterModel.HairSprite = null;
            currentOutfit.IsRobot = false;
            currentOutfit.IsMask = false;
            currentOutfit.IsHideFace = false;
        }
        else
        {
            currentOutfit.HairSprite = characterSelection.GetHairSprite(dropDown.Options[dropDown.Index]);
            CharacterModel.HairSprite = currentOutfit.HairSprite;
            currentOutfit.IsRobot = GameManager.Instance.GetCharactersPresets().IsHeadRobot(CharacterModel.HairSprite != null ? CharacterModel.HairSprite.name : "");
            currentOutfit.IsMask = GameManager.Instance.GetCharactersPresets().IsHeadMask(CharacterModel.HairSprite != null ? CharacterModel.HairSprite.name : "");
            currentOutfit.IsHideFace = GameManager.Instance.GetCharactersPresets().IsHeadHideFace(CharacterModel.HairSprite != null ? CharacterModel.HairSprite.name : "");
        }
        CharacterModel.SetFaceVisible(!currentOutfit.IsHideFace);
        CharacterModel.RefreshModelDisplay();
    }

    public void ChangeGlasses(ArrowSelection dropDown)
    {
        if (dropDown.Options[dropDown.Index].Equals(""))
        {
            currentOutfit.GlassesSprite = null;
            CharacterModel.GlassesSprite = null;
        }
        else
        {
            currentOutfit.GlassesSprite = characterSelection.GetGlassesSprite(dropDown.Options[dropDown.Index]);
            CharacterModel.GlassesSprite = currentOutfit.GlassesSprite;
        }
        CharacterModel.RefreshModelDisplay();
    }

    public void ChangeFacialFeatures(ArrowSelection dropDown)
    {
        if (dropDown.Options[dropDown.Index].Equals(""))
        {
            currentOutfit.FacialFeaturesSprite = null;
            CharacterModel.FacialFeaturesSprite = null;
        }
        else
        {
            currentOutfit.FacialFeaturesSprite = characterSelection.GetFacialFeaturesSprite(dropDown.Options[dropDown.Index]);
            CharacterModel.FacialFeaturesSprite = currentOutfit.FacialFeaturesSprite;
        }
        CharacterModel.RefreshModelDisplay();
    }

    public void ChangeArms(ArrowSelection dropDown)
    {
        if (dropDown.Options[dropDown.Index] == "")
        {
            currentOutfit.LeftArmSprite = null;
            currentOutfit.LeftForearmSprite = null;
            currentOutfit.LeftHandSprite = null;
            currentOutfit.LeftHandClosedSprite = null;
            currentOutfit.LeftHandPointSprite = null;

            currentOutfit.RightArmSprite = null;
            currentOutfit.RightForearmSprite = null;
            currentOutfit.RightHandSprite = null;
            currentOutfit.RightHandClosedSprite = null;
            currentOutfit.RightHandPointSprite = null;
            currentOutfit.IsHideHands = false;
        }
        else
        {
            ArmsPreset arms = GameManager.Instance.GetCharactersPresets().GetArmPreset(dropDown.Options[dropDown.Index]);
            currentOutfit.LeftArmSprite = characterSelection.GetArmSprite(arms.LeftArmName);
            currentOutfit.LeftForearmSprite = characterSelection.GetForearmSprite(arms.LeftForearmName);
            currentOutfit.LeftHandSprite = characterSelection.GetHandSprite(arms.LeftHandName);
            currentOutfit.LeftHandClosedSprite = characterSelection.GetHandClosedSprite(arms.LeftHandClosedName);
            currentOutfit.LeftHandPointSprite = characterSelection.GetHandPointSprite(arms.LeftHandPointName);

            currentOutfit.RightArmSprite = characterSelection.GetArmSprite(arms.RightArmName);
            currentOutfit.RightForearmSprite = characterSelection.GetForearmSprite(arms.RightForearmName);
            currentOutfit.RightHandSprite = characterSelection.GetHandSprite(arms.RightHandName);
            currentOutfit.RightHandClosedSprite = characterSelection.GetHandClosedSprite(arms.RightHandClosedName);
            currentOutfit.RightHandPointSprite = characterSelection.GetHandPointSprite(arms.RightHandPointName);
            currentOutfit.IsHideHands = arms.IsHideHands;

        }
        CharacterModel.LoadOutfit(currentOutfit);
    }


    public void ChangePersonality(ArrowSelection dropDown)
    {
        ChangePersonality(dropDown, true);
    }
    
    public void ChangePersonality(ArrowSelection dropDown, bool isPlayingSound = true)
    {
        Character.ePersonality personality = Character.ePersonality.HAPPY;
        CharacterModel.GetSoundModule().StopAllPlayingEvents();

        if (dropDown.Options[dropDown.Index].Equals(Character.GetPersonalityName(Character.ePersonality.GRUMPY)))
        {
            personality = Character.ePersonality.GRUMPY;
        }
        else if (dropDown.Options[dropDown.Index].Equals(Character.GetPersonalityName(Character.ePersonality.SCARED)))
        {
            personality = Character.ePersonality.SCARED;
        }
        else if (dropDown.Options[dropDown.Index].Equals(Character.GetPersonalityName(Character.ePersonality.WEIRDO)))
        {
            personality = Character.ePersonality.WEIRDO;
        }

        currentOutfit.Personality = personality;
        CharacterModel.SetPersonalityAnim(personality);
        if (isPlayingSound)
        {
            CharacterModel.GetSoundModule().StopCharacterEvent();
            CharacterModel.GetSoundModule().InitPersonality(personality, currentOutfit.IsRobot, currentOutfit.IsMask, false, CharacterSoundModule.eState.SELECTED);
        }
    }

    public void ChangePersonality(Character.ePersonality personality, bool isPlayingSound = true)
    {
        for (int i = 0; i < PersonalitySelection.Options.Count; ++i)
        {
            if (PersonalitySelection.Options[i].Equals(Character.GetPersonalityName(personality)))
            {
                PersonalitySelection.Index = i;
            }
        }
        ChangePersonality(PersonalitySelection, isPlayingSound);
    }


    public void ChangeAssisted(bool state)
    {
        for (int i = 0; i < AssistedSelection.Options.Count; ++i)
        {
            if (!state && AssistedSelection.Options[i].Equals("Off")
                || state && AssistedSelection.Options[i].Equals("On"))
            {
                AssistedSelection.Index = i;
            }
        }
        ChangeAssisted(AssistedSelection);
    }

    public void ChangeAssisted(ArrowSelection dropDown)
    {
        if (dropDown.Options[dropDown.Index].Equals("Off"))
        {
            currentPlayer.SetIsAssisted(false);
            CharacterModel.GetLeftHand().RemoveCurrentAccessory();
            CharacterModel.GetRightHand().RemoveCurrentAccessory();
            CharacterModel.SetHandVisible(CharacterModel.GetLeftHand(), !currentOutfit.IsHideHands, false);
            CharacterModel.SetHandVisible(CharacterModel.GetRightHand(), !currentOutfit.IsHideHands, false);
        }
        else if (dropDown.Options[dropDown.Index].Equals("On"))
        {
            currentPlayer.SetIsAssisted(true);
            if (!CharacterModel.GetLeftHand().HasAccessory())
            {
                CharacterModel.GetLeftHand().AddNewAccessory(Instantiate(AssistedAccessoryLeft, CharacterModel.GetLeftHand().transform));
            }
            if (!CharacterModel.GetRightHand().HasAccessory())
            {
                CharacterModel.GetRightHand().AddNewAccessory(Instantiate(AssistedAccessoryRight, CharacterModel.GetRightHand().transform));
            }
        }
    }

    public void ConfirmOutfit()
    {
        currentPlayer.SetCharacterOutfit(currentOutfit);
        Selection.SetActive(false);
        Ready.SetActive(true);
        //Controls.SetActive(true);
        CharacterModel.SetIsStatic(false);
        CharacterModel.ListenPlayerId(PlayerInput.id);

        //CurrentEventSystem.SetSelectedGameObject(ButtonsSelectedControls[0].gameObject);

        currentPlayer.GetControls().SetCurrentLayout(PlayerControls.eLayout.ASSISTED);
        currentPlayer.GetControls().SetMapEnabled(true, "Character");
        characterSelection.GetSoundModule().PlayOneShot("Selected");

        //currentState = eSelectionState.CONTROLS;
        currentState = eSelectionState.READY;
    }

    /*public void ReturnToSelection()
    {
        //Controls.SetActive(false);
        Ready.SetActive(false);
        Selection.SetActive(true);

        CurrentEventSystem.SetSelectedGameObject(FirstSelectedSelection);

        currentState = eSelectionState.SELECTION;
        characterSelection.GetSoundModule().PlayOneShot("Cancel");
    }*/

    /*public void ConfirmControls(PlayerControls.eLayout layout)
    {
        Controls.SetActive(false);
        Ready.SetActive(true);
        CharacterModel.SetIsStatic(false);
        CharacterModel.ListenPlayerId(PlayerInput.id);

        //Deselect all buttons
        CurrentEventSystem.SetSelectedGameObject(null);
        foreach (ControlsButton button in ButtonsSelectedControls)
        {
            button.OnDeselect(null);
        }

        currentPlayer.GetControls().SetCurrentLayout(layout);
        currentPlayer.GetControls().SetMapEnabled(true, "Character");
        characterSelection.GetSoundModule().PlayOneShot("Selected");

        currentState = eSelectionState.READY;
    }*/

    public void Cancel()
    {
        ResetModel();

        Ready.SetActive(false);
        //Controls.SetActive(true);
        Selection.SetActive(true);

        //currentState = eSelectionState.CONTROLS;
        currentState = eSelectionState.SELECTION;
        characterSelection.GetSoundModule().PlayOneShot("Cancel");
    }

    public void Leave(bool force = false)
    {
        if (GameManager.Instance.GetPlayers().Contains(currentPlayer))
        {
            GameManager.Instance.RemovePlayer(currentPlayer, true);
        }

        if (force)
        {
            Ready.SetActive(false);
            //Controls.SetActive(false);
            ResetModel();
        }

        Selection.SetActive(false);
        WaitingForPlayer.SetActive(true);
        CharacterModel.gameObject.SetActive(false);
        currentState = eSelectionState.WAITING;
        characterSelection.GetSoundModule().PlayOneShot("Cancel");
    }

    public void Remove()
    {
        if (CurrentEventSystem.currentSelectedGameObject == PresetSelection.gameObject)
        {
            PresetSelection.Index = 0;
            changePersonality = true;
            ChangeCharacterPreset(PresetSelection);
            RefreshSelections();
        }
        else if (CurrentEventSystem.currentSelectedGameObject == HairSelection.gameObject)
        {
            HairSelection.Index = 0;
            ChangeHair(HairSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == GlassesSelection.gameObject)
        {
            GlassesSelection.Index = 0;
            ChangeGlasses(GlassesSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == FacialFeaturesSelection.gameObject)
        {
            FacialFeaturesSelection.Index = 0;
            ChangeFacialFeatures(FacialFeaturesSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == ArmsSelection.gameObject)
        {
            ArmsSelection.Index = 0;
            ChangeArms(ArmsSelection);
        }
    }

    void UpdateRemoveButton()
    {
        if (CurrentEventSystem.currentSelectedGameObject == PresetSelection.gameObject)
        {
            ToggleRemoveInfo(PresetSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == ColorSelection.gameObject)
        {
            ToggleRemoveInfo(false);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == HairSelection.gameObject)
        {
            ToggleRemoveInfo(HairSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == GlassesSelection.gameObject)
        {
            ToggleRemoveInfo(GlassesSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == FacialFeaturesSelection.gameObject)
        {
            ToggleRemoveInfo(FacialFeaturesSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == ArmsSelection.gameObject)
        {
            ToggleRemoveInfo(ArmsSelection);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == PersonalitySelection.gameObject)
        {
            ToggleRemoveInfo(false);
        }
        else if (CurrentEventSystem.currentSelectedGameObject == AssistedSelection.gameObject)
        {
            ToggleRemoveInfo(false);
        }
    }

    void ToggleRemoveInfo(ArrowSelection selection)
    {
        if (selection.Options.Count > 1)
        {
            for (int i = 0; i < RemoveButtonsImage.Length; ++i)
            {
                RemoveButtonsImage[i].color = Color.white;
            }
            RemoveButtonText.color = RemoveTextNormalColor;
            canRemove = true;
        }
        else
        {
            for (int i = 0; i < RemoveButtonsImage.Length; ++i)
            {
                RemoveButtonsImage[i].color = RemoveTextDisabledColor;
            }
            RemoveButtonText.color = RemoveTextDisabledColor;
            canRemove = false;
        }
    }

    void ToggleRemoveInfo(bool isVisible)
    {
        if (isVisible)
        {
            for (int i = 0; i < RemoveButtonsImage.Length; ++i)
            {
                RemoveButtonsImage[i].color = Color.white;
            }
            RemoveButtonText.color = RemoveTextNormalColor;
        }
        else
        {
            for (int i = 0; i < RemoveButtonsImage.Length; ++i)
            {
                RemoveButtonsImage[i].color = RemoveTextDisabledColor;
            }
            RemoveButtonText.color = RemoveTextDisabledColor;
        }
        canRemove = isVisible;
    }

    void ResetModel()
    {
        CharacterPresetTryOn CharacterNewModel = Instantiate(CharacterModelPrefab, CharacterModel.transform.position, CharacterModel.transform.rotation);
        CharacterNewModel.SetIsStatic(true);
        CharacterNewModel.LoadOutfit(currentOutfit);
        CharacterNewModel.SetPersonalityAnim(currentPlayer.GetCurrentPersonality());

        Destroy(CharacterModel.gameObject);
        CharacterModel = CharacterNewModel;
        ChangeAssisted(AssistedSelection);

        PlayerInput.controllers.maps.SetAllMapsEnabled(false);
        PlayerInput.controllers.maps.SetMapsEnabled(true, "Menu", "DefaultControl");
    }

    public void RandomColor()
    {
        /*List<int> indexColorNotTaken = new List<int>();
        for (int i = 0; i < ColorSelection.Options.Count; ++i)
        {
            if (!characterSelection.IsColorAlreadySelected(this, characterSelection.GetPresets().GetColorPreset(ColorSelection.Options[i])))
            {
                indexColorNotTaken.Add(i);
            }
        }

        ColorSelection.Index = indexColorNotTaken[Random.Range(0, indexColorNotTaken.Count)];*/
        ColorSelection.Index = Random.Range(0, ColorSelection.Options.Count);
        ChangeColor(ColorSelection);
    }

    public void RandomCharacter()
    {
        RandomColor();

        HairSelection.Index = Random.Range(0, HairSelection.Options.Count);
        ChangeHair(HairSelection);

        GlassesSelection.Index = Random.Range(0, GlassesSelection.Options.Count);
        ChangeGlasses(GlassesSelection);

        FacialFeaturesSelection.Index = Random.Range(0, FacialFeaturesSelection.Options.Count);
        ChangeFacialFeatures(FacialFeaturesSelection);

        ArmsSelection.Index = Random.Range(0, ArmsSelection.Options.Count);
        ChangeArms(ArmsSelection);

        PersonalitySelection.Index = Random.Range(0, PersonalitySelection.Options.Count);
        ChangePersonality(PersonalitySelection);

        //RefreshSelections();

        characterSelection.GetSoundModule().PlayOneShot("Confirm");
    }

    public void SetCharacterNaked()
    {
        PresetSelection.Index = 0;
        changePersonality = false;
        ChangeCharacterPreset(PresetSelection);

        HairSelection.Index = 0;//HairSelection.Options.Count - 1;
        ChangeHair(HairSelection);

        GlassesSelection.Index = 0;//GlassesSelection.Options.Count - 1;
        ChangeGlasses(GlassesSelection);

        FacialFeaturesSelection.Index = 0;// FacialFeaturesSelection.Options.Count - 1;
        ChangeFacialFeatures(FacialFeaturesSelection);

        ArmsSelection.Index = 0;// ArmsSelection.Options.Count - 1;
        ChangeArms(ArmsSelection);
        RefreshSelections();
    }

    public void RefreshSelections()
    {
        for (int i = 0; i < ColorSelection.Options.Count; ++i)
        {
            if (GameManager.Instance.GetCharactersPresets().GetColorPreset(ColorSelection.Options[i]).IsSame(currentOutfit.BodyColor))
            {
                ColorSelection.Index = i;
                break;
            }
        }

        for (int i = 0; i < HairSelection.Options.Count; ++i)
        {
            if (currentOutfit.HairSprite == null && HairSelection.Options[i].Equals("") || currentOutfit.HairSprite.name == HairSelection.Options[i])
            {
                HairSelection.Index = i;
                break;
            }
        }

        for (int i = 0; i < GlassesSelection.Options.Count; ++i)
        {
            if (currentOutfit.GlassesSprite == null && GlassesSelection.Options[i].Equals("") || currentOutfit.GlassesSprite.name == GlassesSelection.Options[i])
            {
                GlassesSelection.Index = i;
                break;
            }
        }

        for (int i = 0; i < FacialFeaturesSelection.Options.Count; ++i)
        {
            if (currentOutfit.FacialFeaturesSprite == null && FacialFeaturesSelection.Options[i].Equals("") || currentOutfit.FacialFeaturesSprite.name == FacialFeaturesSelection.Options[i])
            {
                FacialFeaturesSelection.Index = i;
                break;
            }
        }

        for (int i = 0; i < ArmsSelection.Options.Count; ++i)
        {
            ArmsPreset armSelection = GameManager.Instance.GetCharactersPresets().GetArmPreset(ArmsSelection.Options[i]);
            if (armSelection.IsSame(currentOutfit.LeftArmSprite != null ? currentOutfit.LeftArmSprite.name : "", currentOutfit.LeftForearmSprite  != null ? currentOutfit.LeftForearmSprite.name : "",
                                    currentOutfit.LeftHandSprite != null ? currentOutfit.LeftHandSprite.name : "", currentOutfit.LeftHandClosedSprite != null ? currentOutfit.LeftHandClosedSprite.name : "", currentOutfit.LeftHandPointSprite != null ? currentOutfit.LeftHandPointSprite.name : "",
                                    currentOutfit.RightArmSprite != null ? currentOutfit.RightArmSprite.name : "", currentOutfit.RightForearmSprite != null ? currentOutfit.RightForearmSprite.name : "",
                                    currentOutfit.RightHandSprite != null ? currentOutfit.RightHandSprite.name : "", currentOutfit.RightHandClosedSprite != null ? currentOutfit.RightHandClosedSprite.name : "", currentOutfit.RightHandPointSprite != null ? currentOutfit.RightHandPointSprite.name : "",
                                    currentOutfit.BackSprite != null ? currentOutfit.BackSprite.name : "", currentOutfit.IsHideHands))
            {
                ArmsSelection.Index = i;
                break;
            }
        }
    }

    void ApplyColorPresetToOutfit(ColorPreset colorPreset)
    {
        currentOutfit.BodyColor = new Color(colorPreset.r, colorPreset.g, colorPreset.b);
    }

    void ApplyArmPresetToOutfit(ArmsPreset arms)
    {
        currentOutfit.LeftArmSprite = characterSelection.GetArmSprite(arms.LeftArmName);
        currentOutfit.LeftForearmSprite = characterSelection.GetForearmSprite(arms.LeftForearmName);
        currentOutfit.LeftHandSprite = characterSelection.GetHandSprite(arms.LeftHandName);
        currentOutfit.LeftHandClosedSprite = characterSelection.GetHandClosedSprite(arms.LeftHandClosedName);
        currentOutfit.LeftHandPointSprite = characterSelection.GetHandPointSprite(arms.LeftHandPointName);

        currentOutfit.RightArmSprite = characterSelection.GetArmSprite(arms.RightArmName);
        currentOutfit.RightForearmSprite = characterSelection.GetForearmSprite(arms.RightForearmName);
        currentOutfit.RightHandSprite = characterSelection.GetHandSprite(arms.RightHandName);
        currentOutfit.RightHandClosedSprite = characterSelection.GetHandClosedSprite(arms.RightHandClosedName);
        currentOutfit.RightHandPointSprite = characterSelection.GetHandPointSprite(arms.RightHandPointName);

        currentOutfit.BackSprite = characterSelection.GetBackSprite(arms.BackName);
        currentOutfit.IsHideHands = arms.IsHideHands;
        CharacterModel.SetHandVisible(CharacterModel.GetLeftHand(), !arms.IsHideHands, false);
        CharacterModel.SetHandVisible(CharacterModel.GetRightHand(), !arms.IsHideHands, false);
        CharacterModel.SetFaceVisible(!currentOutfit.IsHideFace);
    }

    void ApplyPresetToOutfit(CharacterPreset preset, bool changePersonality = true)
    {
        ApplyColorPresetToOutfit(preset.BodyColor);

        currentOutfit.HairSprite = characterSelection.GetHairSprite(preset.HairName);
        if (!string.IsNullOrEmpty(preset.HairName))
        {
            currentOutfit.IsRobot = GameManager.Instance.GetCharactersPresets().IsHeadRobot(currentOutfit.HairSprite.name);
            currentOutfit.IsMask = GameManager.Instance.GetCharactersPresets().IsHeadMask(currentOutfit.HairSprite.name);
            currentOutfit.IsHideFace = GameManager.Instance.GetCharactersPresets().IsHeadHideFace(currentOutfit.HairSprite.name);
        }
        else
        {
            currentOutfit.IsRobot = false;
            currentOutfit.IsMask = false;
            currentOutfit.IsHideFace = false;
        }
        currentOutfit.IsHideFace = preset.IsHideFace;
        CharacterModel.SetFaceVisible(!preset.IsHideFace);

        currentOutfit.GlassesSprite = characterSelection.GetGlassesSprite(preset.GlassesName);
        currentOutfit.FacialFeaturesSprite = characterSelection.GetFacialFeaturesSprite(preset.FacialFeaturesName);

        ApplyArmPresetToOutfit(preset.Arms);

        if (changePersonality)
        {
            ChangePersonality(preset.Personality);
        }
    }
}
