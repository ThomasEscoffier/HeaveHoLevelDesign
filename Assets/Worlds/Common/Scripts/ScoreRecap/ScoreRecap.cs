using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Managers;

public class ScoreRecap : MonoBehaviour {

    public Canvas BaseCanvas = null;
    public GameObject[] MicrosoftButtons = null;
    public GameObject[] SonyButtons = null;
    public GameObject[] NintendoButtons = null;
    public GameObject[] KeyboardButtons = null;
    public Fireworks[] Fireworks;

    public GameObject GoToNextWorldObj;

    public Animator AnimLoaderSkipBackNormal;
    public Animator AnimLoaderSkipBackKeyboard;

    public Animator AnimLoaderSkipNextWorldNormal;
    public Animator AnimLoaderSkipNextWorldKeyboard;

    CeremonyRules rules = null;
    bool isSkipped = false;
    bool isGoToNextLevel = false;

    bool isKeyboard = false;

    public Animator animLoaderBack = null;
    public Animator animLoaderNextWorld = null;

    public List<ScoreMetricsDisplay> MetricsPrefabsToDisplay = new List<ScoreMetricsDisplay>();
    ScoreMetricsDisplay currentMetricsToDisplay = null;
    WorldProperties nextWorld;

    void Awake()
    {
        SetActivateMenuInputs(true);
        MetricsManager.PlayerMetrics playerMetrics = GameManager.Instance.GetMetricsManager().GetPlayerWithMostDeaths();

        //Save run score and Unlock next world
        var runMetrics = GameManager.Instance.GetMetricsManager().GetCurrentRunMetrics();
        float score = 0f;
        foreach (var time in runMetrics.LevelTimes)
        {
            score += time;
        }

        var levelSelector = GameManager.Instance.GetLevelSelector();
        if (GameManager.Instance.GetScoreManager().IsBetterRunScore(levelSelector.GetCurrentWorld().WorldName, score,
            levelSelector.GetCurrentRun().NbPlayerMin, levelSelector.GetCurrentGameMode()))
        {
            GameManager.Instance.GetScoreManager().AddNewRunScore(levelSelector.GetCurrentWorld().WorldName, score,
                levelSelector.GetCurrentRun().NbPlayerMin, levelSelector.GetCurrentGameMode());
        }

        var saveManager = GameManager.Instance.GetSaveManager();
        saveManager.Save();
        saveManager.HasToSave = false;
    }

    void Start()
    {
        SetPlatformButton();
        nextWorld = GameManager.Instance.GetLevelSelector().GetNextWorld();
        if (nextWorld == null)
        {
            GoToNextWorldObj.SetActive(false);
        }
    }

    void Update()
    {
        if (isSkipped && isGoToNextLevel)
        {
            GameManager.Instance.GetLevelSelector().StartRun(nextWorld);
            isGoToNextLevel = false;
            return;
        }

        CheckBackMainMenu();
        if (nextWorld != null)
        {
            CheckNextLevel();
        }

        if (currentMetricsToDisplay == null || currentMetricsToDisplay.GetIsFinished())
        {
            if (MetricsPrefabsToDisplay.Count > 0)
            {
                if (currentMetricsToDisplay != null)
                {
                    Destroy(currentMetricsToDisplay.gameObject);
                }
                currentMetricsToDisplay = Instantiate(MetricsPrefabsToDisplay[0], BaseCanvas.transform);
                currentMetricsToDisplay.SetScoreRecap(this);
                currentMetricsToDisplay.SetFireworks(Fireworks);
                MetricsPrefabsToDisplay.RemoveAt(0);
            }
            else
            {
                BackToMainMenu();
            }
        }
    }

    public void SetRules(CeremonyRules ceremonyRules)
    {
        rules = ceremonyRules;
    }

    protected void SetPlatformButton()
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
                    for(int i = 0; i < KeyboardButtons.Length; ++i)
                    {
                        KeyboardButtons[i].SetActive(true);
                    }
                    isKeyboard = true;
                    animLoaderBack = AnimLoaderSkipBackKeyboard;
                    animLoaderNextWorld = AnimLoaderSkipNextWorldKeyboard;
                    break;
                }
            case ControllerTypesManager.eControllerType.XBOX360:
            case ControllerTypesManager.eControllerType.XBOXONE:
                {
                    for (int i = 0; i < MicrosoftButtons.Length; ++i)
                    {
                        MicrosoftButtons[i].SetActive(true);
                    }
                    animLoaderBack = AnimLoaderSkipBackNormal;
                    animLoaderNextWorld = AnimLoaderSkipNextWorldNormal;
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
                        NintendoButtons[i].SetActive(true);
                    }
                    animLoaderBack = AnimLoaderSkipBackNormal;
                    animLoaderNextWorld = AnimLoaderSkipNextWorldNormal;
                    break;
                }
            case ControllerTypesManager.eControllerType.BITPRO8:
            case ControllerTypesManager.eControllerType.PS4:
            default:
                {
                    for (int i = 0; i < SonyButtons.Length; ++i)
                    {
                        SonyButtons[i].SetActive(true);
                    }
                    animLoaderBack = AnimLoaderSkipBackNormal;
                    animLoaderNextWorld = AnimLoaderSkipNextWorldNormal;
                    break;
                }
        }
    }

    void SetActivateMenuInputs(bool activate)
    {
        foreach (Player player in GameManager.Instance.GetPlayers())
        {
            player.GetControls().SetMapEnabled(activate, "Menu");
        }
    }

    protected void CheckBackMainMenu()
    {
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (isKeyboard && player.GetButtonDown(RewiredConsts.Action.Menu_Special)
                || player.GetButtonDown(RewiredConsts.Action.Character_Angry))
            {
                animLoaderBack.SetBool("IsHidden", false);
                if (animLoaderBack.GetCurrentAnimatorStateInfo(0).IsName("Hiding"))
                {
                    animLoaderBack.Play("Showing", 0, 1 - animLoaderBack.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
            }
        }

        if (!isSkipped && animLoaderBack.GetCurrentAnimatorStateInfo(0).IsName("Showned"))
        {
            BackToMainMenu();
        }

        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (isKeyboard && player.GetButtonUp(RewiredConsts.Action.Menu_Special)
                || player.GetButtonUp(RewiredConsts.Action.Character_Angry))
            {
                animLoaderBack.SetBool("IsHidden", true);
                if (animLoaderBack.GetCurrentAnimatorStateInfo(0).IsName("Showing"))
                {
                    animLoaderBack.Play("Hiding", 0, 1 - animLoaderBack.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
            }
        }
    }

    protected void CheckNextLevel()
    {
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (isKeyboard && player.GetButtonDown(RewiredConsts.Action.Menu_Skip)
                || player.GetButtonDown(RewiredConsts.Action.Character_Joy))
            {
                animLoaderNextWorld.SetBool("IsHidden", false);
                if (animLoaderNextWorld.GetCurrentAnimatorStateInfo(0).IsName("Hiding"))
                {
                    animLoaderNextWorld.Play("Showing", 0, 1 - animLoaderNextWorld.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
            }
        }

        if (!isSkipped && animLoaderNextWorld.GetCurrentAnimatorStateInfo(0).IsName("Showned"))
        {
            GoToNextLevel();
        }

        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (isKeyboard && player.GetButtonUp(RewiredConsts.Action.Menu_Skip)
                || player.GetButtonUp(RewiredConsts.Action.Character_Joy))
            {
                animLoaderNextWorld.SetBool("IsHidden", true);
                if (animLoaderNextWorld.GetCurrentAnimatorStateInfo(0).IsName("Showing"))
                {
                    animLoaderNextWorld.Play("Hiding", 0, 1 - animLoaderNextWorld.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
            }
        }
    }

    protected void BackToMainMenu()
    {
        SetActivateMenuInputs(false);
        isSkipped = true;

        GameManager.Instance.GetLevelSelector().LoadMainScene();
    }

    protected void GoToNextLevel()
    {
        SetActivateMenuInputs(false);
        isSkipped = true;
        isGoToNextLevel = true;
    }
}
