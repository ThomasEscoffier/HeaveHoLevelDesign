#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System.Collections;
#endif
using UnityEngine;
using LetterboxCamera;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

    Animator anim = null;
    SoundModule sound = null;

    public GameManager GameManagerPrefab = null;
    public GameObject LogoEmpty = null;
    public GameObject LogoNormal = null;
    public GameObject Mascot = null;

    public string NextScene = "";

    public float TimeBeforeChangeScene = 3f;
    float timer = 0f;
    bool isTimer = false;

    FlashEffect flash = null;

    bool isSplashscreenLaunched = false;
    ForceCameraRatio boxCam = null;

    void Awake ()
    {
        anim = GetComponent<Animator>();
        sound = GetComponent<SoundModule>();
        flash = FindObjectOfType<FlashEffect>();
        boxCam = FindObjectOfType<ForceCameraRatio>();
        if (GameManager.Instance == null)
        {
            Instantiate(GameManagerPrefab);
        }
    }

    void Start()
    {
        SaveManager.NewSaveData GameSaveData = GameManager.Instance.GetSaveManager().GameSaveData;

        //Init graphic resolution fullscreen
        FullScreenMode fullScreenMode = GraphicSettings.GetFullScreenFromName(GameSaveData.GraphicOptions.FullScreenMode);
        if (!Screen.currentResolution.width.Equals(GameSaveData.GraphicOptions.ResolutionWidth) || !Screen.currentResolution.height.Equals(GameSaveData.GraphicOptions.ResolutionHeight)
            || Screen.fullScreen != GameSaveData.GraphicOptions.IsFullScreen)
        {
            if (GraphicSettings.IsResolutionAvailable(GameSaveData.GraphicOptions.ResolutionWidth, GameSaveData.GraphicOptions.ResolutionHeight))
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                Screen.SetResolution(GameSaveData.GraphicOptions.ResolutionWidth, GameSaveData.GraphicOptions.ResolutionHeight, GameSaveData.GraphicOptions.IsFullScreen && fullScreenMode != FullScreenMode.FullScreenWindow);
#else
                Screen.SetResolution(GameSaveData.GraphicOptions.ResolutionWidth, GameSaveData.GraphicOptions.ResolutionHeight, GameSaveData.GraphicOptions.IsFullScreen);
#endif
            }
        }
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            StartCoroutine(SetFrameless());
        }
        else
        {
            StartCoroutine(SetFrame());
        }
#endif
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    IEnumerator SetFrameless()
    {
        yield return new WaitForSeconds(0.1f);
        BorderlessWindow.SetFramelessWindow();
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, Camera.main.pixelWidth, Camera.main.pixelHeight);
    }

    IEnumerator SetFrame()
    {
        yield return new WaitForSeconds(0.1f);
        BorderlessWindow.SetFramedWindow();
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, boxCam.letterBoxCamera.pixelWidth - 8, boxCam.letterBoxCamera.pixelHeight - 40);
    }
#endif

    void Update ()
    {
        if (isTimer)
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeChangeScene);

            if (!isSplashscreenLaunched && timer == TimeBeforeChangeScene)
            {
                LoadTitleScreen();
            }
        }
	}

    public void ChangeLogo()
    {
        LogoNormal.SetActive(true);
        LogoEmpty.SetActive(false);
        Mascot.SetActive(false);

        flash.Launch();

        isTimer = true;
    }

    public void PlayLogoSound()
    {
        sound.PlayOneShot("Logo");
    }

    public void LoadTitleScreen()
    {
        SceneManager.LoadScene(NextScene, LoadSceneMode.Single);
        isSplashscreenLaunched = true;
    }
}
