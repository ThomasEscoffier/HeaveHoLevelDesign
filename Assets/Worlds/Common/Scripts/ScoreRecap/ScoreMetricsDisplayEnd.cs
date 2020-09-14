using UnityEngine;
using UnityEngine.UI;

public class ScoreMetricsDisplayEnd : ScoreMetricsDisplay
{
    public Text Description = null;

    void Awake()
    {
        Description.transform.localScale = new Vector3(0f, 0f, 0f);
        Description.gameObject.SetActive(false);
    }

    void Start()
    {
        GoToNextStep();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void GoToNextStep()
    {
        if (step == 0)
        {
            Description.gameObject.SetActive(true);
            for (int i = 0; i < fireworks.Length; ++i)
            {
                fireworks[i].StartFireworks();
            }
        }

        base.GoToNextStep();
    }
}
