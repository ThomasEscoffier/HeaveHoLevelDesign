using UnityEngine;

public class HelpRope : MonoBehaviour
{
    public float TimeBeforeAppearing = 600f;
    public float FadeTime = 3f;

    float timer = 0f;
    bool isFading = false;

    SpriteRenderer[] elements;

    Color startColor = new Color(1f, 1f, 1f, 0f);
    Color endColor = new Color(1f, 1f, 1f, 1f);

    void Awake()
    {
        elements = GetComponentsInChildren<SpriteRenderer>(true);
    }

    void Update()
    {
        if (isFading)
        {
            timer = Mathf.Min(timer + Time.deltaTime, FadeTime);

            for (int i = 0; i < elements.Length; ++i)
            {
                elements[i].color = Color.Lerp(startColor, endColor, timer / FadeTime);
            }

            if (timer == FadeTime)
            {
                enabled = false;
                timer = 0f;
            }
        }
        else
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeAppearing);
            if (timer == TimeBeforeAppearing)
            {
                isFading = true;
                for (int i = 0; i < elements.Length; ++i)
                {
                    elements[i].gameObject.SetActive(true);
                    elements[i].color = startColor;
                }
                timer = 0f;
            }
        }
    }

    public float GetCurrentTimer()
    {
        return timer;
    }

    public bool GetIsFading()
    {
        return isFading;
    }

    public void ReactivateTimer(float newTimer, bool isDisplayed)
    {
        if (isDisplayed)
        {
            for (int i = 0; i < elements.Length; ++i)
            {
                elements[i].gameObject.SetActive(true);
                elements[i].color = endColor;
            }
            enabled = false;
        }
        else
        {
            timer = newTimer;
        }
    }
}
