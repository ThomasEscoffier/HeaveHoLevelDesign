using UnityEngine;
using UnityEngine.UI;

public class SimpleUIAnimator : MonoBehaviour
{
    public Sprite[] sprites;
    public float frameRate = 10;
    public bool isRawDeltaTime = false;

    private Image image;
    private float timeSinceLastFrame;
    private float frameTime;
    private int currentIndex = 0;

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
        
        if (sprites.Length != 0)
        {
            if (sprites[0] != null)
            {
                image.sprite = sprites[0];
            }
        }
        else
        {
            enabled = false;
        }

        frameTime = 1 / frameRate;
    }

    protected virtual void Update()
    {
        timeSinceLastFrame += isRawDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;

        //If the game is using RawDeltaTime and unfocused/paused/SWITCH Applet is on, then the UnscaledDeltaTime
        //will get very high when returning to focus, which causes the animation to rush. 
        if (isRawDeltaTime && timeSinceLastFrame > frameTime * 2.0f)
        {
            timeSinceLastFrame = 0.0f;
        }

        if (timeSinceLastFrame >= frameTime)
        {
            currentIndex++;

            if (currentIndex >= sprites.Length)
            {
                currentIndex = 0;
            }

            image.sprite = sprites[currentIndex];

            timeSinceLastFrame  -= frameTime;
        }
    }
}