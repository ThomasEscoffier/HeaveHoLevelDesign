using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour {

    public bool RespawnEveryoneWhenActivated = true;
    public SpriteRenderer SpriteRend = null;
    public Color ColorActivated = Color.green;
    public Color ColorStandby = Color.yellow;
    public List<RespawnPoint> AssociatedRespawnPoints = null;
    public Transform CameraRestartPoint = null;
    public bool UseDifferentRespawnPoints = false;

    protected bool isActivated = false;
    bool hasToRewind = false;
    bool isRewindActivated = false;
    SoundModule soundModule = null;
    Camera cam = null;

    public float UpBorderRatio = 0.25f;
    public float DownBorderRatio = 0.25f;
    public float LeftBorderRatio = 0.25f;
    public float RightBorderRatio = 0.25f;

    public bool IsFirstCheckpoint = false;

    public CheckpointParamEvent CheckpointActivatedEvent = new CheckpointParamEvent();
    public class CheckpointParamEvent : UnityEvent<Checkpoint> { }

    void Awake()
    {
        soundModule = GetComponent<SoundModule>();
        cam = Camera.main;
    }

    void Update()
    {
        if (isActivated && isRewindActivated)
        {
            if (!hasToRewind && !IsCheckpointInSafeZone())
            {
                if (SpriteRend != null)
                {
                    SpriteRend.color = ColorStandby;
                }
                hasToRewind = true;
            }
            else if (hasToRewind && IsCheckpointInSafeZone())
            {
                if (SpriteRend != null)
                {
                    SpriteRend.color = ColorActivated;
                }
                hasToRewind = false;
            }
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isActivated)
        {
            if (collider.CompareTag("Character"))
            {
                Character obj = collider.GetComponent<Character>();
                if (obj && LevelManager.IsObjectInsideCamera(gameObject))
                {
                    List<Player> players = GameManager.Instance.GetPlayers();
                    for (int i = 0; i < players.Count; ++i)
                    {
                        if (UseDifferentRespawnPoints)
                        {
                            players[i].SetCheckpoint(this, AssociatedRespawnPoints.ToArray());
                        }
                        else
                        {
                            players[i].SetCheckpoint(this, AssociatedRespawnPoints.ToArray());
                        }
                    }
                    SetIsActivated();
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        OnTriggerEnter2D(collider);
    }

    public void SetRewindActivated(bool state)
    {
        isRewindActivated = state;
    }

    public bool IsCheckpointInSafeZone()
    {
        Vector2 checkpointOnScreen = cam.WorldToScreenPoint(transform.position);
        return (checkpointOnScreen.x > (Screen.width * LeftBorderRatio) && checkpointOnScreen.x < (Screen.width - Screen.width * RightBorderRatio)
            && checkpointOnScreen.y > (Screen.height * DownBorderRatio) && checkpointOnScreen.y < (Screen.height - Screen.height * UpBorderRatio));
    }

    public bool IsCameraRestartPointIn()
    {
        Vector2 resetpointOnCamera = cam.WorldToScreenPoint(CameraRestartPoint.position);
        return (resetpointOnCamera.x > 0 && resetpointOnCamera.x < Screen.width && resetpointOnCamera.y > 0 && resetpointOnCamera.y < Screen.height);
    }

    public virtual void SetIsActivated(bool respawnPlayers = true)
    {
        isActivated = true;
        if (SpriteRend != null)
        {
            SpriteRend.color = ColorActivated;
        }
        soundModule.PlayOneShot("CheckPointOn", gameObject);

        if (RespawnEveryoneWhenActivated && respawnPlayers)
        {
            List<Player> players = GameManager.Instance.GetPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].GetCurrentCharacter() == null)
                {
                    players[i].Respawn();
                }
            }
        }
        CheckpointActivatedEvent.Invoke(this);
    }
}
