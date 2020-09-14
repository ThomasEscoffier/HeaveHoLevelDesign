using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using Managers;

public class MainButtonsPanel : MonoBehaviour
{
    public MainMenu MainMenuRoot = null;
    public LevelsButtonsPanel NextPanel = null;
    public GameObject NextPanelFirstSelectedObj = null;
    public GameObject FirstSelectedPanel = null;

    public GameObject[] MicrosoftButtons;
    public GameObject[] SonyButtons;
    public GameObject[] NintendoButtons;
    public GameObject[] KeyboardButtons;

    public MenuScrollingButton ButtonCoop = null;
    public MenuScrollingButton ButtonSolo = null;
    public OptionsMainMenu OptionsMenu = null;

    [System.Serializable]
    public struct ButtonInfo
    {
        public string LocaKey;
        public Color ButtonColor;
        public GameObject PanelToActivate;
        public LevelSelector.eGameMode GameMode;

        public ButtonInfo(string key, Color color, GameObject panel, LevelSelector.eGameMode gameMode)
        {
            LocaKey = key;
            ButtonColor = color;
            PanelToActivate = panel;
            GameMode = gameMode;
        }
    }

    List<MenuScrollingButton> levelBaseButtons;
    EventSystem eventSystem = null;
    SoundModule soundModule = null;

    bool isActivated = true;

    void Awake()
    {
        levelBaseButtons = new List<MenuScrollingButton>(GetComponentsInChildren<MenuScrollingButton>());
        eventSystem = FindObjectOfType<EventSystem>();
        soundModule = GetComponent<SoundModule>();
    }

    void Start()
    {
        SetCorrespondingButtons();
        /*if (!string.IsNullOrEmpty(GameSystems.Instance.GetPreviousSceneName()) && GameSystems.Instance.GetPreviousSceneName().Contains("Selection"))
        {
            FirstSelectedPanel.SetActive(false);
            switch (GameManager.Instance.GetLevelSelector().GetCurrentGameMode())
            {
                default:
                case LevelSelector.eGameMode.COOP:
                    {
                        eventSystem.SetSelectedGameObject(ButtonCoop.gameObject);
                        ButtonCoop.DisplayPanel();
                        ReOrderButtons();
                        StartCooperative(ButtonCoop);
                        NextPanel.SelectWorld(GameManager.Instance.GetLevelSelector().GetCurrentWorld().WorldName);
                        break;
                    }
                case LevelSelector.eGameMode.VERSUS:
                    {
                        eventSystem.SetSelectedGameObject(ButtonVersus.gameObject);
                        ButtonVersus.DisplayPanel();
                        ReOrderButtons();
                        StartVersus(ButtonVersus);
                        NextPanel.SelectWorld(GameManager.Instance.GetLevelSelector().GetCurrentWorld().WorldName);
                        break;
                    }
                case LevelSelector.eGameMode.SOLO:
                    {
                        eventSystem.SetSelectedGameObject(ButtonSolo.gameObject);
                        ButtonSolo.DisplayPanel();
                        ReOrderButtons();
                        StartSolo(ButtonSolo);
                        NextPanel.SelectWorld(GameManager.Instance.GetLevelSelector().GetCurrentWorld().WorldName);
                        break;
                    }
            }
        }
        else
        {*/
            NextPanel.SelectWorld("Tuto");
        //}
    }

    void Update()
    {
        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if (!OptionsMenu.GetIsInOption() && isActivated && playerInput.GetButtonDown(RewiredConsts.Action.Menu_Cancel))
            {
                soundModule.PlayOneShot("Cancel");
                MainMenuRoot.LoadTitleScreen();
            }
        }
    }

    void ActivateNextPanel()
    {
        NextPanel.RefreshLevelSelected();
        NextPanel.PreviousPanelFirstSelectedObj = eventSystem.currentSelectedGameObject;
        NextPanel.transform.parent.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(NextPanelFirstSelectedObj);
        transform.parent.gameObject.SetActive(false);
    }

    public void StartCooperative(MenuScrollingButton button)
    {
        soundModule.PlayOneShot("Confirm");
        GameManager.Instance.GetLevelSelector().SetGameMode(LevelSelector.eGameMode.COOP);
        NextPanel.SetCurrentButton(button.GetComponentInChildren<Image>().color, button.GetComponentInChildren<Localization>().LocalizationKey);
        ActivateNextPanel();
    }

    public void StartSolo(MenuScrollingButton button)
    {
        soundModule.PlayOneShot("Confirm");
        GameManager.Instance.GetLevelSelector().SetGameMode(LevelSelector.eGameMode.SOLO);
        NextPanel.SetCurrentButton(button.GetComponentInChildren<Image>().color, button.GetComponentInChildren<Localization>().LocalizationKey);
        ActivateNextPanel();
    }

    public void ExitGame()
    {
        Application.Quit();
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

    public void SetIsActivated(bool state)
    {
        isActivated = state;
        Navigation nav = new Navigation
        {
            mode = isActivated ? Navigation.Mode.Vertical : Navigation.Mode.None
        };
        for (int i = 0; i < levelBaseButtons.Count; ++i)
        {
            levelBaseButtons[i].GetButton().navigation = nav;
        }
    }

    public void ReOrderButtons()
    {
        List<MenuScrollingButton> buttons = new List<MenuScrollingButton>();
        MenuScrollingButton lastButton = null;

        foreach (MenuScrollingButton button in levelBaseButtons)
        {
            if (eventSystem.currentSelectedGameObject == button.gameObject)
            {
                lastButton = button;
            }
            else
            {
                buttons.Add(button);
            }
        }
        buttons.Add(lastButton);

        for (int i = 0; i < buttons.Count; ++i)
        {
            buttons[i].transform.SetSiblingIndex(i);
        }
    }
}
