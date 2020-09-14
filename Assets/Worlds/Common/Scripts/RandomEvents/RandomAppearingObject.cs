using UnityEngine;

public class RandomAppearingObject : RandomLevelEvent
{
    public float TimeBeforeEnd = 5f;
    public float TimeTransition = 1f;

    SpriteRenderer spriteRend = null;

    Vector3 startPos = Vector3.zero;
    Vector3 endPos = Vector3.zero;
    float timerMoving = 0f;
    bool isShowing = false;
    bool isInTransition = false;

    public override void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.enabled = false;

        startPos = transform.position;
        Camera cachedMainCamera = Camera.main;
        
        Vector2 pointOnCamera = cachedMainCamera.WorldToScreenPoint(startPos);
        if (pointOnCamera.x < 0)
        {
            endPos = new Vector3(cachedMainCamera.ScreenToWorldPoint(Vector3.zero).x + spriteRend.bounds.size.x / 2, startPos.y, 0);
        }
        else if (pointOnCamera.x > Screen.width)
        {
            endPos = new Vector3(cachedMainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - spriteRend.bounds.size.x / 2, startPos.y, 0);
        }
        else if (pointOnCamera.y < 0)
        {
            endPos = new Vector3(startPos.x, cachedMainCamera.ScreenToWorldPoint(Vector3.zero).y + spriteRend.bounds.size.y / 2, 0);
        }
        else if (pointOnCamera.y > Screen.height)
        {
            endPos = new Vector3(startPos.x, cachedMainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - spriteRend.bounds.size.y / 2, 0);
        }
        base.Start();
    }

    public override void Init()
    {
        base.Init();
        timerMoving = 0f;
        transform.position = startPos;
    }

    public override void Play()
    {
        base.Play();
        isInTransition = true;
        spriteRend.enabled = true;
        isShowing = true;
    }

    public override void UpdatePlaying()
    {
        if (isShowing)
        {
            if (isInTransition)
            {
                timerMoving = Mathf.Min(timerMoving + Time.deltaTime, TimeTransition);
                transform.position = Vector3.Lerp(startPos, endPos, timerMoving/TimeTransition);
                if (timerMoving >= TimeTransition)
                {
                    isInTransition = false;
                    timerMoving = 0f;
                }
            }
            else
            {
                timerMoving = Mathf.Min(timerMoving + Time.deltaTime, TimeBeforeEnd);
                if (timerMoving >= TimeBeforeEnd)
                {
                    isInTransition = true;
                    isShowing = false;
                    timerMoving = 0f;
                }
            }
        }
        else
        {
            if (isInTransition)
            {
                timerMoving = Mathf.Min(timerMoving + Time.deltaTime, TimeTransition);
                transform.position = Vector3.Lerp(endPos, startPos, timerMoving / TimeTransition);
                if (timerMoving >= TimeTransition)
                {
                    isInTransition = false;
                    spriteRend.enabled = false;
                    timerMoving = 0f;
                }
            }
        }
    }

    public override void Finish()
    {
        isInTransition = true;
        isShowing = false;
        timerMoving = 0f;
        base.Finish();
    }

    public override bool GetIsFinished()
    {
        return !isShowing && !isInTransition;
    }
}