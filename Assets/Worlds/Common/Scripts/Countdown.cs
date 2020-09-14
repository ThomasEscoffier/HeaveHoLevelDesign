using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {

    public float TimeCountdown = 10f;
    protected float timeDelay = 0f;
    protected float timer = 0f;

    protected Text text = null;
    protected bool isStarted = false;
    protected bool isDelayed = false;
    protected bool isFinished = false;

    public virtual void Awake()
    {
        text = GetComponent<Text>();
        text.enabled = false;
    }

    public virtual void Update ()
    {
        if (isStarted)
        {
            timer = Mathf.Max(timer - Time.deltaTime, 0f);
            RefreshCountdownText();
            if (timer == 0f)
            {
                isFinished = true;
                isStarted = false;
                if (text != null)
                {
                    text.enabled = false;
                }
            }
        }

        if (isDelayed)
        {
            timer = Mathf.Max(timer - Time.deltaTime, 0f);
            if (timer == 0f)
            {
                StartCountdown();
                isDelayed = false;
            }
        }
	}

    public virtual void RefreshCountdownText()
    {
        text.text = timer.ToString("F0");
    }

    public bool GetIsFinished()
    {
        return isFinished;
    }

    public bool GetIsStarted()
    {
        return isStarted;
    }

    public void SetCountdownCurrentTime(float timeCountdown)
    {
        timer = timeCountdown;
    }

    public virtual void StartCountdown()
    {
        //if (timer == 0f)
        //{
            timer = TimeCountdown;
        //}
        if (text != null)
        {
            text.enabled = true;
        }
        isStarted = true;
        isFinished = false;
    }

    public void StartCountDownWithDelay(float newTimeDelay, float timeCountdown)
    {
        timeDelay = newTimeDelay;
        timer = timeDelay;
        TimeCountdown = timeCountdown;
        isDelayed = true;
    }

    public virtual void Stop()
    {
        if (text != null)
        {
            text.enabled = false;
        }
        isStarted = false;
    }

    public float GetCurrentSpentTime()
    {
        return TimeCountdown - timer;
    }

    public float GetCurrentTimer()
    {
        return timer;
    }

    public void AddTime(float addedTime)
    {
        timer += addedTime;
    }
}
