using UnityEngine;

public class RandomMovingObject : RandomLevelEvent
{
    public float TimeMoving = 10f;
    public float Speed = 5f;

    SpriteRenderer spriteRend = null;
    Vector3 startPosition = Vector3.zero;
    float timer = 0f;

    bool isMovingLeft = false;
    bool isVisible = false;
    SoundModule sound = null;

    public override void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.enabled = false;

        sound = GetComponent<SoundModule>();

        startPosition = transform.position;

        Vector2 pointOnCamera = Camera.main.WorldToScreenPoint(startPosition);
        if (pointOnCamera.x > Screen.width)
        {
            isMovingLeft = true;
            spriteRend.flipX = true;
        }

        base.Start();
    }

    public override void Init()
    {
        base.Init();
        transform.position = startPosition;
    }

    public override void Play()
    {
        base.Play();
        spriteRend.enabled = true;
    }

    public override void UpdatePlaying()
    {
        timer = Mathf.Min(timer + Time.deltaTime, TimeMoving);
        transform.position += new Vector3(Time.deltaTime * Speed * (isMovingLeft ? -1f: 1f), 0f, 0f);

        if (timer >= TimeMoving)
        {
            spriteRend.enabled = false;
        }

        if (!isVisible && spriteRend.isVisible)
        {
            isVisible = true;
            if (sound != null && sound.DoesEventExist("Move"))
            {
                sound.PlayEvent("Move");
            }
        }
        else if (isVisible && !spriteRend.isVisible)
        {
            isVisible = false;
            if (sound != null && sound.DoesEventExist("Move"))
            {
                sound.StopEvent("Move");
            }
        }

        if (sound != null && sound.DoesEventExist("Move") && sound.IsPlaying("Move"))
        {
            sound.AttachInstanceTo("Move", transform, null);
        }
    }

    public override void Finish()
    {
        if (sound != null && sound.DoesEventExist("Move"))
        {
            sound.StopEvent("Move");
        }

        if (spriteRend != null)
        {
            spriteRend.enabled = false;
        }
        
        base.Finish();
    }

    public override bool GetIsFinished()
    {
        return timer >= TimeMoving;
    }
}
