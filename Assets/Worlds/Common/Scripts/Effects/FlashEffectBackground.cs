using UnityEngine;

public class FlashEffectBackground : MonoBehaviour {

    SpriteRenderer flashImage = null;
    public float StartFlashTime = 0f;
    public float WaitBeforeFade = 0f;
    public float EndFadeFlashTime = 0.5f;

    float timerFlash;

    bool flash = false;
    bool startFade = false;
    bool isWaiting = false;

    void Awake()
    {
        flashImage = GetComponent<SpriteRenderer>();
        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0f);
    }

    void Update()
    {
        if (flash)
        {
            if (startFade)
            {
                timerFlash = Mathf.Min(timerFlash + Time.deltaTime, StartFlashTime);
                if (StartFlashTime == 0f)
                {
                    flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 1f);
                }
                else
                {
                    flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, Mathf.Lerp(0f, 1f, timerFlash / StartFlashTime));
                }

                if (timerFlash == StartFlashTime)
                {
                    startFade = false;
                    isWaiting = true;
                    timerFlash = 0f;
                }
            }
            else if (isWaiting)
            {
                timerFlash = Mathf.Min(timerFlash + Time.deltaTime, WaitBeforeFade);
                if (timerFlash == WaitBeforeFade)
                {
                    isWaiting = false;
                    timerFlash = 0f;
                }
            }
            else
            {
                timerFlash = Mathf.Min(timerFlash + Time.deltaTime, EndFadeFlashTime);
                flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, Mathf.Lerp(1f, 0f, timerFlash / EndFadeFlashTime));
                if (timerFlash == EndFadeFlashTime)
                {
                    flash = false;
                    timerFlash = 0f;
                }
            }
        }
    }

    public void Launch()
    {
        flash = true;
        timerFlash = 0f;
        startFade = true;
    }

    public void SetColor(Color newColor)
    {
        flashImage.color = newColor;
    }
}
