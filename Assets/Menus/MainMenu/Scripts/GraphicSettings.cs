using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;
using Rewired.Integration.UnityUI;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System.Collections;
#endif
using LetterboxCamera;

public class GraphicSettings : MonoBehaviour
{
    public ArrowSelection ResolutionSelection = null;
    public ArrowSelection FullScreenModeSelection = null;
    public ArrowSelection IsFullScreenSelection = null;
    //public ArrowSelection MonitorSelection = null;

    public EventSystem eventSystem = null;
    public RewiredStandaloneInputModule input;

    Resolution[] availableResolutions;
    Resolution currentResolution;
    Resolution defaultResolution;
    FullScreenMode fullScreenMode;
    bool isFullScreen;
    //int monitor = 0;
    GameObject currentObjSelected = null;
    Camera cam = null;
    ForceCameraRatio boxCam = null;

    public const string exclusiveFullScreen = "ExclusiveFullScreen";
    public const string fullScreenWindow = "FullScreenWindow";
    public const string exclusiveFullScreenLocaKey = "Options_Fullscreen";
    public const string fullScreenWindowLocaKey = "Options_Borderless";
    public static readonly string[] UnsupportedResolutions = {};

    void Awake()
    {
        cam = Camera.main;
        boxCam = FindObjectOfType<ForceCameraRatio>();

        //monitor = GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.Monitor;
        defaultResolution.width = GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.DefaultResolutionWidth;
        defaultResolution.height = GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.DefaultResolutionHeight;
        currentResolution.width = GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.ResolutionWidth;
        currentResolution.height = GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.ResolutionHeight;
        fullScreenMode = GetFullScreenFromName(GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.FullScreenMode);
        isFullScreen = GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.IsFullScreen;

#if !UNITY_STANDALONE_WIN || UNITY_EDITOR
        FullScreenModeSelection.gameObject.SetActive(false);
#endif
    }

    void Start()
    {
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
        if (input == null)
        {
            input = FindObjectOfType<RewiredStandaloneInputModule>();
        }

        ResolutionSelection.SetCurrentEventSystem(eventSystem, input);
        InitializeAvailableResolutions();
        IsFullScreenSelection.SetCurrentEventSystem(eventSystem, input);
        InitializeIsFullScreen();
        FullScreenModeSelection.SetCurrentEventSystem(eventSystem, input);
        InitializeFullScreenMode();
        //MonitorSelection.SetCurrentEventSystem(eventSystem, input);
        //InitializeAvailableMonitors();
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject != currentObjSelected)
        {
            currentObjSelected = eventSystem.currentSelectedGameObject;
        }
        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if (playerInput.GetButtonUp(RewiredConsts.Action.Menu_Special))
            {
                ResetDefault();
            }
        }
    }

    static bool IsSupportedResolution(Resolution resolution)
    {
        foreach (string unsupportedResolution in UnsupportedResolutions)
        {
            if (unsupportedResolution.Contains(resolution.width.ToString()) && unsupportedResolution.Contains(resolution.height.ToString()))
            {
                return false;
            }
        }
        return true;
    }

    void ResetDefault()
    {
        if (currentObjSelected.name == ResolutionSelection.name)
        {
            if (!ResolutionSelection.Options[ResolutionSelection.Index].Contains(currentResolution.width.ToString())
                || !ResolutionSelection.Options[ResolutionSelection.Index].Contains(currentResolution.height.ToString()))
            {
                ResolutionSelection.Index = ResolutionSelection.Options.FindIndex(i => i.Contains(currentResolution.width.ToString()) && i.Contains(currentResolution.height.ToString()));
                OnChangeResolution(ResolutionSelection);
            }
        }
        else if (currentObjSelected.name == IsFullScreenSelection.name)
        {
            if (!IsFullScreenSelection.Options[IsFullScreenSelection.Index].Equals("Options_FullscreenOn"))
            {
                IsFullScreenSelection.Index = IsFullScreenSelection.Options.FindIndex(i => i.Contains("Options_FullscreenOn"));
                OnChangeIsFullScreen(IsFullScreenSelection);
            }
        }
        else if (currentObjSelected.name == FullScreenModeSelection.name)
        {
            if (!FullScreenModeSelection.Options[FullScreenModeSelection.Index].Equals("Options_Fullscreen"))
            {
                FullScreenModeSelection.Index = FullScreenModeSelection.Options.FindIndex(i => i.Contains("Options_Fullscreen"));
                OnChangeFullScreenMode(FullScreenModeSelection);
            }
        }
        /*else if (currentObjSelected.name == MonitorSelection.name)
        {
            if (MonitorSelection.Index != 0)
            {
                MonitorSelection.Index = 0;
                OnChangeMonitor(MonitorSelection);
            }
        }*/
    }

    void InitializeAvailableResolutions()
    {
        availableResolutions = Screen.resolutions;
        for (int i = 0; i < availableResolutions.Length; ++i)
        {
            if (IsSupportedResolution(availableResolutions[i]))
            {
                ResolutionSelection.AddOption(availableResolutions[i].width.ToString() + "x" + availableResolutions[i].height.ToString());
                if (currentResolution.width == availableResolutions[i].width && currentResolution.height == availableResolutions[i].height)
                {
                    ResolutionSelection.Index = ResolutionSelection.Options.FindIndex(j => j.Contains(currentResolution.width.ToString()) && j.Contains(currentResolution.height.ToString()));
                }
            }
        }
        ResolutionSelection.RefreshLocalization();
    }

    void InitializeIsFullScreen()
    {
        if (isFullScreen)
        {
            IsFullScreenSelection.Index = IsFullScreenSelection.Options.FindIndex(i => i.Equals("Options_FullscreenOn"));
        }
        else
        {
            IsFullScreenSelection.Index = IsFullScreenSelection.Options.FindIndex(i => i.Equals("Options_FullsreenOff"));
        }
        IsFullScreenSelection.RefreshLocalization();
    }

    void InitializeFullScreenMode()
    {
        FullScreenModeSelection.Index = FullScreenModeSelection.Options.FindIndex(i => i.Equals(GetKeyLocaNameFromFullScreenEnum(fullScreenMode)));
        FullScreenModeSelection.RefreshLocalization();
    }

    /*void InitializeAvailableMonitors()
    {
        for (int i = 0; i < Display.displays.Length; ++i)
        {
            MonitorSelection.AddOption((i + 1).ToString());
            if (monitor == i)
            {
                MonitorSelection.Index = i;
            }
        }
        MonitorSelection.RefreshLocalization();
    }*/

    public static string GetNameFromFullScreenEnum(FullScreenMode mode)
    {
        switch (mode)
        {
            default:
            case FullScreenMode.ExclusiveFullScreen:
                return exclusiveFullScreen;
            case FullScreenMode.FullScreenWindow:
                return fullScreenWindow;
        }
    }

    public static string GetKeyLocaNameFromFullScreenEnum(FullScreenMode mode)
    {
        switch (mode)
        {
            default:
            case FullScreenMode.ExclusiveFullScreen:
                return exclusiveFullScreenLocaKey;
            case FullScreenMode.FullScreenWindow:
                return fullScreenWindowLocaKey;
        }
    }

    public static FullScreenMode GetFullScreenEnumFromKeyLocaName(string name)
    {
        switch (name)
        {
            default:
            case exclusiveFullScreenLocaKey:
                return FullScreenMode.ExclusiveFullScreen;
            case fullScreenWindowLocaKey:
                return FullScreenMode.FullScreenWindow;
        }
    }

    public static FullScreenMode GetFullScreenFromName(string name)
    {
        switch (name)
        {
            default:
            case exclusiveFullScreen:
                return FullScreenMode.ExclusiveFullScreen;
            case fullScreenWindow:
                return FullScreenMode.FullScreenWindow;
        }
    }

    public void OnChangeResolution(ArrowSelection selection)
    {
        RefreshGraphics();
    }

    public void OnChangeIsFullScreen(ArrowSelection selection)
    {
        RefreshGraphics();
    }

    public void OnChangeFullScreenMode(ArrowSelection selection)
    {
        RefreshGraphics();
    }

    public void RefreshGraphics()
    {
        //Resolution
        string[] numbers = ResolutionSelection.Options[ResolutionSelection.Index].Split(new char[] { 'x' });
        currentResolution.width = int.Parse(numbers[0]);
        currentResolution.height = int.Parse(numbers[1]);

        //Fullscreen
        bool isOn = IsFullScreenSelection.Options[IsFullScreenSelection.Index].Equals("Options_FullscreenOn");

        Screen.SetResolution(currentResolution.width, currentResolution.height, isOn);

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        //Mode
        FullScreenMode mode = GetFullScreenEnumFromKeyLocaName(FullScreenModeSelection.Options[FullScreenModeSelection.Index]);

        if (mode == FullScreenMode.ExclusiveFullScreen)
        {
            if (!isOn)
            {
                StartCoroutine(SetFrame());
            }
        }
        else if (mode == FullScreenMode.FullScreenWindow)
        {
            Screen.fullScreen = false;
            StartCoroutine(SetFrameless());
        }
        else
        {
            StartCoroutine(SetFrame());
        }
        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.FullScreenMode = GetNameFromFullScreenEnum(fullScreenMode);
#else
        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.FullScreenMode = GetNameFromFullScreenEnum(FullScreenMode.ExclusiveFullScreen);
#endif

        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.ResolutionWidth = currentResolution.width;
        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.ResolutionHeight = currentResolution.height;
        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.IsFullScreen = isOn;
        GameManager.Instance.GetSaveManager().Save();
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    IEnumerator SetFrameless()
    {
        yield return new WaitForSeconds(0.1f);
        BorderlessWindow.SetFramelessWindow();
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, cam.pixelWidth - 8, cam.pixelHeight - 40);
    }

    IEnumerator SetFrame()
    {
        yield return new WaitForSeconds(0.1f);
        BorderlessWindow.SetFramedWindow();
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, boxCam.letterBoxCamera.pixelWidth - 8, boxCam.letterBoxCamera.pixelHeight - 40);
    }
#endif

    /*public void OnChangeMonitor(ArrowSelection selection)
    {
        Display.displays[selection.Index].Activate();
        InitializeAvailableResolutions();
        FixUnsupportedResolution(Screen.currentResolution);
        currentResolution = Screen.currentResolution;
        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.Monitor = selection.Index;
        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.ResolutionWidth = currentResolution.width;
        GameManager.Instance.GetSaveManager().GameSaveData.GraphicOptions.ResolutionHeight = currentResolution.height;
        GameManager.Instance.GetSaveManager().Save();
    }*/

    public static bool FixUnsupportedResolution(Resolution resolution)
    {
        if (IsSupportedResolution(resolution))
        {
            return true;
        }

        Resolution[] allRes = Screen.resolutions;
        allRes = Screen.resolutions;
        Resolution newRes;
        for (int i = 0; i < allRes.Length; ++i)
        {
            if (allRes[i].width == resolution.width && allRes[i].height == resolution.height && i > 0)
            {
                newRes = allRes[i - 1];
                if (FixUnsupportedResolution(newRes))
                {
                    Screen.SetResolution(newRes.width, newRes.height, FullScreenMode.ExclusiveFullScreen);
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsResolutionAvailable(int width, int height)
    {
        foreach (Resolution res in Screen.resolutions)
        {
            if (res.width == width && res.height == height)
            {
                return true;
            }
        }
        return false;
    }
}
