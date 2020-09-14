using UnityEngine;

public class ScoreMetricsDisplay : MonoBehaviour
{
    public float TimeWaitBeforeFinished = 1f;

    protected bool isFinished = false;
    protected int step = 0;
    protected Fireworks[] fireworks;
    protected ScoreRecap scoreRecap = null;

    float timer = 0f;

    public virtual void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, TimeWaitBeforeFinished);
        if (timer == TimeWaitBeforeFinished)
        {
            isFinished = true;
        }
    }

    public bool GetIsFinished()
    {
        return isFinished;
    }

    public virtual void GoToNextStep()
    {
        step++;
    }

    public void SetFireworks(Fireworks[] newFireworks)
    {
        fireworks = newFireworks;
    }

    public void SetScoreRecap(ScoreRecap recap)
    {
        scoreRecap = recap;
    }
}
