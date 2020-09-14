using UnityEngine;

public class HandTryOn : MonoBehaviour {

    //Debug
    bool previousDebugBlockHand = false;
    [Tooltip("Locks hand when hooked")]
    public bool debugBlockHand = false;

    public bool IsLeft = false;
    public float ForceMovement = 150f;
    [Tooltip("Vibration on left motor when hand starts grabbing something")]
    public float VibrationForce = 1f;
    [Tooltip("Vibration duration when starting to grab something")]
    public float GrabVibrationDuration = 0.5f;

    Rigidbody2D rb;
    FixedJoint2D hookJoint;
    CharacterPresetTryOn parentCharacter;
    Animator animEffects;
    Collider2D colliderHand;

    [Tooltip("Number of colliders checked in trigger when handling Hook")]
    public int NbCollidersBuffer = 10;
    Collider2D[] collidersInsideHand = null;
    ContactFilter2D contactFilterHand = new ContactFilter2D();

    public GameObject GrabFXPrefab = null;

    GameObject hook = null;
    bool isHooked = false;
    bool isPointing = false;
    bool isClosed = false;

    public bool IsForcePointing = true;
    public bool IsForceClosed = true;

    AAccessory accessory = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        parentCharacter = GetComponentInParent<CharacterPresetTryOn>();
        animEffects = GetComponentInChildren<Animator>();

        colliderHand = GetComponent<Collider2D>();
        collidersInsideHand = new Collider2D[NbCollidersBuffer];
        contactFilterHand.useTriggers = true;
        contactFilterHand.useLayerMask = true;
        contactFilterHand.SetLayerMask(LayerMask.GetMask("Default", "UI"));
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

    public CharacterPresetTryOn GetParentCharacter()
    {
        return parentCharacter;
    }

    public FixedJoint2D GetHook()
    {
        return hookJoint;
    }

    public bool GetIsHooked()
    {
        return isHooked;
    }

    public void StartHook()
    {
        Destroy(hookJoint);
        collidersInsideHand = new Collider2D[NbCollidersBuffer];
        colliderHand.OverlapCollider(contactFilterHand, collidersInsideHand);

        for (int i = 0; i < collidersInsideHand.Length; ++i)
        {
            if (collidersInsideHand[i] != null && collidersInsideHand[i].CompareTag("Grab"))
            {
                hook = collidersInsideHand[i].gameObject;
                break;
            }
        }

        if (hook != null)
        {
            hookJoint = hook.AddComponent<FixedJoint2D>();
            hookJoint.connectedBody = rb;

            isHooked = true;

            parentCharacter.GrabVibrate(IsLeft ? Character.eControllerMotors.LEFT : Character.eControllerMotors.RIGHT);
            parentCharacter.GetSoundModule().PlayOneShot("Grab1", transform.position);

            Instantiate(GrabFXPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 359f)));
        }
    }

    public void UnHook()
    {
        Destroy(hookJoint);
        hook = null;
        isHooked = false;
    }

    public void UseAccessory()
    {
        if (accessory != null)
        {
            accessory.OnUse();
        }
    }

    public void StopUseAccessory()
    {
        if (accessory != null)
        {
            accessory.OnStopUse();
        }
    }

    public void AddNewAccessory(AAccessory newAccessory)
    {
        animEffects.SetTrigger("Grab");
        accessory = newAccessory;
        accessory.OnStart(this);
    }

    public bool HasAccessory()
    {
        return accessory != null;
    }

    public void RemoveCurrentAccessory()
    {
        if (accessory != null)
        {
            accessory.OnStop();
            Destroy(accessory.gameObject);
        }
    }

    public bool GetIsPointing()
    {
        return isPointing;
    }

    public void SetIsPointing(bool state)
    {
        isPointing = state;
    }

    public bool GetIsClosed()
    {
        return isClosed;
    }

    public void SetIsClosed(bool state)
    {
        isClosed = state;
    }

    public Animator GetEffectAnimator()
    {
        return animEffects;
    }
}
