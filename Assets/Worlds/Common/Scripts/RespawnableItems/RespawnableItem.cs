using System.Collections.Generic;
using UnityEngine;

public class RespawnableItem : MonoBehaviour {

    public enum ItemType
    {
        KEY,
        ANIMAL,
        TOY,
        NONE,
    }

    public GameObject GreyTokenPrefab = null;
    public bool IsFakeToken = false;

    public float SizeRatio = 1f;

    protected SpriteRenderer mainSprite;
    protected Collider2D[] cols;
    //BoxCollider2D col;
    protected Vector2[] colsSizes;
    //Vector2 colSize = Vector2.zero;
    protected SoundModule soundModule;
    protected LevelManager levelManager;

    public ItemType Type = ItemType.KEY;
    public bool IsStaticWhenNotTaken = true;
    public Animator AnimEffectsRespawnPrefab;
    public Animator AnimEffectsValidatePrefab;
    public Animator Anim;

    Animator currentAnimEffect = null;

    public GameObject[] ToHide;

    public bool IgnoresLevelManagerLimits = false;
    public bool RespawnsAfterDeath = true;
    public float TimeRespawnEffect = 0.5f;
    float timerRespawnEffect = 0f;
    public float TimeBeforeRespawn = 1f;
    float timerBeforeRespawn = 0f;
    public float TimeDisabledHandAfterDeactivate = 0.1f;

    public float TimeBeforeFallingSound = 0.1f;
    float timerBeforeFallingSound = 0f;

    public float TimeBeforeEndHook = 0.01f;
    float timerBeforeEndHook = 0f;
    bool isChangingSize = false;

    public bool CanRoll = true;

    public float TimeLerpValidate = 0.5f;
    float timerLerpValidate = 0f;
    Vector3 previousPos = Vector3.zero;
    Vector3 endPos = Vector3.zero;
    Quaternion previousRotation = Quaternion.identity;

    bool isRespawnAnimLauched = false;
    bool isOutsideCamera = false;
    bool hasToPlayFallingSound = false;

    public float MassWhenTaken = 1f;
    float mass = 0f;
    int nbHooks = 0;

    public float MinSpeedSoundRolling = 5f;
    public float MaxSpeedSoundRolling = 40f;
    int nbSurface = 0;
    bool isRollingSoundPlaying = false;

    public float TimeBetweenCollisionSound = 1f;
    float timerColSound = 0f;

    const float rollingMinValue = 2f;
    const float rollingMaxValue = 8f;

    bool isLerping = false;
    bool isValidated = false;
    bool hasPlayingValidatingEffect = false;
    bool isRespawning = false;

    protected Vector3 startPosition = Vector3.zero;
    protected Quaternion startRotation = Quaternion.identity;
    protected Rigidbody2D rb;
    protected bool isTaken = false;
    protected bool isInVictoryTrigger = false;
    protected RespawnableItemCheckpoint currentCheckpoint = null;

    public PushOutPlayersInside pushOutTrigger = null;
    public ParticleSystem DestroyParticles = null;

    public virtual void Awake()
    {
        if ((Type == ItemType.KEY || Type == ItemType.ANIMAL) && GameManager.Instance != null)
        {
            if (Type == ItemType.KEY && (GameManager.Instance.GetLevelSelector().GetIsTokenAcquired()))
            { 
                gameObject.SetActive(false);
            }
            else if (!IsFakeToken && GameManager.Instance.GetSaveManager().IsTokenAlreadyAcquiredInCurrentLevel())
            {
                Instantiate(GreyTokenPrefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }

        rb = GetComponent<Rigidbody2D>();
        mass = rb.mass;

        mainSprite = GetComponent<SpriteRenderer>();
        cols = GetComponents<Collider2D>();
        colsSizes = new Vector2[cols.Length];
        soundModule = GetComponent<SoundModule>();
        levelManager = FindObjectOfType<LevelManager>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        soundModule.InitEvent("Roll");
        soundModule.AddParameter("Roll", "Speed", 0f);

        for (int i = 0; i < cols.Length; ++i)
        {
            CapsuleCollider2D capsule = cols[i] as CapsuleCollider2D;
            BoxCollider2D box = cols[i] as BoxCollider2D;
            CircleCollider2D circle = cols[i] as CircleCollider2D;
            if (capsule != null)
            {
                colsSizes[i] = capsule.size * SizeRatio;
            }
            else if (box != null)
            {
                colsSizes[i] = box.size * SizeRatio;
            }
            else if (circle != null)
            {
                colsSizes[i].x = circle.radius * SizeRatio;
            }

            if (IsStaticWhenNotTaken)
            {
                cols[i].isTrigger = true;
            }
        }
    }

    public virtual void Update()
    {
        if (levelManager.GetVictoryTrigger() == null || !levelManager.GetVictoryTrigger().IsValidated())
        {
            if (isOutsideCamera)
            {
                if (LevelManager.IsObjectInsideCamera(gameObject) && mainSprite.enabled)
                {
                    isOutsideCamera = false;
                    isRespawning = false;
                    for (int i = 0; i < ToHide.Length; ++i)
                    {
                        ToHide[i].SetActive(true);
                    }
                }
            }
            else if (rb.bodyType == RigidbodyType2D.Dynamic && IgnoresLevelManagerLimits ? !LevelManager.IsObjectInsideCamera(gameObject) : levelManager.GetIsOutsideLimit(gameObject))
            {
                if (nbHooks > 0) // Sometimes nbHooks stays at more than 0 even if no hand is holding it
                {
                    if (GetHoldingHands().Count == 0)
                    {
                        nbHooks = 0;
                        HandleStopIsTaken();
                    }
                    else
                    {
                        return;
                    }
                }
                isOutsideCamera = true;
                isRespawning = true;
                timerBeforeRespawn = 0f;
                for (int i = 0; i < ToHide.Length; ++i)
                {
                    ToHide[i].SetActive(false);
                }
            }
        }

        if (!isTaken)
        {
            if (hasToPlayFallingSound)
            {
                if (timerBeforeFallingSound >= TimeBeforeFallingSound)
                {
                    hasToPlayFallingSound = false;
                    soundModule.PlayOneShot("Fall", gameObject);
                }
                else
                {
                    timerBeforeFallingSound = Mathf.Min(timerBeforeFallingSound + Time.deltaTime, TimeBeforeFallingSound);
                }
            }
        }

        if (isRespawning)
        {
            HandleTryRespawn();
        }
        else if (isLerping)
        {
            timerLerpValidate = Mathf.Min(timerLerpValidate + Time.deltaTime, TimeLerpValidate);
            transform.position = Vector3.Lerp(previousPos, endPos, timerLerpValidate / TimeLerpValidate);
            transform.rotation = Quaternion.Lerp(previousRotation, Quaternion.identity, timerLerpValidate / TimeLerpValidate);
            if (timerLerpValidate == TimeLerpValidate)
            {
                isLerping = false;
                StartValidateAnimEffect();
            }
        }
        else if (isValidated)
        {
            if (AnimEffectsValidatePrefab == null || (AnimEffectsValidatePrefab != null && hasPlayingValidatingEffect && currentAnimEffect == null))
            {
                foreach (Collider2D col in cols)
                {
                    col.enabled = false;
                }
                Destroy(gameObject);
                isValidated = false;
            }
        }

        if (CanRoll && nbSurface > 0 && !isTaken)
        {
            UpdateRolling();
        }
        else if (isRollingSoundPlaying)
        {
            isRollingSoundPlaying = false;
            soundModule.StopEvent("Roll");
        }

        timerColSound = Mathf.Min(timerColSound + Time.deltaTime, TimeBetweenCollisionSound);
    }

    public virtual void FixedUpdate()
    {
        if (isChangingSize)
        {
            timerBeforeEndHook = Mathf.Min(timerBeforeEndHook + Time.fixedDeltaTime, TimeBeforeEndHook);
            for (int i = 0; i < cols.Length; ++i)
            {
                CapsuleCollider2D capsule = cols[i] as CapsuleCollider2D;
                BoxCollider2D box = cols[i] as BoxCollider2D;
                CircleCollider2D circle = cols[i] as CircleCollider2D;
                if (capsule != null)
                {
                    capsule.size = Vector2.Lerp(Vector2.zero, colsSizes[i], timerBeforeEndHook / TimeBeforeEndHook);
                }
                else if (box != null)
                {
                    box.size = Vector2.Lerp(Vector2.zero, colsSizes[i], timerBeforeEndHook / TimeBeforeEndHook);
                }
                else if (circle != null)
                {
                    circle.radius = Mathf.Lerp(0f, colsSizes[i].x, timerBeforeEndHook / TimeBeforeEndHook);
                }
            }
            if (timerBeforeEndHook == TimeBeforeEndHook)
            {
                isChangingSize = false;
                timerBeforeEndHook = 0f;
            }
        }
    }

    void UpdateRolling()
    {
        if (Mathf.Abs(rb.angularVelocity) > MinSpeedSoundRolling)
        {
            soundModule.SetParameterValue("Roll", "Speed", Mathf.Clamp(((rb.angularVelocity - MinSpeedSoundRolling) / MaxSpeedSoundRolling) * rollingMaxValue, rollingMinValue, rollingMaxValue));
            soundModule.AttachInstanceTo("Roll", transform, rb);
            if (!isRollingSoundPlaying)
            {
                soundModule.PlayEvent("Roll");
                isRollingSoundPlaying = true;
            }
        }
        else
        {
            soundModule.StopEvent("Roll");
            isRollingSoundPlaying = false;
        }
    }

	public virtual void StartIsTaken(Hand hand)
    {
        if (nbHooks == 0)
        {
            if (IsStaticWhenNotTaken)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                foreach (Collider2D col in cols)
                {
                    col.isTrigger = false;
                }
                isChangingSize = true;
            }
            rb.mass = MassWhenTaken;
            isTaken = true;
            soundModule.PlayOneShot("Grab", gameObject);
            if (GameManager.Instance.GetMusicManager().GetIsDynamic())
            {
                GameManager.Instance.GetMusicManager().SetGrabCollectibleEvent(true);
            }
            hand.LockWrist();

            soundModule.StopEvent("Roll");
            hasToPlayFallingSound = false;
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
        HandleStopIsTaken();
    }

    void HandleStopIsTaken()
    {
        if (nbHooks == 0)
        {
            rb.mass = mass;
            isTaken = false;
            hasToPlayFallingSound = true;
            timerBeforeFallingSound = 0f;
            if (GameManager.Instance.GetMusicManager().GetIsDynamic())
            {
                GameManager.Instance.GetMusicManager().SetGrabCollectibleEvent(false);
            }
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

    public bool GetIsTaken()
    {
        return isTaken;
    }

    public void Deactivate(bool respawn = true)
    {
        mainSprite.enabled = false;
        foreach (Collider2D col in cols)
        {
            col.enabled = false;
        }
        foreach (Hand hand in GetHoldingHands())
        {
            hand.UnHook();
            hand.DisableHand(TimeDisabledHandAfterDeactivate);
        }
        if (nbHooks > 0)
        {
            nbHooks = 0;
            HandleStopIsTaken();
        }

        soundModule.PlayOneShot("Broken");
        hasToPlayFallingSound = false;

        if (respawn)
        {
            for (int i = 0; i < ToHide.Length; ++i)
            {
                ToHide[i].SetActive(false);
            }
            isRespawning = true;
            isOutsideCamera = true;
        }
    }

    public virtual void SpawnDestroyParticles(Vector3 position)
    {
        if (DestroyParticles != null)
        {
            Instantiate(DestroyParticles, position, Quaternion.identity);
        }
    }

    public virtual void Respawn()
    {
        for (int i = 0; i < ToHide.Length; ++i)
        {
            ToHide[i].SetActive(true);
        }
        mainSprite.enabled = true;
        foreach (Collider2D col in cols)
        {
            col.enabled = true;
            if (IsStaticWhenNotTaken)
            {
                col.isTrigger = true;
            }
        }

        isOutsideCamera = false;
        isRespawning = false;
        nbSurface = 0;
        soundModule.StopEvent("Roll");
        soundModule.PlayOneShot("Respawn", gameObject);
    }

    public void SetIsOutsideCamera()
    {
        isOutsideCamera = true;
    }

    public List<Hand> GetHoldingHands()
    {
        List<Hand> hands = new List<Hand>();
        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();

        foreach (FixedJoint2D joint in joints)
        {
            Hand hand = joint.connectedBody.GetComponent<Hand>();
            if (hand != null)
            {
                hands.Add(hand);
            }
        }

        return hands;
    }

    void RespawnAndReset()
    {
        if (pushOutTrigger != null)
        {
            pushOutTrigger.StopCheckTrigger();
        }
        timerBeforeRespawn = 0f;
        Respawn();
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        mainSprite.enabled = false;

        //Sometimes the HandleStopIsTaken function is not called when the token has to respawn
        if (nbHooks > 0)
        {
            nbHooks = 0;
            HandleStopIsTaken();
            hasToPlayFallingSound = false;
        }

        if (IsStaticWhenNotTaken)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void HandleTryRespawn()
    {
        if (pushOutTrigger != null && !pushOutTrigger.GetIsStarted())
        {
            pushOutTrigger.StartCheckTrigger();
        }
        timerBeforeRespawn = Mathf.Min(timerBeforeRespawn + Time.deltaTime, TimeBeforeRespawn);
        if (timerBeforeRespawn == TimeBeforeRespawn)
        {
            if (!RespawnsAfterDeath)
            {
                Destroy(gameObject);
                return;
            }
            if (isRespawnAnimLauched)
            {
                timerRespawnEffect = Mathf.Min(timerRespawnEffect + Time.deltaTime, TimeRespawnEffect);
                if (timerRespawnEffect == TimeRespawnEffect)
                {
                    timerRespawnEffect = 0f;
                    isRespawnAnimLauched = false;
                    RespawnAndReset();
                }
            }
            else
            {
                if (pushOutTrigger != null)
                {
                    if (!pushOutTrigger.GetHasPlayersInside())
                    {
                        ResetPosition();

                        if (AnimEffectsRespawnPrefab != null)
                        {
                            foreach (Collider2D col in cols)
                            {
                                col.enabled = false;
                            }
                            currentAnimEffect = Instantiate(AnimEffectsRespawnPrefab, startPosition + AnimEffectsRespawnPrefab.transform.position, startRotation);
                            isRespawnAnimLauched = true;
                        }
                        else
                        {
                            RespawnAndReset();
                        }
                    }
                }
                else
                {
                    ResetPosition();

                    if (AnimEffectsRespawnPrefab != null)
                    {
                        foreach (Collider2D col in cols)
                        {
                            col.enabled = false;
                        }
                        currentAnimEffect = Instantiate(AnimEffectsRespawnPrefab, startPosition + AnimEffectsRespawnPrefab.transform.position, startRotation);
                        isRespawnAnimLauched = true;
                    }
                    else
                    {
                        RespawnAndReset();
                    }
                }
            }
        }
    }

    public void SetIsInVictoryTrigger(bool state)
    {
        isInVictoryTrigger = state;
    }

    public void SetCurrentCheckpoint(RespawnableItemCheckpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
        startPosition = checkpoint.posRespawnable.position;
        startRotation = checkpoint.posRespawnable.rotation;
    }

    public RespawnableItemCheckpoint GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }

    public bool GetIsStatic()
    {
        return rb.bodyType == RigidbodyType2D.Static;
    }

    public void ValidateItem(Transform LerpPosition = null)
    {
        CanRoll = false;
        List<Hand> hands = GetHoldingHands();
        foreach (Hand hand in hands)
        {
            hand.UnHook();
        }
        gameObject.tag = "Untagged";

        if (LerpPosition != null)
        {
            for (int i = 0; i < cols.Length; ++i)
            {
                cols[i].enabled = false;
            }

            isLerping = true;
            timerLerpValidate = 0f;
            previousPos = transform.position;
            endPos = LerpPosition.position;
            previousRotation = transform.rotation;
        }
        else
        {
            StartValidateAnimEffect();
        }

        if (isRollingSoundPlaying)
        {
            soundModule.StopEvent("Roll");
        }
    }

    void StartValidateAnimEffect()
    {
        for (int i = 0; i < ToHide.Length; ++i)
        {
            ToHide[i].SetActive(false);
        }

        rb.bodyType = RigidbodyType2D.Static;
        if (AnimEffectsValidatePrefab != null)
        {
            if (Anim != null)
            {
                soundModule.PlayOneShot("Success", gameObject);
                Anim.SetTrigger("Validate");
            }
            else
            {
                soundModule.PlayOneShot("Success", gameObject);
                LaunchAnimEffectValidate();
            }
        }
        else
        {
            mainSprite.enabled = false;
        }
        isValidated = true;
    }

    public void LaunchAnimEffectValidate()
    {
        currentAnimEffect = Instantiate(AnimEffectsValidatePrefab, transform.position + AnimEffectsValidatePrefab.transform.position, transform.rotation);
        hasPlayingValidatingEffect = true;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grab"))
        {
            nbSurface++;
            if (timerColSound == TimeBetweenCollisionSound)
            {
                soundModule.PlayOneShot("Impact");
                timerColSound = 0f;
            }
            hasToPlayFallingSound = false;
        }
    }

    public virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grab"))
        {
            nbSurface--;
            if (nbSurface == 0 && !isTaken)
            {
                hasToPlayFallingSound = true;
                timerBeforeFallingSound = 0f;
                if (isRollingSoundPlaying || soundModule.IsPlaying("Roll"))
                {
                    soundModule.StopEvent("Roll");
                    soundModule.PlayOneShot("EndRoll");
                }
            }
        }
    }

    void OnDestroy()
    {

        if(soundModule != null)
            soundModule.TerminateEventInstance("Roll");
    }
}
