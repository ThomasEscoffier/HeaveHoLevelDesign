using UnityEngine;

[CreateAssetMenu(menuName = "Level/Rules/ScrollingTimer")]
public class ScrollingTimerRules : ScrollingRules {

    Countdown countdownUI = null;

    public float TimeEndLevel = 600f;

    public override void OnStart()
    {
        base.OnStart();

        countdownUI = FindObjectOfType<Countdown>();
        countdownUI.TimeCountdown = TimeEndLevel;
        countdownUI.StartCountdown();

        BonusTimeObject[] objs = FindObjectsOfType<BonusTimeObject>();
        for (int i = 0; i < objs.Length; ++i)
        {
            objs[i].SetRules(this);
        }

    }

    public override bool CheckIsFinished()
    {
        if (!base.CheckIsFinished())
        {
            if (countdownUI.GetCurrentTimer() == 0f)
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        return false;
    }

    public override float GetCurrentScore()
    {
        return countdownUI.GetCurrentSpentTime();
    }

    public void AddTime(float time)
    {
        countdownUI.AddTime(time);
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
