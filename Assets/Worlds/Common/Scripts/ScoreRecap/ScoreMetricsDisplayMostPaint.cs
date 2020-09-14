using UnityEngine;
using UnityEngine.UI;

public class ScoreMetricsDisplayMostPaint : ScoreMetricsDisplay
{
    public ScoreRecapHead CharacterHead = null;
    public Text Description = null;
    public Text NobodyPaint = null;

    MetricsManager.PlayerMetrics playerMetrics;

    void Awake()
    {
        Description.transform.localScale = new Vector3(0f, 0f, 0f);
        NobodyPaint.transform.localScale = new Vector3(0f, 0f, 0f);

        Description.gameObject.SetActive(false);
        NobodyPaint.gameObject.SetActive(false);
        CharacterHead.gameObject.SetActive(false);
    }

    void Start()
    {
        //playerMetrics = GameManager.Instance.GetMetricsManager().GetPlayerWithMostPaint();
        if (playerMetrics == null)
        {
            NobodyPaint.gameObject.SetActive(true);
            step = 1;
        }
        else
        {
            CharacterHead.gameObject.SetActive(true);
            CharacterHead.SetOutfit(playerMetrics.CorrespondingPlayer.GetCurrentCharacterOutfit());
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

        base.GoToNextStep();
    }
}
