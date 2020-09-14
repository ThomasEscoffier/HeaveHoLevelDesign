using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hand : MonoBehaviour 
{
    [SerializeField]
    Transform handVisualisation;
    [SerializeField]
    SpriteRenderer handTintRenderer, handSkinRenderer;

    public Transform HandVisualisation => handVisualisation;
    
    //Debug
    bool previousDebugBlockHand = false;
    [Tooltip("Locks hand when hooked")]
    public bool debugBlockHand = false;

    public bool IsLeft = false;

    public Sprite HandOpenSprite;
    public Sprite HandCloseSprite;
    public Sprite HandPointSprite;

    Sprite handSkinOpenSprite;
    Sprite handSkinClosedSprite;
    Sprite handSkinPointSprite;

    [Tooltip("Velocity needed when colliding other hand to make a clap sound")]
    public float ClapVelocity = 1f;

    Animator animEffects;

    [Tooltip("Time the collider forearm changes size")]
    public float TimeChangeSizeAfterHook = 0.1f;
    float timerAfterHook = 0f;
    bool isChangingSize = false;

    [Tooltip("Size forearm collider when not hooked")]
    public Vector2 ForearmSizeNormal;
    [Tooltip("Offset forearm collider when not hooked")]
    public Vector2 ForearmOffsetNormal;
    [Tooltip("Size forearm collider when hooked")]
    public Vector2 ForearmSizeHooked;
    [Tooltip("Offset forearm collider when not hooked")]
    public Vector2 ForearmOffsetHooked;


    HingeJoint2D rotationJoint;

    CapsuleCollider2D forearmCollider;
    
    [SerializeField]
    Collider2D colliderHand;

    SpriteRenderer handRenderer;

    [Tooltip("Time lerp position hand to touch hook target")]
    public float TimeLerp = 0.1f;
    public float MaxTimeTryLerp = 0.5f;

    float timerLerp = 0f;
    float timerMaxLerp = 0f;
    Rigidbody2D rb;
    FixedJoint2D hookJoint;
    Character parentCharacter = null;

    bool isLerping = false;
    Vector2 startPoint = Vector2.zero;
    Vector2 endPointLocal = Vector2.zero;

    GameObject hook = null;
    bool isHookNew = false;
    bool isPointing = false;
    bool isClosed = false;
    bool lockedWrist = true;
    
    // Hand visualisation's local position to the joint.
    private Vector3 hookedOffset;
    // Hand visualisation's local rotation to the joint
    private Vector3 hookedRightDirection;

    bool waitBeforeEndHook = false;
    float timerBeforeEndHook = 0f;

    [Tooltip("Number of colliders checked in trigger when handling Hook")]
    public int NbCollidersBuffer = 10;
    Collider2D[] collidersInsideHand = null;
    ContactFilter2D contactFilterHand = new ContactFilter2D();

    float timerDisableHand = 0f;
    bool hasTimerDisableHand = false;
    bool isDisabled = false;
    bool isIgnoringOtherCharacters = false;

    public float RadiusRaycastHand = 2f;
    
    private readonly RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[10];

    public GameObject GrabFXPrefab = null;

    AAccessory accessory = null;

    private LayerMask defaultLayerMask, defaultAndCharactersLayerMask;

    private Vector3 startLocalIdlePosition;

    public class HandParamEvent : UnityEvent<Hand> { }
    public HandParamEvent OnHookEvent = new HandParamEvent();

    private DistanceJoint2D distanceJointOnHand;

    public void SetCenterOfMass(Vector2 newCenter)
	{
		rb.centerOfMass = newCenter;
	}

    public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
    {
        rb.AddForce(force, mode);
    }

    public void AddTorque(float force)
    {
        rb.AddTorque(force);
    }

    public void MoveRotation(float angle)
    {
        rb.MoveRotation(angle);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        parentCharacter = transform.parent.GetComponent<Character>();
        handRenderer =  handVisualisation.GetComponent<SpriteRenderer>();
        startLocalIdlePosition = handVisualisation.localPosition;
        rotationJoint = GetComponent<HingeJoint2D>();
        distanceJointOnHand = GetComponent<DistanceJoint2D>();
        animEffects = GetComponentInChildren<Animator>();

        forearmCollider = rotationJoint.connectedBody.GetComponent<CapsuleCollider2D>();
        forearmCollider.size = ForearmSizeNormal;
        forearmCollider.offset = ForearmOffsetNormal;

        collidersInsideHand = new Collider2D[NbCollidersBuffer];
        contactFilterHand.useTriggers = true;
        contactFilterHand.useLayerMask = true;
        
        defaultLayerMask = LayerMask.GetMask("Default");
        defaultAndCharactersLayerMask = LayerMask.GetMask("Default", "Characters");
        
        contactFilterHand.SetLayerMask(defaultAndCharactersLayerMask);
    }

    void Start()
    {
        InitPaintMaterial();
        OpenHand(); // Set defaut sprites/skins    
    }

    void InitPaintMaterial()
    {
        float stencilID = 0;

        stencilID = handRenderer.material.GetFloat("_Stencil");
        handRenderer.material.SetFloat("_Stencil", stencilID + (parentCharacter.GetPlayer().OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);

        stencilID = handSkinRenderer.material.GetFloat("_Stencil");
        handSkinRenderer.material.SetFloat("_Stencil", stencilID + (parentCharacter.GetPlayer().OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);

        SpriteRenderer armRenderer = GetArm().GetComponent<SpriteRenderer>();
        stencilID = armRenderer.material.GetFloat("_Stencil");
        armRenderer.material.SetFloat("_Stencil", stencilID + (parentCharacter.GetPlayer().OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);

        SpriteRenderer armSkinRenderer = GetArm().transform.GetChild(1).GetComponent<SpriteRenderer>();
        stencilID = armSkinRenderer.material.GetFloat("_Stencil");
        armSkinRenderer.material.SetFloat("_Stencil", stencilID + (parentCharacter.GetPlayer().OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);

        SpriteRenderer forearmRenderer = GetArm().Forearm.GetComponent<SpriteRenderer>();
        stencilID = forearmRenderer.material.GetFloat("_Stencil");
        forearmRenderer.material.SetFloat("_Stencil", stencilID + (parentCharacter.GetPlayer().OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);

        SpriteRenderer forearmSkinRenderer = GetArm().Forearm.transform.GetChild(1).GetComponent<SpriteRenderer>();
        stencilID = forearmSkinRenderer.material.GetFloat("_Stencil");
        forearmSkinRenderer.material.SetFloat("_Stencil", stencilID + (parentCharacter.GetPlayer().OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
    }

    void Update()
    {
        if (isDisabled && hasTimerDisableHand)
        {
            timerDisableHand = Mathf.Max(timerDisableHand - Time.deltaTime, 0f);
            if (timerDisableHand <= 0f)
            {
                isDisabled = false;
            }
        }

        if (waitBeforeEndHook)
        {
            timerBeforeEndHook = Mathf.Max(timerBeforeEndHook - Time.deltaTime, 0f);
            if (timerBeforeEndHook == 0)
            {
                HookAfterLerp();
            }
        }

        if (!lockedWrist && hook != null)
        {
            // while the joint is in action update the hand visualisation to preserve its local position to the joint.
            handVisualisation.right = rotationJoint.transform.TransformDirection(hookedRightDirection);
            handVisualisation.position = rotationJoint.transform.TransformPoint(hookedOffset);
        }
        else
        {
            handVisualisation.localRotation = Quaternion.identity;
            handVisualisation.localPosition = startLocalIdlePosition;
        }
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            if (hook == null)
            {
                isLerping = false;
            }
            else
            {
                Vector2 pos = Vector2.Lerp(startPoint, hook.transform.TransformPoint(endPointLocal), timerLerp / TimeLerp);
                rb.MovePosition(pos);
                timerLerp = Mathf.Min(timerLerp + Time.fixedDeltaTime, TimeLerp);
                timerMaxLerp = Mathf.Min(timerMaxLerp + Time.fixedDeltaTime, MaxTimeTryLerp);
                if (timerMaxLerp >= MaxTimeTryLerp)
                {
                    isLerping = false;
                    timerLerp = 0f;
                    timerMaxLerp = 0f;
                    hook = null;
                }
            }
            HandleEndLerp();
        }

        if (isChangingSize)
        {
            if (forearmCollider.size == ForearmSizeNormal)
            {
                isChangingSize = false;
                timerAfterHook = 0f;
                return;
            }
            Vector2 currentSize = Vector2.Lerp(ForearmSizeHooked, ForearmSizeNormal, timerAfterHook / TimeChangeSizeAfterHook);
            Vector2 currentOffset = Vector2.Lerp(ForearmOffsetHooked, ForearmOffsetNormal, timerAfterHook / TimeChangeSizeAfterHook);
            forearmCollider.size = currentSize;
            forearmCollider.offset = currentOffset;
            timerAfterHook = Mathf.Min(timerAfterHook + Time.fixedDeltaTime, TimeChangeSizeAfterHook);
            if (timerAfterHook >= TimeChangeSizeAfterHook)
            {
                isChangingSize = false;
                timerAfterHook = 0f;
            }
        }
    }

    void LateUpdate()
    {
        //Hand can stay hooked on debug
        if (previousDebugBlockHand != debugBlockHand)
        {
            if (!debugBlockHand && GetIsHooked() && !parentCharacter.IsPlayerGrabbing(IsLeft))
            {
                UnHook();
            }
        }
        previousDebugBlockHand = debugBlockHand;
    }

    public Character GetParentCharacter()
    {
        return parentCharacter;
    }

    public Arm GetArm()
    {
        return IsLeft ? parentCharacter.LeftArm : parentCharacter.RightArm;
    }

    public void SetHandOutfit(Sprite handOpen, Sprite handClosed, Sprite handPoint)
    {
        handSkinOpenSprite = handOpen;
        handSkinClosedSprite = handClosed;
        handSkinPointSprite = handPoint;

        if (isClosed)
        {
            handSkinRenderer.sprite = handSkinClosedSprite;
        }
        else if (isPointing)
        {
            handSkinRenderer.sprite = handSkinPointSprite;
        }
        else
        {
            handSkinRenderer.sprite = handSkinOpenSprite;
        }
    }

    public void SetHandOutfitColor(Color newColor)
    {
        handSkinRenderer.color = newColor;
        handSkinRenderer.color = newColor;
    }

    public void SetHandColor(Color newColor)
    {
        handRenderer.color = newColor;
    }

    public void SetTint(Color newTint, Color newBodyTint)
    {
        handTintRenderer.color = newBodyTint;
        handSkinRenderer.color = newTint;

        handTintRenderer.color = newBodyTint;
        handSkinRenderer.color = newTint;
    }

    public bool GetIsPointing()
    {
        return isPointing;
    }

    public bool GetIsClosed()
    {
        return isClosed;
    }

    public void Point()
    {
        handRenderer.sprite = HandPointSprite;
        handTintRenderer.sprite = HandPointSprite;
        handSkinRenderer.sprite = handSkinPointSprite;       
        handTintRenderer.sprite = HandPointSprite;
        handSkinRenderer.sprite = handSkinPointSprite;
        isPointing = true;
        if (accessory != null)
        {
            accessory.OnUse();
        }
    }

    public void SetIsPointing(bool state)
    {
        isPointing = state;
    }

    public void OpenHand()
    {
        handRenderer.sprite = HandOpenSprite;
        handTintRenderer.sprite = HandOpenSprite;
        handSkinRenderer.sprite = handSkinOpenSprite;

        if (isPointing && accessory != null)
        {
            accessory.OnStopUse();
        }
        isPointing = false;
        isClosed = false;
    }

    public void CloseHand()
    {
        handRenderer.sprite = HandCloseSprite;
        handTintRenderer.sprite = HandCloseSprite;
        handSkinRenderer.sprite = handSkinClosedSprite;

        isClosed = true;
    }

    public void SetHandVisible(bool state, bool sameForSkin = true)
    {
        handRenderer.enabled = state;
        if (sameForSkin)
        {
            handTintRenderer.enabled = state;
            handSkinRenderer.enabled = state;
        }
    }

    public bool GetIsDisabled()
    {
        return isDisabled;
    }

    public void DisableHand(float timer = 0f) // If timer == 0 -> no timer
    {
        hasTimerDisableHand = timer > 0f;
        isDisabled = true;
        timerDisableHand = timer;
    }

    public void EnableHand()
    {
        isDisabled = false;
    }

    public void SetIsIgnoringOtherCharacters(bool state)
    {
        isIgnoringOtherCharacters = state;
        if (isIgnoringOtherCharacters)
        {
            contactFilterHand.SetLayerMask(LayerMask.GetMask("Default"));
        }
        else
        {
            contactFilterHand.SetLayerMask(LayerMask.GetMask("Default", "Characters"));
        }
    }

    public FixedJoint2D GetHook()
    {
        return hookJoint;
    }

    public bool GetIsHooked()
    {
        return hook != null && hookJoint != null;
    }

    //Moves hand if needed to touch target
    public void StartHook(RaycastHit2D target)
    {
        if (waitBeforeEndHook)
            return;

        //CloseHand();
        if (target.collider.gameObject != null)
        {
            hook = target.collider.gameObject;
            Hand holdingHand = hook.GetComponent<Hand>();
            if (holdingHand != null && holdingHand.GetIsHooked() && holdingHand.GetHook().gameObject == gameObject)
            {
                HookAfterLerp();
            }
            else
            {
                StartLerp(target.point);
            }
        }
    }
    
    //Stops lerp
    public void CancelHooking()
    {
        OpenHand();
        isLerping = false;
        timerLerp = 0f;
        timerMaxLerp = 0f;
        hook = null;
        waitBeforeEndHook = false;

        if (hookJoint != null)
        {
            Destroy(hookJoint);
        }
    }

    void HookAfterLerp()
    {
        if (!waitBeforeEndHook)
        {
            if (HandleNewHookJointCollectible())
            {
                waitBeforeEndHook = true;
                return;
            }
        }
        waitBeforeEndHook = false;
        FinishHook();
    }

    public void UnlockWrist()
    {
        lockedWrist = false;
        // Cache the local position of the hand visualisation to the joint.
        hookedOffset = rotationJoint.transform.InverseTransformPoint(handVisualisation.position);
        hookedRightDirection = rotationJoint.transform.InverseTransformDirection(handVisualisation.right);
        rotationJoint.useMotor = false;
    }

    void FinishHook()
    {
        hookJoint = hook.AddComponent<FixedJoint2D>();
        hookJoint.connectedBody = rb;

        isHookNew = true;

        Toy toy = hook.GetComponent<Toy>();
        if (toy == null || !toy.lockHandRotation)
        {
            UnlockWrist();
        }

        if (hook.CompareTag("Grab"))
        {
            forearmCollider.size = ForearmSizeHooked;
            forearmCollider.offset = ForearmOffsetHooked;
        }

        //SetIsTrigger(false);
        OnHookEvent.Invoke(this);
        HandleNewHookJointPlatform();
        HandleNewHookJointHelpObject();
        HandleNewHookJointPass();
        //HandleNewHookJointCollectible();
        //HandleNewHookJointHand();
        GameManager.Instance.GetChainsManager().AddHookToChain(GetArm(), hook);

        parentCharacter.HandleEffortHook(this);
        parentCharacter.GrabVibrate(IsLeft ? Character.eControllerMotors.LEFT : Character.eControllerMotors.RIGHT);
        parentCharacter.GetSoundModule().PlayOneShot("Grab1", transform.position);
        /*if (parentCharacter.GetPlayer().GetControls().GetControlsMode() == PlayerControls.eControlsMode.MOUSE)
        {
            parentCharacter.SetPreviousMousePos(parentCharacter.GetPlayer().GetControls().GetPlayerMouseInput().screenPosition);
        }*/

        Instantiate(GrabFXPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 359f)));
    }

    void HandleNewHookJointPlatform()
    {
        MovingPlatform movingPlatform = hook.GetComponent<MovingPlatform>();
        /*if (movingPlatform != null)
        {
            movingPlatform.AddCharacterJoint(hookJoint);
        }*/
    }

    bool HandleNewHookJointCollectible()
    {
        RespawnableItem hookedCollectible = hook.GetComponent<RespawnableItem>();
        if (hookedCollectible != null)
        {
            timerBeforeEndHook = hookedCollectible.TimeBeforeEndHook;
            hookedCollectible.StartIsTaken(this);
            return true;
        }
        return false;
    }

    bool HandleNewHookJointHelpObject()
    {
        RandomHelpObject hookedRandomHelpObject = hook.GetComponent<RandomHelpObject>();
        if (hookedRandomHelpObject != null)
        {
            hookedRandomHelpObject.StartIsTaken(this);
            return true;
        }
        return false;
    }

    void HandleNewHookJointHand()
    {
        Hand hookedHand = hook.GetComponent<Hand>();
        if (hookedHand != null)
        {
            if (hookedHand.GetIsHooked() && hookedHand.GetHook() == gameObject)
            {
                SetIsTrigger(true);
            }
            else
            {
                SetIsTrigger(false);
            }
        }
    }

    void HandleNewHookJointPass()
    {
        if (hook == null)
            return;
    }

    public bool GetIsHookNew()
    {
        return isHookNew;
    }

    public void ConsumeNewHook()
    {
        isHookNew = false;
    }

    public void UnHook()
    {
        //In case the previous hook didn't reach its destination
        isLerping = false;
        timerLerp = 0f;
        timerMaxLerp = 0f;

        //SetIsTrigger(true);
        HandleOldHookJointCollectible();
        HandleOldHookJointHelpObject();
        HandleOldHookJointPass();
        //HandleOldHookJointHand();
        parentCharacter.HandleEffortUnhook(this);
        GameObject previousHook = hook;
        if (hook != null)
        {
            GameManager.Instance.GetChainsManager().RemoveHookToChain(GetArm(), hook);
        }

        //DestroyImmediate(hookJoint);
        Destroy(hookJoint);
        hook = null;

        LockWrist();

        isChangingSize = true;

        OpenHand();
    }

    public void LockWrist()
    {
        lockedWrist = true;
        // enabling this seems to stop the rotation of the joint.
        rotationJoint.useMotor = true;
    }

    void HandleOldHookJointCollectible()
    {
        if (hook == null)
            return;

        RespawnableItem hookedCollectible = hook.GetComponent<RespawnableItem>();
        if (hookedCollectible != null)
        {
            hookedCollectible.StopIsTaken(this);
        }
    }

    void HandleOldHookJointHelpObject()
    {
        if (hook == null)
            return;

        RandomHelpObject hookedHelpObject = hook.GetComponent<RandomHelpObject>();
        if (hookedHelpObject != null)
        {
            hookedHelpObject.StopIsTaken(this);
        }
    }

    void HandleOldHookJointHand()
    {
        if (hook == null)
            return;

        Hand hookedHand = hook.GetComponent<Hand>();
        if (hookedHand != null && hookedHand.GetIsHooked() && hookedHand.GetHook() == gameObject)
        {
            hookedHand.SetIsTrigger(false);
        }
    }

    void HandleOldHookJointPass()
    {
        if (hook == null)
            return;
    }

    public void SetIsTrigger(bool state)
    {
        colliderHand.isTrigger = state;
    }

    public bool GetIsTrigger()
    {
        return colliderHand.isTrigger;
    }

    public void StartLerp(Vector2 target)
    {
        startPoint = transform.position;
        endPointLocal = hook.transform.InverseTransformPoint(target);
        isLerping = true;
    }
    
    public RaycastHit2D GetClosestHook()
    {
        RaycastHit2D closestTarget = new RaycastHit2D();

        if (isLerping)
            return closestTarget;

        float distance = Mathf.Infinity;
        var cachedTransform = handVisualisation;
        var size = Physics2D.CircleCastNonAlloc(cachedTransform.position, RadiusRaycastHand, cachedTransform.forward, raycastHit2Ds, float.MaxValue, isIgnoringOtherCharacters ? defaultLayerMask : defaultAndCharactersLayerMask);
        for (var i = 0; i < size; i++)
        {
            //Check if current object is already held by other hand
            var hit = raycastHit2Ds[i];
            if (hit.collider.CompareTag("Collectible") || hit.collider.CompareTag("Toy"))
            {
                Hand otherHand = parentCharacter.GetOtherHand(this);
                if (otherHand != null && otherHand.GetIsHooked() && otherHand.GetHook().gameObject == hit.collider.gameObject)
                {
                    continue;
                }
            }
            if (hit.collider.CompareTag("Grab") || hit.collider.CompareTag("Collectible") || hit.collider.CompareTag("Toy") || hit.collider.CompareTag("Ground") || (!isIgnoringOtherCharacters && hit.collider.CompareTag("Character") && !parentCharacter.IsGameObjectFromCharacter(hit.collider.gameObject)))
            {
                float currentDistance = Vector2.Distance(hit.collider.transform.position, cachedTransform.position);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closestTarget = hit;
                }
            }
        }
        return closestTarget;
    }

    /*void OnTriggerEnter2D(Collider2D collider)
    {
        Hand otherHand = parentCharacter.GetOtherHand(this);
        if (otherHand)
        {
            Rigidbody2D otherHandRb = otherHand.GetComponent<Rigidbody2D>();
            if (otherHandRb.gameObject == collider.gameObject && (rb.velocity.magnitude >= ClapVelocity || otherHandRb.velocity.magnitude >= ClapVelocity))
            {
                parentCharacter.GetSoundModule().PlayOneShot("Clap");
            }
        }
    }*/

    void HandleEndLerp()
    {
        colliderHand.OverlapCollider(contactFilterHand, collidersInsideHand);

        bool hasHookInsideTrigger = false;
        for (int i = 0; i < collidersInsideHand.Length; ++i)
        {
            if (collidersInsideHand[i] != null && collidersInsideHand[i].gameObject == hook)
            {
                hasHookInsideTrigger = true;
                break;
            }
        }

        if (hook == null || hasHookInsideTrigger)
        {
            timerLerp = 0f;
            timerMaxLerp = 0f;
            isLerping = false;
            if (hook != null)
            {
                HookAfterLerp();
            }
        }
    }

    public void AddNewAccessory(AAccessory newAccessory)
    {
        accessory = newAccessory;
        accessory.OnStart(this);
    }

    public bool HasAccessory()
    {
        return accessory != null;
    }

    public void RemoveCurrentAccessory()
    {
        accessory.OnStop();
        Destroy(accessory.gameObject);
    }

    public Animator GetEffectAnimator()
    {
        return animEffects;
    }

    public void DestroyDistanceJointOnHand()
    {
        Destroy(distanceJointOnHand);
    }
}
