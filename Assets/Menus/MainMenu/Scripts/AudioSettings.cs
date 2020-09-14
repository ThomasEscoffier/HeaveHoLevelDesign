using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using Rewired.Integration.UnityUI;

public class AudioSettings : MonoBehaviour
{
    public GameObject MicrosoftButton, MicrosoftResetButton;
    public GameObject SonyButton, SonyResetButton;
    public GameObject NintendoButton,NintendoResetButton;
    public GameObject KeyboardButton,KeyboardResetButton;

    public Slider SFXSlider = null;
    public Slider MusicSlider = null;
    public Slider VoicesSlider = null;
    public ArrowSelection LanguageSelection = null;

    public Color ResetTextNormalColor = Color.white;
    public Color ResetTextDisabledColor = Color.grey;
    public Image[] ResetImages = null;
    public Text ResetText = null;

    public EventSystem eventSystem = null;
    public RewiredStandaloneInputModule input;

    FMOD.Studio.Bus musicBus;
    FMOD.Studio.Bus sfxBus;
    FMOD.Studio.Bus voicesBus;

    GameObject currentObjSelected = null;

    void Awake()
    {
        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MASTER/MUSIC");
        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/MASTER/SFX");
        voicesBus = FMODUnity.RuntimeManager.GetBus("bus:/MASTER/VOIX");

        if (MicrosoftButton != null && SonyButton != null && NintendoButton != null)
        {
            SetCorrespondingButtons();
        }
    }

    void Start()
    {
        SFXSlider.value = GameManager.Instance.GetSaveManager().GameSaveData.AudioOptions.SFX;
        sfxBus.setVolume(SFXSlider.value);
        MusicSlider.value = GameManager.Instance.GetSaveManager().GameSaveData.AudioOptions.Music;
        musicBus.setVolume(MusicSlider.value);
        VoicesSlider.value = GameManager.Instance.GetSaveManager().GameSaveData.AudioOptions.Voices;
        voicesBus.setVolume(VoicesSlider.value);

        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
        if (input == null)
        {
            input = FindObjectOfType<RewiredStandaloneInputModule>();
        }
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject != currentObjSelected)
        {
            currentObjSelected = eventSystem.currentSelectedGameObject;
            HandleNewSelectedObject();
        }
        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if (playerInput.GetButtonUp(RewiredConsts.Action.Menu_Special))
            {
                ResetDefault();
            }
        }
    }

    void HandleNewSelectedObject()
    {
        if (currentObjSelected.name == VoicesSlider.name || currentObjSelected.name == SFXSlider.name || currentObjSelected.name == MusicSlider.name)
        {
            for (int i = 0; i < ResetImages.Length; ++i)
            {
                ResetImages[i].color = Color.white;
            }
            ResetText.color = ResetTextNormalColor;
        }
        else
        {
            for (int i = 0; i < ResetImages.Length; ++i)
            {
                ResetImages[i].color = ResetTextDisabledColor;
            }
            ResetText.color = ResetTextDisabledColor;
        }
    }

    void ResetDefault()
    {
        if (currentObjSelected.name == VoicesSlider.name)
        {
            VoicesSlider.value = 1f;
            VoicesVolumeLevel(VoicesSlider);
        }
        else if (currentObjSelected.name == SFXSlider.name)
        {
            SFXSlider.value = 1f;
            SFXVolumeLevel(SFXSlider);
        }
        else if (currentObjSelected.name == MusicSlider.name)
        {
            MusicSlider.value = 1f;
            MusicVolumeLevel(MusicSlider);
        }
    }

    public void MusicVolumeLevel(Slider slider)
    {
        musicBus.setVolume(slider.value);
        GameManager.Instance.GetSaveManager().GameSaveData.AudioOptions.Music = slider.value;
        GameManager.Instance.GetSaveManager().Save();
    }

    public void SFXVolumeLevel(Slider slider)
    {
        sfxBus.setVolume(slider.value);
        GameManager.Instance.GetSaveManager().GameSaveData.AudioOptions.SFX = slider.value;
        GameManager.Instance.GetSaveManager().Save();
    }

    public void VoicesVolumeLevel(Slider slider)
    {
        voicesBus.setVolume(slider.value);
        GameManager.Instance.GetSaveManager().GameSaveData.AudioOptions.Voices = slider.value;
        GameManager.Instance.GetSaveManager().Save();
    }

    void SetCorrespondingButtons()
    {
        if (GameManager.Instance.GetPlayers().Count == 0)
            return;

        ControllerTypesManager.eControllerType type = ControllerTypesManager.eControllerType.KEYBOARD;
        Rewired.Player player = ReInput.players.GetPlayer(GameManager.Instance.GetPlayers()[0].GetControls().GetPlayerId());
        if (player.controllers.joystickCount > 0)
        {
            type = GameManager.Instance.GetControllerTypesManager().GetControllerTypeFromJoystick(player.controllers.Joysticks[0] as Joystick);
        }
        switch (type)
        {
            case ControllerTypesManager.eControllerType.KEYBOARD:
                {
                    MicrosoftButton.SetActive(false);
                    SonyButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    KeyboardButton.SetActive(true);
                    MicrosoftResetButton.SetActive(false);
                    SonyResetButton.SetActive(false);
                    NintendoResetButton.SetActive(false);
                    KeyboardResetButton.SetActive(true);
                    break;
                }
            case ControllerTypesManager.eControllerType.XBOX360:
            case ControllerTypesManager.eControllerType.XBOXONE:
                {
                    MicrosoftButton.SetActive(true);
                    SonyButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    KeyboardButton.SetActive(false);
                    MicrosoftResetButton.SetActive(true);
                    SonyResetButton.SetActive(false);
                    NintendoResetButton.SetActive(false);
                    KeyboardResetButton.SetActive(false);
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
                    KeyboardButton.SetActive(false);
                    MicrosoftResetButton.SetActive(false);
                    SonyResetButton.SetActive(false);
                    NintendoResetButton.SetActive(true);
                    KeyboardResetButton.SetActive(false);
                    break;
                }
            case ControllerTypesManager.eControllerType.BITPRO8:
            case ControllerTypesManager.eControllerType.PS4:
            default:
                {
                    SonyButton.SetActive(true);
                    MicrosoftButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    SonyResetButton.SetActive(true);
                    KeyboardButton.SetActive(false);
                    MicrosoftResetButton.SetActive(false);
                    NintendoResetButton.SetActive(false);
                    KeyboardResetButton.SetActive(false);
                    break;
                }
        }
    }

    public void OnChangeLanguage(ArrowSelection selection)
    {
//
    }

    void OnDisable()
    {
        for (int i = 0; i < ResetImages.Length; ++i)
        {
            ResetImages[i].color = Color.white;
        }
        ResetText.color = ResetTextNormalColor;
    }
}
