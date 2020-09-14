using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;

public class LevelsButtonsPanel : MonoBehaviour
{
    public Animator BackgroundAnimator = null;
    public Image[] Tokens;
    public Image[] TokensUnfound;
    public float RepeatDelay = 0.2f;
    public float DeadZone = 0.5f;
    public GameObject PreviousPanel = null;
    public GameObject PreviousPanelFirstSelectedObj = null;

    public Animator ArrowLeft;
    public Animator ArrowRight;

    public Text HighScoreText;
    public Button currentButton = null;

    RuntimeAnimatorController[] menuAnimators;

    EventSystem eventSystem;
    SoundModule soundModule = null;
    float timer = 0f;

    bool isWorldSelected = false;

    public struct RunInfo
    {
        public string WorldName;
        public string LocaKey;
        public RuntimeAnimatorController BackgroundAnim;
        public float Record;
        public int Rank;
        public int NbToken;
        public int NbTokenMax;

        public RunInfo(string NewWorldName, string key, RuntimeAnimatorController anim, float record, int rank, int nbToken, int nbTokenMax)
        {
            WorldName = NewWorldName;
            LocaKey = key;
            BackgroundAnim = anim;
            Record = record;
            Rank = rank;
            NbToken = nbToken;
            NbTokenMax = nbTokenMax;
        }
    }

    List<WorldProperties> runs = new List<WorldProperties>();
    List<RunInfo> runInfos = new List<RunInfo>();
    int currentOrderInPanel = 0;
    string currentWorld = "";

    void Awake()
    {
        soundModule = GetComponent<SoundModule>();

        if (runs.Count == 0)
        {
            runs.AddRange(GameManager.Instance.GetLevelSelector().GetWorldProperties());
            runs.RemoveAll(i => i.TypeName == "MINIGAME");
        }
        runs.Sort((p1, p2) => p1.OrderInMenu.CompareTo(p2.OrderInMenu));

        menuAnimators = Resources.LoadAll<RuntimeAnimatorController>("WorldPreviews/");

        foreach (var run in runs)
        {
            bool exclude = false;
            foreach (var excluded in GameManager.Instance.GetLevelSelector().ExcludedWorlds)
            {
                if (excluded == null)
                {
                    continue;
                }
                if (run.WorldName == excluded.WorldName)
                {
                    exclude = true;
                    break;
                }
            }

            if (!exclude)
            {
                runInfos.Add(new RunInfo(run.WorldName, run.LocaKey, GetAnimatorFromName(run.BackgroundAnimatorName), 0, 0, 0, 0));
            }
        }

        eventSystem = FindObjectOfType<EventSystem>();
    }

    void Start()
    {
        if (!isWorldSelected)
        {
            OnRunSelected(0, runInfos[0]);
        }
    }

    void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, RepeatDelay);

        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if (playerInput.GetButtonDown("Cancel"))
            {
                ActivatePreviousPanel();
                soundModule.PlayOneShot("Cancel");
            }

            if (currentOrderInPanel > 0 && timer == RepeatDelay && playerInput.GetAxis("NavHorizontal") < -DeadZone)
            {
                currentOrderInPanel--;
                UpdateShowArrows();
                ArrowLeft.SetTrigger("Activate");
                soundModule.PlayOneShot("Arrow");
                OnRunSelected(currentOrderInPanel, runInfos[currentOrderInPanel]);
                timer = 0f;
            }
            else if (currentOrderInPanel < (runInfos.Count - 1) && timer == RepeatDelay && playerInput.GetAxis("NavHorizontal") > DeadZone)
            {
                currentOrderInPanel++;
                UpdateShowArrows();
                ArrowRight.SetTrigger("Activate");
                soundModule.PlayOneShot("Arrow");
                OnRunSelected(currentOrderInPanel, runInfos[currentOrderInPanel]);
                timer = 0f;
            }
        }
    }

    RuntimeAnimatorController GetAnimatorFromName(string animName)
    {
        for (int i = 0; i < menuAnimators.Length; ++i)
        {
            if (menuAnimators[i].name.Equals(animName))
            {
                return menuAnimators[i];
            }
        }
        return null;
    }

    public void SelectWorld(string worldName)
    {
        for (int i = 0; i < runInfos.Count; ++i)
        {
            if (runInfos[i].WorldName == worldName)
            {
                currentOrderInPanel = i;
                UpdateShowArrows();
                ArrowLeft.SetTrigger("Activate");
                soundModule.PlayOneShot("Arrow");
                OnRunSelected(currentOrderInPanel, runInfos[currentOrderInPanel]);
                isWorldSelected = true;
            }
        }
    }

    void UpdateShowArrows()
    {
        ArrowLeft.gameObject.SetActive(currentOrderInPanel > 0);
        ArrowRight.gameObject.SetActive(currentOrderInPanel < runInfos.Count - 1);
    }

    void ActivatePreviousPanel()
    {
        PreviousPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(PreviousPanelFirstSelectedObj);
        transform.parent.gameObject.SetActive(false);
    }

    void OnRunSelected(int orderInPanel, RunInfo buttonInfo)
    {
        currentOrderInPanel = orderInPanel;
        currentWorld = buttonInfo.WorldName;
        BackgroundAnimator.runtimeAnimatorController = buttonInfo.BackgroundAnim;

        float highscore = GameManager.Instance.GetScoreManager().GetScoreFromWorld(currentWorld, GameManager.Instance.GetLevelSelector().GetCurrentGameMode());
        int minutes = (int)(highscore / 60f);
        int seconds = (int)(highscore % 60f);
        float miliseconds = (highscore * 1000f) % 1000f / 10f;
        HighScoreText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, miliseconds);

        WorldProperties world = GameManager.Instance.GetLevelSelector().GetWorldPropertiesFromName(currentWorld);
        int indexToStart = 0;
        for (int i = 0; i < world.runs[0].properties.Count; ++i)
        {
            if (!world.runs[0].properties[i].HasToken)
            {
                indexToStart++;
            }
        }

        //No token to find in Solo mode
        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.SOLO)
        {
            for (int i = 0; i < Tokens.Length; ++i)
            {
                Tokens[i].gameObject.SetActive(false);
                TokensUnfound[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < Tokens.Length; ++i)
            {
                if (i >= indexToStart && world.runs[0].properties[i - indexToStart].HasToken)
                {
                    if (GameManager.Instance.GetSaveManager().IsTokenAlreadyAcquired(currentWorld, i - indexToStart))
                    {
                        Tokens[i].gameObject.SetActive(true);
                        TokensUnfound[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        Tokens[i].gameObject.SetActive(false);
                        TokensUnfound[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    Tokens[i].gameObject.SetActive(false);
                    TokensUnfound[i].gameObject.SetActive(false);
                }
            }
        }

        timer = 0f;
    }

    public void GoToCharacterSelection()
    {
        GameManager.Instance.GetLevelSelector().SetNextWorld(currentWorld);
        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.SOLO)
        {
            GameManager.Instance.KeepSmallerId();
            GameManager.Instance.GetLevelSelector().LoadSelectionMenuSoloScene();
        }
        else
        {
            GameManager.Instance.GetLevelSelector().LoadSelectionMenuScene();
        }
    }

    public void SetCurrentButton(Color color, string locaText)
    {
        currentButton.GetComponent<Image>().color = color;
        currentButton.GetComponentInChildren<Localization>().SetText(locaText);
    }

    public void RefreshLevelSelected()
    {
        if (runInfos.Count > 0)
        {
            OnRunSelected(currentOrderInPanel, runInfos[currentOrderInPanel]);
        }
    }
}
