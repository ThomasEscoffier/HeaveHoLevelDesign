using Managers; 
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class PauseMenu : MonoBehaviour
{
    public GameObject Base = null;
    public GameObject OptionsMenu = null;
    public GameObject FirstOption = null;
    public GameObject FirstPauseOption = null;
    public GameObject OptionsObj = null;
    public GameObject Quit = null;

    public GameObject[] PauseOptions;
    public GameObject[] Options;

    public float RepeatDelay = 0.2f;

    EventSystem eventSystem = null;
    SoundModule soundModule = null;

    bool isLoading = false;

    public bool IsOn { get; private set; }
    
    void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        soundModule = GetComponent<SoundModule>();
    }

    void Update()
    {
        foreach (Player player in GameManager.Instance.GetPlayers())
        {
            if (!isLoading && player.GetControls().GetPlayerInput().GetButtonDown(RewiredConsts.Action.Character_Pause))
            {
                ToggleMenu();
            }
            if (OptionsMenu.gameObject.activeSelf && player.GetControls().GetPlayerInput().GetButtonDown(RewiredConsts.Action.Menu_Cancel))
            {
                ToggleOptions();
            }
        }
    }

    public void ToggleMenu()
    {
        if (Base == null) // So Steam callback doesn't crash if the scene doesn't have a Pause menu
            return;
        if (IsOn)
        {
            Time.timeScale = 1f;
            Base.gameObject.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
            foreach (Player player in GameManager.Instance.GetPlayers())
            {
                player.GetControls().SetMapEnabled(false, "Menu");
                if (player.GetCurrentCharacter() != null)
                {
                    player.GetCurrentCharacter().UnPause();
                }
            }
            soundModule.StopEvent("Pause");
            OptionsMenu.gameObject.SetActive(false);
            GameManager.Instance.GetMusicManager().UnPauseCurrentMusic();
            IsOn = false;
        }
        else if (Time.timeScale != 0)
        {
            Time.timeScale = 0f;
            Base.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(FirstPauseOption);
            foreach (Player player in GameManager.Instance.GetPlayers())
            {
                player.GetControls().SetMapEnabled(true, "Menu");
                if (player.GetCurrentCharacter() != null)
                {
                    player.GetCurrentCharacter().Pause();
                }
            }
            soundModule.PlayEvent("Pause");
            GameManager.Instance.GetMusicManager().PauseCurrentMusic();
            IsOn = true;
        }
    }

    public void ToggleOptions()
    {
        if (OptionsMenu.gameObject.activeSelf)
        {
            eventSystem.SetSelectedGameObject(OptionsObj);
            OptionsMenu.gameObject.SetActive(false);
        }
        else
        {
            OptionsMenu.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(FirstOption);
        }
    }

    public void Resume()
    {
        ToggleMenu();
    }

    public void RestartLevel()
    {
        if (isLoading || GameManager.Instance.GetLevelSelector().GetCurrentLevel().LevelType == LevelProperties.eLevelType.OTHER)
            return;

        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
        isLoading = true;
    }

    public void RestartRun()
    {
        if (isLoading || GameManager.Instance.GetLevelSelector().GetCurrentLevel().LevelType == LevelProperties.eLevelType.OTHER)
            return;

            GameManager.Instance.GetLevelSelector().RestartRun();
            isLoading = true;

    }

    public void BackMainMenu()
    {
        if (isLoading)
            return;

        Time.timeScale = 1f;
        GameManager.Instance.GetLevelSelector().LoadMainScene();
        isLoading = true;
    }

    public bool GetIsLoading()
    {
        return isLoading;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
