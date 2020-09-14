using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnlockTrapeze : MonoBehaviour {

    public float RopeMoveTime = 5f;
    public float RopeMoveActivateTime = 2f;
    public AnimationCurve RopeAppearSpeed;
    public AnimationCurve RopeActivateSpeed;
    public Transform EndPos;
    public Transform EndPosActivate;
    public Rigidbody2D RopeBase;

    enum eState
    {
        APPEARING,
        WAIT_CHARACTERS,
        ACTIVATE,
    }

    eState currentState = eState.APPEARING;

    float timer = 0f;
    Vector3 startposition = Vector3.zero;
    bool isActivated = false;
    SoundModule soundModule = null;

    MiniGameRopePart[] miniGameRopeParts;

    public UnityEvent ActivateEvent = new UnityEvent();

    void Awake()
    {
        soundModule = GetComponent<SoundModule>();
        startposition = RopeBase.transform.position;
        miniGameRopeParts = GetComponentsInChildren<MiniGameRopePart>();
    }

    void Update()
    {
        UpdateState();
    }

    void SetState(eState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case eState.APPEARING:
                {
                    timer = 0f;
                    break;
                }
            case eState.WAIT_CHARACTERS:
                {
                    timer = 0f;
                    RopeBase.bodyType = RigidbodyType2D.Static;
                    break;
                }
            case eState.ACTIVATE:
                {
                    timer = 0f;
                    RopeBase.bodyType = RigidbodyType2D.Kinematic;
                    ActivateEvent.Invoke();
                    soundModule.PlayOneShot("HelpRopeActivate", gameObject);
                    break;
                }
            default:
                break;
        }
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case eState.APPEARING:
                {
                    timer = Mathf.Min(timer + Time.deltaTime, RopeMoveTime);
                    if (timer == RopeMoveTime)
                    {
                        SetState(eState.WAIT_CHARACTERS);
                    }
                    else
                    {
                        RopeBase.MovePosition(Vector3.Lerp(startposition, EndPos.position, RopeAppearSpeed.Evaluate(timer)));
                    }
                    break;
                }
            case eState.WAIT_CHARACTERS:
                {
                    if (!isActivated)
                        break;

                    foreach (MiniGameRopePart part in miniGameRopeParts)
                    {
                        List<Character> characters = Character.GetCharactersHolding(part.gameObject);
                        if (characters.Count > 0)
                        {
                            SetState(eState.ACTIVATE);
                            break;
                        }
                    }
                    break;
                }
            case eState.ACTIVATE:
                {
                    timer = Mathf.Min(timer + Time.deltaTime, RopeMoveActivateTime);
                    if (timer == RopeMoveActivateTime)
                    {
                        isActivated = false;
                        SetState(eState.WAIT_CHARACTERS);
                    }
                    else
                    {
                        RopeBase.MovePosition(Vector3.Lerp(EndPos.position, EndPosActivate.position, RopeActivateSpeed.Evaluate(timer)));
                    }
                    break;
                }
            default:
                break;
        }
    }

    public void SetIsActivated(bool state)
    {
        isActivated = state;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        MiniGameRopePart part = collision.GetComponent<MiniGameRopePart>();
        if (part != null)
        {
            if (currentState == eState.APPEARING || currentState == eState.ACTIVATE)
            {
                part.SetSpriteOn(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        MiniGameRopePart part = collision.GetComponent<MiniGameRopePart>();
        if (part != null)
        {
            if (currentState == eState.ACTIVATE)
            {
                part.SetSpriteOn(false);
            }
        }
    }
}
