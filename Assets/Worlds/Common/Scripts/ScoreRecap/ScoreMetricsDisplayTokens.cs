using UnityEngine;
using UnityEngine.UI;

public class ScoreMetricsDisplayTokens : ScoreMetricsDisplay
{
    public Text Description = null;
    public Text NbTokens = null;

    MetricsManager.RunMetrics runMetrics;
    SoundModule sound = null;

    void Awake()
    {
        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.VERSUS)
        {
            isFinished = true;
        }
        sound = GetComponent<SoundModule>();

        Description.transform.localScale = new Vector3(0f, 0f, 0f);

        Description.gameObject.SetActive(false);
        NbTokens.gameObject.SetActive(false);
    }

    void Start()
    {
        runMetrics = GameManager.Instance.GetMetricsManager().GetCurrentRunMetrics();
        if (runMetrics != null)
        {
            NbTokens.text = (runMetrics.NbTokenObtained + runMetrics.NbTokenObtainedMinigame).ToString();
            GoToNextStep();
        }
    }

    public override void Update()
    {
        if (step == 2)
        {
            base.Update();
        }
    }

    public override void GoToNextStep()
    {
        if (step == 0)
        {
            Description.gameObject.SetActive(true);
        }
        if (step == 1)
        {
            NbTokens.gameObject.SetActive(true);
            if ((runMetrics.NbTokenObtained + runMetrics.NbTokenObtainedMinigame) > 0)
            {
                sound.PlayOneShot("CrowdCheer");
                foreach (var firework in fireworks)
                {
                    if (firework != null)
                    {
                        firework.StartFireworks();
                    }
                }
            }
        }

        base.GoToNextStep();
    }
}
