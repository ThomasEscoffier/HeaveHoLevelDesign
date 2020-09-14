using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomHelpObject : MonoBehaviour
{
    public UnityEvent OnDeath = new UnityEvent();
    public float TimeBeforeDeath = 1f;
    public bool UseLevelManagerLimits = true;
    public GameObject DestroyEffect = null;
    public float TimeUnlockTrophy = 3f;
    float timer = 0f;

    Rigidbody2D rb = null;
    LevelManager levelManager = null;
    SoundModule soundModule = null;

    float timerBeforeDeath = 0f;
    int nbHooks = 0;

    bool isTaken = false;
    bool isOutsideCamera = false;
    bool isPaused = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        levelManager = FindObjectOfType<LevelManager>();
        soundModule = GetComponent<SoundModule>();
    }

    void Update()
    {
        if (isPaused)
            return;

        if (!isTaken)
        {
            if (isOutsideCamera)
            {
                if (LevelManager.IsObjectInsideCamera(gameObject))
                {
                    isOutsideCamera = false;
                }
                else
                {
                    timerBeforeDeath = Mathf.Min(timerBeforeDeath + Time.deltaTime, TimeBeforeDeath);
                    if (timerBeforeDeath == TimeBeforeDeath)
                    {
                        Die();
                    }
                }
            }
            else if (rb.bodyType == RigidbodyType2D.Dynamic && UseLevelManagerLimits ? levelManager.GetIsOutsideLimit(gameObject) : !LevelManager.IsObjectInsideCamera(gameObject))
            {
                isOutsideCamera = true;
                timerBeforeDeath = 0f;
            }
        }
    }

    List<Hand> GetHoldingHands()
    {
        List<Hand> hands = new List<Hand>();
        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();

        foreach (FixedJoint2D joint in joints)
        {
            if (joint.connectedBody == null)
            {
                continue;
            }
            Hand hand = joint.connectedBody.GetComponent<Hand>();
            if (hand != null)
            {
                hands.Add(hand);
            }
        }

        return hands;
    }

    public virtual void StartIsTaken(Hand hand)
    {
        if (nbHooks == 0)
        {
            isTaken = true;
            hand.LockWrist();
        }
        else if (nbHooks == 1)
        {
            List<Hand> hands = GetHoldingHands();
            foreach (Hand otherHand in hands)
            {
                otherHand.UnlockWrist();
            }
            hand.UnlockWrist();
        }
        nbHooks++;
    }

    public virtual void StopIsTaken(Hand hand)
    {
        nbHooks--;
        if (nbHooks == 0)
        {
            timer = 0f;
            isTaken = false;
        }
        else if (nbHooks == 1)
        {
            List<Hand> hands = GetHoldingHands();
            foreach (Hand otherHand in hands)
            {
                otherHand.LockWrist();
            }
        }
    }

    public void Reactivate()
    {
        isPaused = false;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Die()
    {
        List<Hand> hands = GetHoldingHands();
        foreach (Hand hand in hands)
        {
            hand.UnHook();
        }
        if (soundModule != null)
        {
            soundModule.PlayOneShot("Die", gameObject);
        }
        if (DestroyEffect != null)
        {
            Instantiate(DestroyEffect, transform.position, Quaternion.identity);
        }
        OnDeath.Invoke();
        Destroy(gameObject);
    }
}
