using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Rules/Scrolling")]
public class ScrollingRules : ALevelRules
{
    public float TimeBeforeRewind = 1f;
    public float TimeBeforeCameraRestart = 1f;
    float timer = 0f;
    bool isCameraStopped = false;

    protected Animator anim = null;
    protected Victory victory = null;
    protected Checkpoint startCheckpoint = null;
    protected Checkpoint[] checkpoints;

    public float BaseSpeed = 1f;
    public float ScrollingMaxSpeed = 2f;
    public float ScrollingSpeedLerpTimeAcceleration = 1f;
    public float ScrollingSpeedLerpTimeDeceleration = 1f;
    public float RewindSpeed = 2f;
    public float RewindPitch = -2f;

    public string ScrollingParameter = "Area";
    public float MinValue = 0f;
    public float MaxValue = 90f;

    MusicManager musicManager = null;
    AmbianceManager ambianceManager = null;

    List<Player> players = new List<Player>();

    protected bool isRewinding = false;
    protected bool isIntroPlayed = false;

    float timerLerpSpeed = 0f;
    bool isLerpingAcceleration = false;
    bool isLerpingDeceleration = false;
    float oldSpeed = 0f;
    float nextSpeed = 0f;
    float currentSpeed = 0f;

    int nbDeath = 0;

    bool isStartStingerDone = false;
    bool isFirstPlayer = true;

    public override void OnStart()
    {
        base.OnStart();

        isStartStingerDone = false;
        isFirstPlayer = true;

        timer = 0f;
        timerLerpSpeed = 0f;
        oldSpeed = 0f;
        currentSpeed = 0f;
        nextSpeed = 0f;

        isLerpingAcceleration = false;
        isCameraStopped = false;

        isIntroPlayed = false;
        ScrollingCameraTriggers cameraTriggers = FindObjectOfType<ScrollingCameraTriggers>();
        if (cameraTriggers != null)
        {
            cameraTriggers.SetScrollingRules(this);
        }

        if (cameraTriggers != null && cameraTriggers.transform.parent != null)
        {
            anim = cameraTriggers.transform.parent.GetComponent<Animator>();
        }
        if (anim != null)
        {
            anim.SetFloat("Direction", currentSpeed);
        }
        players = GameManager.Instance.GetPlayers();

        checkpoints = FindObjectsOfType<Checkpoint>();

        LevelSelector.eGameMode gameMode = GameManager.Instance.GetLevelSelector().GetCurrentGameMode();

        for (int i = 0; i < checkpoints.Length; ++i)
        {
            if (checkpoints[i].IsFirstCheckpoint)
            {
                checkpoints[i].SetIsActivated(false);
                startCheckpoint = checkpoints[i];
                startCheckpoint.UseDifferentRespawnPoints = gameMode == LevelSelector.eGameMode.VERSUS;

                foreach (RespawnPoint resPoint in startCheckpoint.AssociatedRespawnPoints)
                {
                    if (anim != null)
                    {
                        resPoint.SetIsPaused(true);
                    }
                    resPoint.SpawnOnlyOnce = (gameMode == LevelSelector.eGameMode.VERSUS && anim != null);
                }
            }
            else
            {
                if (gameMode == LevelSelector.eGameMode.VERSUS && anim != null)
                {
                    checkpoints[i].gameObject.SetActive(false);
                }
            }

            // Activate rewind
            if (gameMode == LevelSelector.eGameMode.VERSUS)
            {
                checkpoints[i].SetRewindActivated(false);
            }
            else
            {
                checkpoints[i].SetRewindActivated(anim != null);
            }
        }

        //Shuffle player respawn order at start
        List<Player> playersToInit = new List<Player>(players);
        while (playersToInit.Count > 0)
        {
            Player player = playersToInit[Random.Range(0, playersToInit.Count)];
            InitPlayer(player);
            playersToInit.Remove(player);
        }

        currentSpeed = BaseSpeed;
        victory = FindObjectOfType<Victory>();
        if (gameMode == LevelSelector.eGameMode.VERSUS)
        {
            victory.IsSoloActivated = true;
        }

        if (anim != null)
        {
            musicManager = GameManager.Instance.GetMusicManager();
            ambianceManager = GameManager.Instance.GetAmbianceManager();
            if (musicManager.GetIsDynamic())
            {
                ambianceManager.AddAmbianceParameter(ScrollingParameter);
            }
        }
        nbDeath = 0;
    }

    public override void InitPlayer(Player player)
    {
        base.InitPlayer(player);
        if (startCheckpoint.UseDifferentRespawnPoints)
        {
            player.SetCheckpoint(startCheckpoint, startCheckpoint.AssociatedRespawnPoints.ToArray());
        }
        else
        {
            player.SetCheckpoint(startCheckpoint, startCheckpoint.AssociatedRespawnPoints.ToArray());
        }

        if (player.GetCurrentCharacter() == null)
        {
            player.Respawn(isFirstPlayer);
            if (isFirstPlayer)
            {
                isFirstPlayer = false;
            }
        }
    }

    public override void InitCharacter(Character character)
    {
        if (character.GetPlayer() != null && character.GetPlayer().GetIsAssisted())
        {
            if (character.LeftHand != null)
            {
                character.LeftHand.AddNewAccessory(Instantiate(character.AssistanceLeftHand, character.LeftHand.HandVisualisation));
            }
            if (character.RightHand != null)
            {
                character.RightHand.AddNewAccessory(Instantiate(character.AssistanceRightHand, character.RightHand.HandVisualisation));
            }
        }
        if (!isStartStingerDone && GameManager.Instance.GetMusicManager().GetIsDynamic())
        {
            character.SetIsFirstCharacterToSpawn();
            isStartStingerDone = true;
        }

        character.DeadEvent.AddListener(OnCharacterDeath);
    }

    public override bool CheckIsFinished()
    {
        if (isIntroPlayed)
        {
            if ((GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.VERSUS && anim != null && GameManager.Instance.GetNbPlayersAlive() == 1)
                || (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.VERSUS && anim == null && victory != null && victory.IsValidated())
                || (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.COOP && victory != null && victory.IsValidated())
                || (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.SOLO && victory != null && victory.IsValidated()))
            {
                return true;
            }
        }
        return false;
    }

    public override void OnFinish()
    {
        levelManager.GetSoundModule().PlayOneShot("EndLevel");
    }

    void Rewind()
    {
        if (anim != null)
        {
            isRewinding = true;
            isLerpingAcceleration = true;
            isLerpingDeceleration = false;
            anim.SetFloat("Direction", -RewindSpeed);
            GameManager.Instance.GetMusicManager().ChangeMusicPitch(RewindPitch);
        }
    }

    void StopRewind()
    {
        if (anim != null)
        {
            anim.SetFloat("Direction", 0f);
            GameManager.Instance.GetMusicManager().ChangeMusicPitch(1f);
            isRewinding = false;
        }

        isCameraStopped = true;
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].Respawn();
        }
    }

    public override float GetCurrentScore()
    {
        return Time.time - timeStarted;
    }

    public override void PlayerLeaves(Player player)
    {
        if (player.GetCurrentCharacter() != null)
        {
            player.GetCurrentCharacter().Die();
        }
    }

    public override void OnUpdate()
    {
        if (!isIntroPlayed)
        {
            if (anim == null || anim.GetCurrentAnimatorStateInfo(0).IsName("Scrolling"))
            {
                isIntroPlayed = true;
                foreach (RespawnPoint resPoint in startCheckpoint.AssociatedRespawnPoints)
                {
                    if (anim != null)
                    {
                        resPoint.SetIsPaused(false);
                    }
                }
            }
            else
            {
                return;
            }
        }

        if (isRewinding)
        {
            UpdateRewinding();
        }
        else
        {
            if (CheckRewind())
                return;

            if (isCameraStopped)
            {
                timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeCameraRestart);
                if (timer == TimeBeforeCameraRestart)
                {
                    timer = 0f;
                    currentSpeed = BaseSpeed;
                    anim.SetFloat("Direction", currentSpeed);
                    isCameraStopped = false;
                }
            }
            else if (isLerpingAcceleration)
            {
                timerLerpSpeed = Mathf.Min(timerLerpSpeed + Time.deltaTime, ScrollingSpeedLerpTimeAcceleration);
                currentSpeed = Mathf.Lerp(oldSpeed, nextSpeed, timerLerpSpeed / ScrollingSpeedLerpTimeAcceleration);
                anim.SetFloat("Direction", currentSpeed);
                if (timerLerpSpeed == ScrollingSpeedLerpTimeAcceleration)
                {
                    isLerpingAcceleration = false;
                }
            }
            else if (isLerpingDeceleration)
            {
                timerLerpSpeed = Mathf.Min(timerLerpSpeed + Time.deltaTime, ScrollingSpeedLerpTimeDeceleration);
                currentSpeed = Mathf.Lerp(oldSpeed, nextSpeed, timerLerpSpeed / ScrollingSpeedLerpTimeDeceleration);
                anim.SetFloat("Direction", currentSpeed);
                if (timerLerpSpeed == ScrollingSpeedLerpTimeDeceleration)
                {
                    isLerpingDeceleration = false;
                }
            }
        }

        if (anim != null && musicManager != null && musicManager.GetIsDynamic())
        {
            UpdateMusic();
        }
    }

    bool CheckRewind()
    {
        if (GameManager.Instance.AreAllPlayersDead())
        {
            if (anim != null ? players[0].GetCheckPoint().IsCheckpointInSafeZone() : LevelManager.IsObjectInsideCamera(players[0].GetCheckPoint().AssociatedRespawnPoints[0].transform.position))
            {
                for (int i = 0; i < players.Count; ++i)
                {
                    players[i].Respawn();
                }
            }
            else
            {
                timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeRewind);
                if (timer == TimeBeforeRewind)
                {
                    timer = 0f;
                    Rewind();
                    return true;
                }
            }
        }
        return false;
    }

    void UpdateRewinding()
    {
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] animatorClip = anim.GetCurrentAnimatorClipInfo(0);
        float timerLevel = animatorClip[0].clip.length * animationState.normalizedTime;

        if (players[0].GetCheckPoint().IsCameraRestartPointIn() || timerLevel <= 0f)
        {
            StopRewind();
        }
    }

    void UpdateMusic()
    {
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
        musicManager.SetAreaEvent((animationState.normalizedTime * MaxValue) / 1f);
        ambianceManager.SetAmbianceParameter(ScrollingParameter, (animationState.normalizedTime * MaxValue) / 1f);
    }

    public void SetMaxSpeed()
    {
        isLerpingAcceleration = true;
        isLerpingDeceleration = false;
        oldSpeed = currentSpeed;
        nextSpeed = ScrollingMaxSpeed;
        timerLerpSpeed = 0f;
    }

    public void SetNormalSpeed()
    {
        isLerpingDeceleration = true;
        isLerpingAcceleration = false;
        oldSpeed = ScrollingMaxSpeed;
        nextSpeed = BaseSpeed;
        timerLerpSpeed = 0f;
    }

    public bool GetIsRewinding()
    {
        return isRewinding;
    }

    void OnCharacterDeath(Character character)
    {
        nbDeath++;
    }
}
