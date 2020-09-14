using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Rewired;

public class TitleScreen : MonoBehaviour {

    public string NextLevelName = "SelectionMenu";
    public string CurrentBundleName = "";
    public CharacterPresetTryOn Model = null;
    public Character.ePersonality ModelPersonality = Character.ePersonality.HAPPY;
    public float TimeBetweenFaceAnims = 10f;
    public string[] AnimNames;
    public GameObject MicrosoftButton = null;
    public GameObject SonyButton = null;
    public GameObject NintendoButton = null;
    public GameObject KeyboardButton = null;

    float timer = 0f;
    bool isActivated = false;

    SoundModule soundModule = null;

    void Awake()
    {
        soundModule = GetComponent<SoundModule>();
        GameManager.Instance.GetMusicManager().PlayMusic(GameManager.Instance.GetMusicManager().DefaultMusic);
    }

    void Start()
    {
        int state = 0;
        switch (ModelPersonality)
        {
            case Character.ePersonality.WEIRDO:
                state = 1;
                break;
            case Character.ePersonality.SCARED:
                state = 2;
                break;
            default:
            case Character.ePersonality.HAPPY:
                state = 3;
                break;
            case Character.ePersonality.GRUMPY:
                state = 4;
                break;
        }
        soundModule.InitEvent("TitleCharacter");
        soundModule.AddParameter("TitleCharacter", "State", state);
        soundModule.PlayEvent("TitleCharacter");

        Model.SetPersonalityAnim(ModelPersonality);
        Model.SetIsStatic(true);
        SetCorrespondingButtons();
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            player.controllers.maps.SetAllMapsEnabled(false);
            player.controllers.maps.SetMapsEnabled(true, "Character", "AssistedControlLeft");
        }
    }

    void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, TimeBetweenFaceAnims);
        if (timer == TimeBetweenFaceAnims)
        {
            Model.SetAnimation(AnimNames[Random.Range(0, AnimNames.Length)]);
            timer = 0f;
        }

        if (isActivated)
            return;
        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if ((NintendoButton.gameObject.activeSelf && playerInput.GetButtonDown(RewiredConsts.Action.Character_GrabLeft) && playerInput.GetButtonDown(RewiredConsts.Action.Character_GrabRight))
                || playerInput.GetButtonDown(RewiredConsts.Action.Character_Pause) || playerInput.GetButtonDown(RewiredConsts.Action.Character_EnterGame))
            {
                SceneManager.LoadScene(NextLevelName, LoadSceneMode.Single);
                isActivated = true;
            }
        }
    }

    void SetCorrespondingButtons()
    {
#if UNITY_SWITCH
            MicrosoftButton.SetActive(false);
            SonyButton.SetActive(false);
            NintendoButton.SetActive(true);
            return;
#endif

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
                    KeyboardButton.SetActive(true);
                    break;
                }
            case ControllerTypesManager.eControllerType.XBOX360:
            case ControllerTypesManager.eControllerType.XBOXONE:
                {
                    MicrosoftButton.SetActive(true);
                    SonyButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    KeyboardButton.SetActive(false);
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
                    break;
                }
            case ControllerTypesManager.eControllerType.BITPRO8:
            case ControllerTypesManager.eControllerType.PS4:
            default:
                {
                    SonyButton.SetActive(true);
                    MicrosoftButton.SetActive(false);
                    NintendoButton.SetActive(false);
                    KeyboardButton.SetActive(false);
                    break;
                }
        }
    }
}
