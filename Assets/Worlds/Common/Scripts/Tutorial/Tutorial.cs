using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class Tutorial : MonoBehaviour {

    public PlatformTutorial Platform;
    public GameObject MicrosoftButton;
    public GameObject SonyButton;
    public GameObject NintendoButton;
    public GameObject KeyboardButton;
    public Animator MicrosoftController;
    public Animator SonyController;
    public Animator NintendoController;
    public Animator KeyboardController;
    public Text TextMoveArms;
    public Text TextGrab;
    public Text TextSkip;
    public Animator AnimLoaderSkipNormal;
    public Animator AnimLoaderSkipKeyboard;

    public Vector2 PosPlatformDown = Vector2.zero;
    public float TimePlatformMoveDown = 2f;
    public float TimeBeforePlatformMoveDown = 3f;

    protected Animator anim;
    protected Animator currentController;
    protected SoundModule soundModule = null;
    protected Vector2 oldPlatformPos = Vector2.zero;
    protected float timerPlatformMoveDown;
    protected bool isStarted = false;
    protected bool isFinished = false;
    protected int nbStep = 0;
    protected Animator animLoader = null;

    bool hasMovedArms = false;

    //Level to load
    string nextLevel = "";
    string nextWorld = "";
    bool isLoadingSpecificLevel = false;
    bool isLoadingSpecificWorld = false;

    bool isSkipped = false;

    void Awake()
    {
        anim = Camera.main.GetComponent<Animator>();
        oldPlatformPos = Platform.transform.position;
        Platform.SetTutorial(this);
        soundModule = GetComponent<SoundModule>();
    }

    void Update()
    {
        if (isStarted)
        {
            CheckSkip();
            CheckNumberPlayers();
        }

        CheckSteps();
    }

    protected virtual void CheckSteps()
    {
        switch (nbStep)
        {
            case 0: //After first camera move - wait
                {
                    if (!Platform.GetIsCheckingPlayers() && anim.GetCurrentAnimatorStateInfo(0).IsName("WaitingDown"))
                    {
                        currentController.gameObject.SetActive(true);
                        if (currentController != KeyboardController)
                        {
                            TextMoveArms.gameObject.SetActive(true);
                        }
                        SetNextStep();
                    }
                    break;
                }
            case 1: //Wait before moving platform
                {
                    if (!hasMovedArms)
                    {
                        CheckMoveArms();
                    }
                    WaitBeforeMoveDown();
                    break;
                }
            case 2: //Move platform
                {
                    MovePlatformDown();
                    break;
                }
            default:  //FINISH - Charge First Level
                CheckFinishTuto();
                break;
        }
    }

    void CheckMoveArms()
    {
        foreach (Player player in GameManager.Instance.GetPlayers())
        {
            if (Mathf.Abs(ReInput.players.GetPlayer(player.GetControls().GetPlayerId()).GetAxis("LeftArmMovementX")) > Mathf.Epsilon
                || Mathf.Abs(ReInput.players.GetPlayer(player.GetControls().GetPlayerId()).GetAxis("LeftArmMovementY")) > Mathf.Epsilon
                || Mathf.Abs(ReInput.players.GetPlayer(player.GetControls().GetPlayerId()).GetAxis("RightArmMovementX")) > Mathf.Epsilon
                || Mathf.Abs(ReInput.players.GetPlayer(player.GetControls().GetPlayerId()).GetAxis("RightArmMovementY")) > Mathf.Epsilon)
            {
                hasMovedArms = true;
            }
        }
    }

    void WaitBeforeMoveDown()
    {
        if (timerPlatformMoveDown >= TimeBeforePlatformMoveDown && hasMovedArms)
        {
            timerPlatformMoveDown = 0f;
            Platform.gameObject.SetActive(true);
            soundModule.PlayOneShot("TutoBarDown");
            SetNextStep();
        }
        else
        {
            timerPlatformMoveDown = Mathf.Min(timerPlatformMoveDown + Time.deltaTime, TimeBeforePlatformMoveDown);
        }
    }

    protected virtual void MovePlatformDown()
    {
        if (timerPlatformMoveDown >= TimePlatformMoveDown)
        {
            timerPlatformMoveDown = 0f;
            currentController.SetTrigger("Grab");
            if (currentController != KeyboardController)
            {
                TextMoveArms.gameObject.SetActive(false);
                TextGrab.gameObject.SetActive(true);
            }
            Platform.StartCheckingPlayers();
            SetNextStep();
        }
        else
        {
            timerPlatformMoveDown = Mathf.Min(timerPlatformMoveDown + Time.deltaTime, TimePlatformMoveDown);
            Platform.transform.position = Vector2.Lerp(oldPlatformPos, PosPlatformDown, timerPlatformMoveDown / TimePlatformMoveDown);
        }
    }

    protected virtual void CheckFinishTuto()
    {
        if (!isFinished && anim.GetCurrentAnimatorStateInfo(0).IsName("WaitingUp"))
        {
            isFinished = true;
            LoadNextLevel();
        }
    }

    protected void CheckSkip()
    {
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (player.GetButtonDown(RewiredConsts.Action.Menu_Skip))
            {
                animLoader.SetBool("IsHidden", false);
                if (animLoader.GetCurrentAnimatorStateInfo(0).IsName("Hiding"))
                {
                    animLoader.Play("Showing", 0, 1 - animLoader.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
            }
        }

        if (!isSkipped && animLoader.GetCurrentAnimatorStateInfo(0).IsName("Showned"))
        {
            LoadNextLevel();
            isSkipped = true;
        }

        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (player.GetButtonUp(RewiredConsts.Action.Menu_Skip))
            {
                animLoader.SetBool("IsHidden", true);
                if (animLoader.GetCurrentAnimatorStateInfo(0).IsName("Showing"))
                {
                    animLoader.Play("Hiding", 0, 1 - animLoader.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
            }
        }
    }

    protected void CheckNumberPlayers()
    {
        if (GameManager.Instance.GetPlayers().Count <= 0)
        {
            GameManager.Instance.GetLevelSelector().LoadSelectionMenuScene();
        }
    }

    protected void SetAnimController()
    {
        if (GameManager.Instance.GetPlayers().Count == 0)
            return;

        ControllerTypesManager.eControllerType type = ControllerTypesManager.eControllerType.NONE;
        Rewired.Player player = ReInput.players.GetPlayer(GameManager.Instance.GetPlayers()[0].GetControls().GetPlayerId());
        if (player.controllers.joystickCount > 0)
        {
            type = GameManager.Instance.GetControllerTypesManager().GetControllerTypeFromJoystick(player.controllers.Joysticks[0] as Joystick);
        }
        else
        {
            type = ControllerTypesManager.eControllerType.KEYBOARD;
        }

        switch (type)
        {
            case ControllerTypesManager.eControllerType.KEYBOARD:
                {
                    currentController = KeyboardController;
                    KeyboardButton.SetActive(true);
                    animLoader = AnimLoaderSkipKeyboard;
                    break;
                }
            case ControllerTypesManager.eControllerType.XBOX360:
            case ControllerTypesManager.eControllerType.XBOXONE:
                {
                    currentController = MicrosoftController;
                    MicrosoftButton.SetActive(true);
                    animLoader = AnimLoaderSkipNormal;
                    break;
                }
            case ControllerTypesManager.eControllerType.JOYCON_DUAL:
            case ControllerTypesManager.eControllerType.JOYCON_LEFT:
            case ControllerTypesManager.eControllerType.JOYCON_RIGHT:
            case ControllerTypesManager.eControllerType.JOYCON_PRO:
            case ControllerTypesManager.eControllerType.JOYCON_HANDHELD:
                {
                    currentController = NintendoController;
                    NintendoButton.SetActive(true);
                    animLoader = AnimLoaderSkipNormal;
                    break;
                }
            case ControllerTypesManager.eControllerType.BITPRO8:
            case ControllerTypesManager.eControllerType.PS4:
            default:
                {
                    currentController = SonyController;
                    SonyButton.SetActive(true);
                    animLoader = AnimLoaderSkipNormal;
                    break;
                }
        }
    }

    public void SetNextLevel(string nextLvlName)
    {
        nextLevel = nextLvlName;
        isLoadingSpecificLevel = true;
    }

    public void SetNextWorld(string nextWorldName)
    {
        nextWorld = nextWorldName;
        isLoadingSpecificWorld = true;
    }

    public virtual void StartTuto()
    {
        isStarted = true;
        TextSkip.gameObject.SetActive(true);
        anim.SetTrigger("MoveCharDown");
        SetAnimController();
    }

    public void MoveCharactersUp()
    {
        anim.SetTrigger("MoveCharUp");
        soundModule.PlayOneShot("TutoBarUp");
    }

    protected void LoadNextLevel()
    {
        //Activate Character inputs to allow players to enter the game
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            player.controllers.maps.SetAllMapsEnabled(false);
            player.controllers.maps.SetMapsEnabled(true, "Character", "AssistedControlLeft");
            if (GameManager.Instance.GetIsDebugModeOn())
            {
                player.controllers.maps.SetMapsEnabled(true, "Cheats");
            }
        }

        if (isLoadingSpecificLevel)
        {
            GameManager.Instance.GetLevelSelector().LoadSpecificLevel(nextLevel);
        }
        else if (isLoadingSpecificWorld)
        {
            GameManager.Instance.GetLevelSelector().StartRun(nextWorld);
        }
        else
        {
            GameManager.Instance.GetLevelSelector().StartRun();
        }
    }

    public void SetNextStep()
    {
        nbStep++;
    }
}
