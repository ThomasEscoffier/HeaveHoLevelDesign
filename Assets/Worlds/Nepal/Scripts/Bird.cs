using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

    public AnimationCurve FlyCurve;
    public AnimationCurve ComeBackCurve;
    public float FlyTime = 5f;
    public float ComeBackTime = 3f;
    public float TimeWaitingToComeBack = 20f;

    public Transform EndPos = null;
    public bool IsFlyingLeft = false;

    Transform spriteTransform = null;

    Vector3 startPos = Vector3.zero;
    float timer = 0f;

    Animator anim = null;
    SoundModule sound = null;
    SpriteRenderer spriteRend = null;
    List<BodyPart> bodyPartsInTrigger = new List<BodyPart>();

    enum eState
    {
        WAITING,
        WAITING_OUTSIDE_SCREEN,
        FLY,
        COMEBACK,
    }

    eState currentState = eState.WAITING;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sound = GetComponentInChildren<SoundModule>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        spriteTransform = spriteRend.transform;
        startPos = spriteTransform.position;
        SetState(eState.WAITING);
	}
	
	void Update()
    {
        switch (currentState)
        {
            case eState.WAITING:
                if (bodyPartsInTrigger.Count > 0)
                {
                    SetState(eState.FLY);
                }
                break;
            case eState.WAITING_OUTSIDE_SCREEN:
                timer = Mathf.Min(timer + Time.deltaTime, TimeWaitingToComeBack);
                if (bodyPartsInTrigger.Count == 0 && timer == TimeWaitingToComeBack)
                {
                    SetState(eState.COMEBACK);
                }
                break;
            case eState.FLY:
                timer = Mathf.Min(timer + Time.deltaTime, FlyTime);
                spriteTransform.position = Vector3.Lerp(startPos, EndPos.position, FlyCurve.Evaluate(timer));
                if (timer == FlyTime)
                {
                    SetState(eState.WAITING_OUTSIDE_SCREEN);
                }
                break;
            case eState.COMEBACK:
                timer = Mathf.Min(timer + Time.deltaTime, ComeBackTime);
                spriteTransform.position = Vector3.Lerp(EndPos.position, startPos, ComeBackCurve.Evaluate(timer));
                if (timer == ComeBackTime)
                {
                    SetState(eState.WAITING);
                }
                break;
        }
	}

    void SetState(eState state)
    {
        currentState = state;

        switch (currentState)
        {
            case eState.WAITING:
                anim.SetBool("Flying", false);
                break;
            case eState.WAITING_OUTSIDE_SCREEN:
                timer = 0f;
                spriteRend.enabled = false;
                anim.SetBool("Flying", false);
                break;
            case eState.FLY:
                timer = 0f;
                spriteRend.flipX = !IsFlyingLeft;
                sound.PlayOneShot("FlyUp", transform.position);
                anim.SetBool("Flying", true);
                break;
            case eState.COMEBACK:
                timer = 0f;
                spriteRend.enabled = true;
                spriteRend.flipX = IsFlyingLeft;
                sound.PlayOneShot("FlyDown", transform.position);
                anim.SetBool("Flying", true);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            BodyPart part = collision.GetComponent<BodyPart>();
            if (part != null && !bodyPartsInTrigger.Contains(part))
            {
                bodyPartsInTrigger.Add(part);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            BodyPart part = collision.GetComponent<BodyPart>();
            if (part != null)
            {
                bodyPartsInTrigger.Remove(part);
            }
        }
    }
}
