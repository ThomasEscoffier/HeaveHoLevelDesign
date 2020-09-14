using System.Collections.Generic;
using UnityEngine;

public class CameraScrolling : MonoBehaviour {

    Animator anim;

    public float RewindSpeed = 2f;

    List<Player> players = new List<Player>();

    bool isRewinding = false;
    bool isStarted = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (GameManager.Instance)
        {
            players = GameManager.Instance.GetPlayers();
        }
    }

    void Update()
    {
        if (!isStarted)
        {
            if (GameManager.Instance.AreAllPlayersDead())
            {
                return;
            }
            else
            {
                isStarted = true;
            }
        }

        if (isRewinding)
        {
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] animatorClip = anim.GetCurrentAnimatorClipInfo(0);
            float timerLevel = animatorClip[0].clip.length * animationState.normalizedTime;

            if (players[0].GetCheckPoint().IsCameraRestartPointIn() || timerLevel <= 0f)
            {
                StopRewind();
            }
        }
        else
        {
            if (GameManager.Instance.AreAllPlayersDead())
            {
                if (players[0].GetCheckPoint().IsCheckpointInSafeZone())
                {
                    for (int i = 0; i < players.Count; ++i)
                    {
                        players[i].Respawn();
                    }
                }
                else
                {
                    Rewind();
                }
            }
        }
    }

    void Rewind()
    {
        isRewinding = true;
        anim.SetFloat("Direction", -RewindSpeed);
    }

    void StopRewind()
    {
        anim.SetFloat("Direction", 1f);
        isRewinding = false;

        for (int i = 0; i < players.Count; ++i)
        {
            players[i].Respawn();
        }
    }
}
