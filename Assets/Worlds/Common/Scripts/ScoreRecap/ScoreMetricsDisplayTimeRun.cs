using UnityEngine;
using UnityEngine.UI;

public class ScoreMetricsDisplayTimeRun : ScoreMetricsDisplay
{
    public Text RunTimeText = null;
    public Text TimeText = null;
    public Text BestScoreText = null;

    MetricsManager.RunMetrics runMetrics;
    SoundModule sound = null;

    bool showBestScore = false;

    void Awake()
    {
        sound = GetComponent<SoundModule>();

        RunTimeText.transform.localScale = new Vector3(0f, 0f, 0f);
        TimeText.transform.localScale = new Vector3(0f, 0f, 0f);
        BestScoreText.transform.localScale = new Vector3(0f, 0f, 0f);

        RunTimeText.gameObject.SetActive(false);
        TimeText.gameObject.SetActive(false);
        BestScoreText.gameObject.SetActive(false);
    }

    void Start()
    {
        runMetrics = GameManager.Instance.GetMetricsManager().GetCurrentRunMetrics();
        if (runMetrics != null)
        {
            float score = 0f;
            for (int i = 0; i < runMetrics.LevelTimes.Count; ++i)
            {
                score += runMetrics.LevelTimes[i];
            }
            SetRunTimeText(score);
            if (GameManager.Instance.GetScoreManager().IsBetterRunScore(GameManager.Instance.GetLevelSelector().GetCurrentWorld().WorldName, score, GameManager.Instance.GetLevelSelector().GetCurrentRun().NbPlayerMin, GameManager.Instance.GetLevelSelector().GetCurrentGameMode()))
            {
                showBestScore = true;
                sound.PlayOneShot("CrowdCheer");
                for (int i = 0; i < fireworks.Length; ++i)
                {
                    fireworks[i].StartFireworks();
                }
            }
            GoToNextStep();
        }
    }

    public override void Update()
    {
        if ((showBestScore && step == 4) || step == 3)
        {
            base.Update();
        }
    }

    public override void GoToNextStep()
    {
        if (step == 0)
        {
            RunTimeText.gameObject.SetActive(true);
        }
        else if (step == 1)
        {
            TimeText.gameObject.SetActive(true);
        }
        else if (step == 2 && showBestScore)
        {
            BestScoreText.gameObject.SetActive(true);
        }

        base.GoToNextStep();
    }

    public void SetRunTimeText(float currentScore)
    {
        int minutes = (int)(currentScore / 60f);
        int seconds = (int)(currentScore % 60f);
        float miliseconds = (currentScore * 1000f) % 1000f / 10f;
        TimeText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, miliseconds);
    }
}
