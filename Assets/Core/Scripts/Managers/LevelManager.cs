using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour {

    public GameManager GameManagerPrefab = null;
    public ALevelRules LevelRules = null;

    public Vector2 Gravity = new Vector2(0, -15f);
    public bool IsNotUsingUpLimit = true;
    public bool IsNotUsingRightLimit = false;
    public bool IsNotUsingLeftLimit = false;
    public bool IsNotUsingDownLimit = false;
    [Tooltip("Kill characters outside screen even if they are grabbing something")]
    public bool KillGrabbingCharacters = false;
    public float TimeOutsideCameraBeforeDeath = 3f;

    public float TimeBeforeNextLevel = 2f;
    float timerBeforeNextLevel = 0f;

    public float TimeBeforeLaunchMusic = 0.5f;
    float timerBeforeLaunchMusic = 0f;
    bool isMusicLaunched = false;

    public bool NextLevelVote = false;
    [Tooltip("Keep blank if random next level")]
    public string NextLevel = "";

    Victory victoryTrigger = null;
    SoundModule soundModule = null;
    FlashEffect flash = null;
    static Camera cachedMainCamera = null;

    PauseMenu pauseMenu = null;

    bool isLevelOver = false;

    public UnityEvent OnLevelFinished = new UnityEvent();

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instantiate(GameManagerPrefab);
        }

        soundModule = GetComponent<SoundModule>();
        flash = GetComponent<FlashEffect>();
        cachedMainCamera = Camera.main;
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

	void Start()
    {
        victoryTrigger = FindObjectOfType<Victory>();

        Physics2D.gravity = Gravity;

        if (LevelRules != null)
        {
            LevelRules.OnStart();
        }

        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.VERSUS || GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.SOLO)
        {
            RespawnableItem[] tokens = FindObjectsOfType<RespawnableItem>();
            for (int i = 0; i < tokens.Length; ++i)
            {
                if (tokens[i].Type == RespawnableItem.ItemType.KEY)
                {
                    tokens[i].gameObject.SetActive(false);
                }
            }
        }

        //isMusicLaunched = !GameManager.Instance.GetMusicManager().GetIsDynamic();
    }

    void Update()
    {
        if (LevelRules == null)
            return;

        if (LevelRules.CheckIsFinished())
        {
            if (!isLevelOver)
            {
                LevelRules.OnFinish();
                isLevelOver = true;
                OnLevelFinished.Invoke();
            }
            else
            {
                timerBeforeNextLevel = Mathf.Min(timerBeforeNextLevel + Time.deltaTime, TimeBeforeNextLevel);
                if (timerBeforeNextLevel >= TimeBeforeNextLevel && GameManager.Instance.GetPlayers().Count > 0)
                {
                    soundModule.StopAllPlayingEvents();
                    timerBeforeNextLevel = 0f;
                    if (NextLevelVote)
                    {
                        GameManager.Instance.GetLevelSelector().LoadMainScene();
                    }
                    else
                    {
                        if (GameManager.Instance.GetLevelSelector().GetIsRandomMode())
                        {
                            GameManager.Instance.GetLevelSelector().SelectRandomNextLevel(NextLevel);
                        }
                        else
                        {
                            GameManager.Instance.GetLevelSelector().SelectNextLevel(NextLevel);
                        }
                    }
                }
            }
        }
        else
        {
            LevelRules.OnUpdate();
            if (!isMusicLaunched)
            {
                timerBeforeLaunchMusic = Mathf.Min(timerBeforeLaunchMusic + Time.deltaTime, TimeBeforeLaunchMusic);
                if (timerBeforeLaunchMusic == TimeBeforeLaunchMusic && !pauseMenu.GetIsLoading())
                {
                    isMusicLaunched = true;
                    timerBeforeLaunchMusic = 0f;
                    LevelProperties currentLevel = GameManager.Instance.GetLevelSelector().GetCurrentLevel(); 
                    if (currentLevel != null)
                    {
                        //GameManager.Instance.GetMusicManager().PlayWorldMusic(currentLevel.Music, currentLevel.Stinger, GameManager.Instance.GetLevelSelector().GetCurrentStep() - 1);
                        if (!currentLevel.IsUsingInteractiveMusic)
                        {
                            GameManager.Instance.GetMusicManager().PlayMusic(currentLevel.Music);
                            GameManager.Instance.GetAmbianceManager().StartAmbiance(currentLevel.Ambiance);
                            GameManager.Instance.GetAmbianceManager().StartSnapshot(currentLevel.Snapshot);
                        }
                        else
                        {
                            GameManager.Instance.GetMusicManager().PlayWorldMusic(currentLevel.Music, currentLevel.StingerOnesShot, currentLevel.Stinger, GameManager.Instance.GetLevelSelector().GetCurrentStep() - 1);
                            GameManager.Instance.GetAmbianceManager().StartWorldAmbiance(currentLevel.Ambiance);
                            GameManager.Instance.GetAmbianceManager().StartWorldSnapshot(currentLevel.Snapshot);
                        }
                    }
                }
            }
        }
    }
    public Camera GetCurrentCamera()
    {
        return cachedMainCamera;
    }

    public bool GetIsLevelFinished()
    {
        return isLevelOver;
    }

    public FlashEffect GetFlashEffect()
    {
        return flash;
    }

    public SoundModule GetSoundModule()
    {
        return soundModule;
    }

    public static Vector2 GetPosOnCamera(Vector3 posInGame)
    {
        return cachedMainCamera.WorldToScreenPoint(posInGame);
    }

    public static bool IsObjectInsideCamera(SpriteRenderer sprite)
    {
        if (sprite == null)
            return false;
        return sprite.isVisible;
    }

    public static bool IsObjectInsideCamera(Vector3 pos)
    {
        if (cachedMainCamera == null)
            return true;
        Vector2 pointOnCamera = cachedMainCamera.WorldToScreenPoint(pos);
        return (pointOnCamera.x > 0 && pointOnCamera.x < Screen.width && pointOnCamera.y > 0 && pointOnCamera.y < Screen.height);
    }

    public static bool IsObjectInsideCamera(GameObject obj)
    {
        if (cachedMainCamera == null)
            return true;
        Vector2 pointOnCamera = cachedMainCamera.WorldToScreenPoint(obj.transform.position);
        return (pointOnCamera.x > 0 && pointOnCamera.x < Screen.width && pointOnCamera.y > 0 && pointOnCamera.y < Screen.height);
    }

    public static bool IsObjectInsideCamera(GameObject obj, float offset)
    {
        if (cachedMainCamera == null)
            return true;

        Vector2 pointOnCamera = cachedMainCamera.WorldToScreenPoint(obj.transform.position);
        Vector2 downLeftCorner = cachedMainCamera.ScreenToWorldPoint(new Vector2(0f, 0f));
        Vector2 upRightCorner = cachedMainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        Vector2 pointDownLeft = cachedMainCamera.WorldToScreenPoint(downLeftCorner + new Vector2(-offset, -offset));
        Vector2 pointUpRight = cachedMainCamera.WorldToScreenPoint(upRightCorner + new Vector2(offset, offset));
        return (pointOnCamera.x > pointDownLeft.x && pointOnCamera.x < pointUpRight.x && pointOnCamera.y > pointDownLeft.y && pointOnCamera.y < pointUpRight.y);
    }

    public bool IsObjectInsideCameraNotStatic(GameObject obj, float offset)
    {
        if (cachedMainCamera == null)
        {
            cachedMainCamera = Camera.main;
            if (cachedMainCamera == null)
                return true;
        }
        Vector2 pointOnCamera = cachedMainCamera.WorldToScreenPoint(obj.transform.position);
        Vector2 downLeftCorner = cachedMainCamera.ScreenToWorldPoint(new Vector2(0f, 0f));
        Vector2 upRightCorner = cachedMainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        Vector2 pointDownLeft = cachedMainCamera.WorldToScreenPoint(downLeftCorner + new Vector2(-offset, -offset));
        Vector2 pointUpRight = cachedMainCamera.WorldToScreenPoint(upRightCorner + new Vector2(offset, offset));
        return (pointOnCamera.x > pointDownLeft.x && pointOnCamera.x < pointUpRight.x && pointOnCamera.y > pointDownLeft.y && pointOnCamera.y < pointUpRight.y);
    }

    public bool GetIsOutsideLimit(GameObject obj)
    {
        bool isOutsideLimitY = false;
        bool isOutsideLimitX = false;

        Vector2 resetpointOnCamera = cachedMainCamera.WorldToScreenPoint(obj.transform.position);

        if (IsNotUsingUpLimit && IsNotUsingDownLimit)
        {
            isOutsideLimitY = false;
        }
        else if (IsNotUsingUpLimit)
        {
            isOutsideLimitY = resetpointOnCamera.y < 0;
        }
        else if (IsNotUsingDownLimit)
        {
            isOutsideLimitY = resetpointOnCamera.y > Screen.height;
        }
        else
        {
            isOutsideLimitY = resetpointOnCamera.y < 0 || resetpointOnCamera.y > Screen.height;
        }

        if (IsNotUsingLeftLimit && IsNotUsingRightLimit)
        {
            isOutsideLimitX = false;
        }
        else if (IsNotUsingRightLimit)
        {
            isOutsideLimitX = resetpointOnCamera.x < 0;
        }
        else if (IsNotUsingLeftLimit)
        {
            isOutsideLimitX = resetpointOnCamera.x > Screen.width;
        }
        else
        {
            isOutsideLimitX = resetpointOnCamera.x < 0 || resetpointOnCamera.x > Screen.width;
        }

        return isOutsideLimitX || isOutsideLimitY;
    }

    public Victory GetVictoryTrigger()
    {
        return victoryTrigger;
    }
}
