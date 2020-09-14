using UnityEngine;

public class ScoreMetricsText : MonoBehaviour
{
    ScoreMetricsDisplay scoreRecap = null;

    void Awake()
    {
        scoreRecap = FindObjectOfType<ScoreMetricsDisplay>();
    }

    public void GoToNextStep()
    {
        scoreRecap.GoToNextStep();
    }
}
