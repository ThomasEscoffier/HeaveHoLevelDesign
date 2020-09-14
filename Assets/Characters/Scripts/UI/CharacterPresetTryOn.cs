using UnityEngine;
using Rewired;

public class CharacterPresetTryOn : MonoBehaviour
{
    public Color BodyColor;

    SpriteRenderer headSpriteRenderer;

    HandTryOn leftHand;
    HandTryOn rightHand;

    Transform leftArmTransform;
    Transform leftForearmTransform;
    Rigidbody2D leftHandRb;
    Rigidbody2D rightHandRb;
    Transform rightForearmTransform;
    Transform rightArmTransform;

    public Sprite LeftHandSprite;
    public Sprite LeftHandClosedSprite;
    public Sprite LeftHandPointSprite;
    public Sprite LeftForearmSprite;
    public Sprite LeftArmSprite;

    SpriteRenderer LeftHandSkin;
    SpriteRenderer LeftHandBase;
    SpriteRenderer LeftForearm;
    SpriteRenderer LeftArm;

    public Sprite RightHandSprite;
    public Sprite RightHandClosedSprite;
    public Sprite RightHandPointSprite;
    public Sprite RightForearmSprite;
    public Sprite RightArmSprite;

    SpriteRenderer RightHandSkin;
    SpriteRenderer RightHandBase;
    SpriteRenderer RightForearm;
    SpriteRenderer RightArm;

    public Sprite HairSprite;
    public Sprite GlassesSprite;
    public Sprite FacialFeaturesSprite;

    SpriteRenderer Hair;
    SpriteRenderer Glasses;
    SpriteRenderer FacialFeatures;

    Sprite HandBaseSprite;
    Sprite HandBaseClosedSprite;
    Sprite HandBasePointSprite;

    public Sprite BackSprite;

    SpriteRenderer Back;

    bool isLeftHandVisible = true;
    bool isRightHandVisible = true;
    bool isFaceVisible = true;

    Animator anim = null;
    SpriteRenderer animSpriteRenderer;
    CharacterSoundModule soundModule = null;

    public bool IsForcedControls = false;
    public SpriteRenderer[] SpritesToColor;

    public virtual void ListenPlayerId(int playerId)
    {
        PlayerId = playerId;
        PlayerInput = ReInput.players.GetPlayer(playerId);
    }

    public virtual int GetPlayerId()
    {
        return PlayerId;
    }

    public string GetSpriteNameFromLeftHand()
    {
        if (LeftHandSprite)
            return LeftHandSprite.name;
        return "";
    }

    public void SetSpriteLeftHand(string spriteName)
    {
        if (spriteName == "")
        {
            LeftHandSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                LeftHandSprite = GameManager.Instance.GetSelectionMenu().GetHandSprite(spriteName);
            }
            else
            {
                LeftHandSprite = GameManager.Instance.CharacterAssets.GetHandOpen(spriteName);
            }
        }
    }

    public string GetSpriteNameFromLeftHandClosed()
    {
        if (LeftHandClosedSprite)
            return LeftHandClosedSprite.name;
        return "";
    }

    public void SetSpriteLeftHandClosed(string spriteName)
    {
        if (spriteName == "")
        {
            LeftHandClosedSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                LeftHandClosedSprite = GameManager.Instance.GetSelectionMenu().GetHandClosedSprite(spriteName);
            }
            else
            {
                LeftHandClosedSprite = GameManager.Instance.CharacterAssets.GetHandClosed(spriteName);
            }
        }
    }

    public string GetSpriteNameFromLeftHandPoint()
    {
        if (LeftHandPointSprite)
            return LeftHandPointSprite.name;
        return "";
    }

    public void SetSpriteLeftHandPoint(string spriteName)
    {
        if (spriteName == "")
        {
            LeftHandPointSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                LeftHandPointSprite = GameManager.Instance.GetSelectionMenu().GetHandPointSprite(spriteName);
            }
            else
            {
                LeftHandPointSprite = GameManager.Instance.CharacterAssets.GetHandPoint(spriteName);
            }
        }
    }

    public string GetSpriteNameFromLeftForearm()
    {
        if (LeftForearmSprite)
            return LeftForearmSprite.name;
        return "";
    }

    public void SetSpriteLeftForearm(string spriteName)
    {
        if (spriteName == "")
        {
            LeftForearmSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                LeftForearmSprite = GameManager.Instance.GetSelectionMenu().GetForearmSprite(spriteName);
            }
            else
            {
                LeftForearmSprite = GameManager.Instance.CharacterAssets.GetForearm(spriteName);
            }
        }
    }

    public string GetSpriteNameFromLeftArm()
    {
        if (LeftArmSprite)
            return LeftArmSprite.name;
        return "";
    }

    public void SetSpriteLeftArm(string spriteName)
    {
        if (spriteName == "")
        {
            LeftArmSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                LeftArmSprite = GameManager.Instance.GetSelectionMenu().GetArmSprite(spriteName);
            }
            else
            {
                LeftArmSprite = GameManager.Instance.CharacterAssets.GetArm(spriteName);
            }
        }
    }

    public string GetSpriteNameFromRightHand()
    {
        if (RightHandSprite)
            return RightHandSprite.name;
        return "";
    }

    public void SetSpriteRightHand(string spriteName)
    {
        if (spriteName == "")
        {
            RightHandSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                RightHandSprite = GameManager.Instance.GetSelectionMenu().GetHandSprite(spriteName);
            }
            else
            {
                RightHandSprite = GameManager.Instance.CharacterAssets.GetHandOpen(spriteName);
            }
        }
    }

    public string GetSpriteNameFromRightHandClosed()
    {
        if (RightHandClosedSprite)
            return RightHandClosedSprite.name;
        return "";
    }

    public void SetSpriteRightHandClosed(string spriteName)
    {
        if (spriteName == "")
        {
            RightHandClosedSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                RightHandClosedSprite = GameManager.Instance.GetSelectionMenu().GetHandClosedSprite(spriteName);
            }
            else
            {
                RightHandClosedSprite = GameManager.Instance.CharacterAssets.GetHandClosed(spriteName);
            }
        }
    }

    public string GetSpriteNameFromRightHandPoint()
    {
        if (RightHandPointSprite)
            return RightHandPointSprite.name;
        return "";
    }

    public void SetSpriteRightHandPoint(string spriteName)
    {
        if (spriteName == "")
        {
            RightHandPointSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                RightHandPointSprite = GameManager.Instance.GetSelectionMenu().GetHandPointSprite(spriteName);
            }
            else
            {
                RightHandPointSprite = GameManager.Instance.CharacterAssets.GetHandPoint(spriteName);
            }
        }
    }

    public string GetSpriteNameFromRightForearm()
    {
        if (RightForearmSprite)
            return RightForearmSprite.name;
        return "";
    }

    public void SetSpriteRightForearm(string spriteName)
    {
        if (spriteName == "")
        {
            RightForearmSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                RightForearmSprite = GameManager.Instance.GetSelectionMenu().GetForearmSprite(spriteName);
            }
            else
            {
                RightForearmSprite = GameManager.Instance.CharacterAssets.GetForearm(spriteName);
            }
        }
    }

    public string GetSpriteNameFromRightArm()
    {
        if (RightArmSprite)
            return RightArmSprite.name;
        return "";
    }

    public void SetSpriteRightArm(string spriteName)
    {
        if (spriteName == "")
        {
            RightArmSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                RightArmSprite = GameManager.Instance.GetSelectionMenu().GetArmSprite(spriteName);
            }
            else
            {
                RightArmSprite = GameManager.Instance.CharacterAssets.GetArm(spriteName);
            }
        }
    }

    public string GetSpriteNameFromHair()
    {
        if (HairSprite)
            return HairSprite.name;
        return "";
    }

    public void SetSpriteHair(string spriteName)
    {
        if (spriteName == "")
        {
            HairSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                HairSprite = GameManager.Instance.GetSelectionMenu().GetHairSprite(spriteName);
            }
            else
            {
                HairSprite = GameManager.Instance.CharacterAssets.GetHair(spriteName);
            }
        }
    }

    public string GetSpriteNameFromGlasses()
    {
        if (GlassesSprite)
            return GlassesSprite.name;
        return "";
    }

    public void SetSpriteGlasses(string spriteName)
    {
        if (spriteName == "")
        {
            GlassesSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                GlassesSprite = GameManager.Instance.GetSelectionMenu().GetGlassesSprite(spriteName);
            }
            else
            {
                GlassesSprite = GameManager.Instance.CharacterAssets.GetGlasses(spriteName);
            }
        }
    }

    public string GetSpriteNameFromFacialFeatures()
    {
        if (FacialFeaturesSprite)
            return FacialFeaturesSprite.name;
        return "";
    }

    public void SetSpriteFacialFeatures(string spriteName)
    {
        if (spriteName == "")
        {
            FacialFeaturesSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                FacialFeaturesSprite = GameManager.Instance.GetSelectionMenu().GetFacialFeaturesSprite(spriteName);
            }
            else
            {
                FacialFeaturesSprite = GameManager.Instance.CharacterAssets.GetFacialFeatures(spriteName);
            }
        }
    }

    public string GetSpriteNameFromBack()
    {
        if (BackSprite)
            return BackSprite.name;
        return "";
    }

    public void SetSpriteBack(string spriteName)
    {
        if (spriteName == "")
        {
            BackSprite = null;
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                BackSprite = GameManager.Instance.GetSelectionMenu().GetBackSprite(spriteName);
            }
            else
            {
                BackSprite = GameManager.Instance.CharacterAssets.GetBack(spriteName);
            }
        }
    }

    public int PlayerId = -1;
    protected Rewired.Player PlayerInput = null;
    bool isControlBlocked = false;

    string HorizontalLeft = "LeftArmMovementX";
    string VerticalLeft = "LeftArmMovementY";
    string ArmLeft = "GrabLeft";
    string PointLeft = "PointLeft";

    string HorizontalRight = "RightArmMovementX";
    string VerticalRight = "RightArmMovementY";
    string ArmRight = "GrabRight";
    string PointRight = "PointRight";

    Vector2 MoveVectorLeft = Vector2.zero;
    Vector2 MoveVectorRight = Vector2.zero;

    bool isInit = false;

    void Awake()
    {
        if (!isInit)
        {
            Init();
        }

        OnValidate();
    }

    void Init()
    {
        headSpriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        animSpriteRenderer = anim.GetComponent<SpriteRenderer>();
        soundModule = GetComponent<CharacterSoundModule>();

        LeftArm = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
        LeftForearm = transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>();
        LeftHandSkin = transform.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>();
        LeftHandBase = transform.GetChild(3).GetComponent<SpriteRenderer>();
        leftHand = transform.GetChild(3).GetComponent<HandTryOn>();

        RightArm = transform.GetChild(6).GetChild(0).GetComponent<SpriteRenderer>();
        RightForearm = transform.GetChild(5).GetChild(0).GetComponent<SpriteRenderer>();
        RightHandSkin = transform.GetChild(4).GetChild(0).GetComponent<SpriteRenderer>();
        RightHandBase = transform.GetChild(4).GetComponent<SpriteRenderer>();
        rightHand = transform.GetChild(4).GetComponent<HandTryOn>();

        Hair = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        Glasses = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        FacialFeatures = transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();

        leftArmTransform = transform.GetChild(1);
        leftForearmTransform = transform.GetChild(2);
        leftHandRb = transform.GetChild(3).GetComponent<Rigidbody2D>();
        rightHandRb = transform.GetChild(4).GetComponent<Rigidbody2D>();
        rightForearmTransform = transform.GetChild(5);
        rightArmTransform = transform.GetChild(6);

        Back = transform.GetChild(7).GetComponent<SpriteRenderer>();

        HandBaseSprite = GameManager.Instance.CharacterAssets.GetStandard("Main");
        HandBaseClosedSprite = GameManager.Instance.CharacterAssets.GetStandard("Poing");
        HandBasePointSprite = GameManager.Instance.CharacterAssets.GetStandard("PoingPointe");

        isInit = true;
    }

    void Update()
    {
        if (!IsForcedControls && (PlayerInput == null || isControlBlocked))
            return;

        if (IsForcedControls)
        {
            ManageHands(leftHand.IsForceClosed, rightHand.IsForceClosed, leftHand.IsForcePointing, rightHand.IsForcePointing);
        }
        else
        {
            if (Mathf.Abs(PlayerInput.GetAxis(HorizontalLeft)) > Mathf.Epsilon || Mathf.Abs(PlayerInput.GetAxis(VerticalLeft)) > Mathf.Epsilon)
            {
                MoveVectorLeft.x = PlayerInput.GetAxis(HorizontalLeft);
                MoveVectorLeft.y = PlayerInput.GetAxis(VerticalLeft);
                //Debug.Log("Left x : " + MoveVectorLeft.x + " y : " + MoveVectorLeft.y);
            }
            if (Mathf.Abs(PlayerInput.GetAxis(HorizontalRight)) > Mathf.Epsilon || Mathf.Abs(PlayerInput.GetAxis(VerticalRight)) > Mathf.Epsilon)
            {
                MoveVectorRight.x = PlayerInput.GetAxis(HorizontalRight);
                MoveVectorRight.y = PlayerInput.GetAxis(VerticalRight);
                //Debug.Log("Right x : " + MoveVectorRight.x + " y : " + MoveVectorRight.y);
            }

            ManageHands(PlayerInput.GetButton(ArmLeft), PlayerInput.GetButton(ArmRight), PlayerInput.GetButton(PointLeft), PlayerInput.GetButton(PointRight));
        }
    }

    void ManageHands(bool isLeftClosed, bool isRightClosed, bool isLeftPoint, bool isRightPoint)
    {
        //Left Hand Grab
        if (isLeftClosed && !leftHand.GetIsHooked())
        {
            LeftHandSkin.sprite = LeftHandClosedSprite;
            LeftHandBase.sprite = HandBaseClosedSprite;
            leftHand.SetIsClosed(true);
            leftHand.StartHook();
            leftHand.SetIsPointing(false);
        }
        else if (!leftHand.debugBlockHand && isLeftPoint && !leftHand.GetIsPointing() && !isLeftClosed)
        {
            LeftHandSkin.sprite = LeftHandPointSprite;
            LeftHandBase.sprite = HandBasePointSprite;
            leftHand.SetIsPointing(true);
            leftHand.UseAccessory();
        }
        else if (!leftHand.debugBlockHand && !isLeftPoint && !isLeftClosed && (leftHand.GetIsPointing() || leftHand.GetIsClosed()))
        {
            LeftHandSkin.sprite = LeftHandSprite;
            LeftHandBase.sprite = HandBaseSprite;
            leftHand.SetIsClosed(false);
            if (leftHand.GetIsHooked())
            {
                leftHand.UnHook();
            }
            if (leftHand.GetIsPointing())
            {
                leftHand.StopUseAccessory();
                leftHand.SetIsPointing(false);
            }
        }

        //Right Hand Grab
        if (isRightClosed && !rightHand.GetIsHooked())
        {
            RightHandSkin.sprite = RightHandClosedSprite;
            RightHandBase.sprite = HandBaseClosedSprite;
            rightHand.SetIsClosed(true);
            rightHand.StartHook();
            rightHand.SetIsPointing(false);
        }
        else if (!rightHand.debugBlockHand && isRightPoint && !rightHand.GetIsPointing() && !isRightClosed)
        {
            RightHandSkin.sprite = RightHandPointSprite;
            RightHandBase.sprite = HandBasePointSprite;
            rightHand.SetIsPointing(true);
            rightHand.UseAccessory();
        }
        else if (!rightHand.debugBlockHand && !isRightPoint && !isRightClosed && (rightHand.GetIsPointing() || rightHand.GetIsClosed()))
        {
            RightHandSkin.sprite = RightHandSprite;
            RightHandBase.sprite = HandBaseSprite;
            rightHand.SetIsClosed(false);
            if (rightHand.GetIsHooked())
            {
                rightHand.UnHook();
            }
            if (rightHand.GetIsPointing())
            {
                rightHand.StopUseAccessory();
                rightHand.SetIsPointing(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (MoveVectorLeft != Vector2.zero && !leftHand.GetIsHooked())
        {
            leftHandRb.AddForce(MoveVectorLeft * leftHand.ForceMovement);
        }
        if (MoveVectorRight != Vector2.zero && !rightHand.GetIsHooked())
        {
            rightHandRb.AddForce(MoveVectorRight * rightHand.ForceMovement);
        }
    }

    void OnValidate()
    {
        GetComponent<SpriteRenderer>().color = BodyColor;
        for (int i = 0; i < SpritesToColor.Length; ++i)
        {
            if (SpritesToColor[i] != null)
            {
                SpritesToColor[i].color = BodyColor;
            }
        }

        if (LeftArm)
        {
            LeftHandSkin.sprite = LeftHandSprite;
            LeftArm.sprite = LeftArmSprite;
            LeftForearm.sprite = LeftForearmSprite;

            RightHandSkin.sprite = RightHandSprite;
            RightArm.sprite = RightArmSprite;
            RightForearm.sprite = RightForearmSprite;

            Hair.sprite = HairSprite;
            Glasses.sprite = GlassesSprite;
            FacialFeatures.sprite = FacialFeaturesSprite;

            Back.sprite = BackSprite;
        }
    }

    public void SetMoveVectorLeft(Vector2 move)
    {
        MoveVectorLeft = move;
    }

    public void SetMoveVectorRight(Vector2 move)
    {
        MoveVectorRight = move;
    }

    public Vector2 GetMoveVectorLeft()
    {
        return MoveVectorLeft;
    }

    public Vector2 GetMoveVectorRight()
    {
        return MoveVectorRight;
    }

    public void SetIsPointing(bool isLeft, bool state)
    {
        if (isLeft)
        {
            leftHand.SetIsPointing(state);
            if (state)
            {
                LeftHandSkin.sprite = LeftHandPointSprite;
                LeftHandBase.sprite = HandBasePointSprite;
            }
            else
            {
                LeftHandSkin.sprite = LeftHandSprite;
                LeftHandBase.sprite = HandBaseSprite;
            }
        }
        else
        {
            rightHand.SetIsPointing(state);
            if (state)
            {
                RightHandSkin.sprite = RightHandPointSprite;
                RightHandBase.sprite = HandBasePointSprite;
            }
            else
            {
                RightHandSkin.sprite = RightHandSprite;
                RightHandBase.sprite = HandBaseSprite;
            }
        }
    }

    public void LoadOutfit(Character.Outfit outfit)
    {
        BodyColor = outfit.BodyColor;
        HairSprite = outfit.HairSprite;
        GlassesSprite = outfit.GlassesSprite;
        FacialFeaturesSprite = outfit.FacialFeaturesSprite;

        LeftHandSprite = outfit.LeftHandSprite;
        LeftHandClosedSprite = outfit.LeftHandClosedSprite;
        LeftHandPointSprite = outfit.LeftHandPointSprite;
        LeftForearmSprite = outfit.LeftForearmSprite;
        LeftArmSprite = outfit.LeftArmSprite;

        RightHandSprite = outfit.RightHandSprite;
        RightHandClosedSprite = outfit.RightHandClosedSprite;
        RightHandPointSprite = outfit.RightHandPointSprite;
        RightForearmSprite = outfit.RightForearmSprite;
        RightArmSprite = outfit.RightArmSprite;

        BackSprite = outfit.BackSprite;

        SetHandVisible(leftHand, !outfit.IsHideHands, false);
        SetHandVisible(rightHand, !outfit.IsHideHands, false);
        SetFaceVisible(!outfit.IsHideFace);

        OnValidate();
    }

    public void LoadPreset(ArmsPreset arms)
    {
        SetSpriteLeftHand(arms.LeftHandName);
        SetSpriteLeftHandClosed(arms.LeftHandClosedName);
        SetSpriteLeftHandPoint(arms.LeftHandPointName);
        SetSpriteLeftForearm(arms.LeftForearmName);
        SetSpriteLeftArm(arms.LeftArmName);

        SetSpriteRightHand(arms.RightHandName);
        SetSpriteRightHandClosed(arms.RightHandClosedName);
        SetSpriteRightHandPoint(arms.RightHandPointName);
        SetSpriteRightForearm(arms.RightForearmName);
        SetSpriteRightArm(arms.RightArmName);

        SetSpriteBack(arms.BackName);

        SetHandVisible(leftHand, !arms.IsHideHands, false);
        SetHandVisible(rightHand, !arms.IsHideHands, false);

        OnValidate();
    }

    public void LoadPreset(CharacterPreset character)
    {
        if (!isInit)
        {
            Init();
        }

        if (!character.BodyColor.Name.Equals(""))
        {
            BodyColor = new Color(character.BodyColor.r, character.BodyColor.g, character.BodyColor.b, character.BodyColor.a);
        }

        SetSpriteHair(character.HairName);
        SetSpriteGlasses(character.GlassesName);
        SetSpriteFacialFeatures(character.FacialFeaturesName);       
        SetPersonalityAnim(character.Personality);
        LoadPreset(character.Arms);

        SetFaceVisible(!character.IsHideFace);
    }

    public void LoadPreset(ColorPreset color)
    {
        BodyColor = new Color(color.r, color.g, color.b, color.a);
        OnValidate();
    }

    public void RefreshModelDisplay()
    {
        OnValidate();
    }

    public void SetIsStatic(bool isStatic)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Rigidbody2D rb = transform.GetChild(i).GetComponent<Rigidbody2D>();
            if (rb)
            {
                if (isStatic)
                {
                    rb.bodyType = RigidbodyType2D.Static;
                }
                else
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }
    }

    public void SetAnimation(string animName)
    {
        anim.SetBool("Effort1", false);
        anim.SetBool("Effort2", false);
        anim.SetBool("Effort3", false);
        anim.SetBool("Waiting", false);
        anim.SetBool("Falling", false);
        anim.SetBool("BumpCharacter", false);
        anim.SetBool("BumpPlatform", false);
        anim.SetBool("Victory", false);

        switch (animName)
        {
            default:
            case "Idle":    
                break;
            case "StartEffort1":
                anim.SetTrigger("StartEffort1");
                anim.SetBool("Effort1", true);
                break;
            case "StartEffort2":
                anim.SetTrigger("StartEffort2");
                anim.SetBool("Effort2", true);
                break;
            case "Effort3":
                anim.SetTrigger("StartEffort2");
                anim.SetBool("Effort2", true);
                anim.SetBool("Effort3", true);
                break;
            case "StartFall":
                anim.SetTrigger("StartFall");
                anim.SetBool("Falling", true);
                break;
            case "Waiting":
                anim.SetBool("Waiting", true);
                break;
            case "StartBumpCharacter":
                anim.SetTrigger("StartBumpCharacter");
                anim.SetBool("BumpCharacter", true);
                break;
            case "StartBumpPlatform":
                anim.SetTrigger("StartBumpPlatform");
                anim.SetBool("BumpPlatform", true);
                break;
            case "Victory":
                anim.SetBool("Victory", true);
                break;
        }

    }

    public void SetPersonalityAnim(Character.ePersonality personality)
    {
        if (anim != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.GetIsInSelection())
            {
                anim.runtimeAnimatorController = GameManager.Instance.GetSelectionMenu().GetPersonalityAnim(Character.GetPersonalityAnimatorName(personality));
            }
            else
            {
                anim.runtimeAnimatorController =
                    GameManager.Instance.CharacterAssets.GetPersonality(
                        Character.GetPersonalityAnimatorName(personality));
            }
        }
    }

    public bool GetIsControlBlocked()
    {
        return isControlBlocked;
    }

    public void SetIsControlBlocked(bool state)
    {
        isControlBlocked = state;
    }

    public CharacterSoundModule GetSoundModule()
    {
        return soundModule;
    }

    public HandTryOn GetLeftHand()
    {
        return leftHand;
    }

    public HandTryOn GetRightHand()
    {
        return rightHand;
    }

    public Transform GetLeftArmTransform()
    {
        return leftArmTransform;
    }

    public Transform GetLeftForearmTransform()
    {
        return leftForearmTransform;
    }

    public Transform GetRightArmTransform()
    {
        return rightArmTransform;
    }

    public Transform GetRightForearmTransform()
    {
        return rightForearmTransform;
    }

    public void GrabVibrate(Character.eControllerMotors motorIndex)
    {
        foreach (Joystick j in PlayerInput.controllers.Joysticks)
        {
            if (j.supportsVibration)
            {
                switch (motorIndex)
                {
                    case Character.eControllerMotors.LEFT:
                        {
                            j.SetVibration(0, leftHand.VibrationForce, leftHand.GrabVibrationDuration);
                            break;
                        }
                    case Character.eControllerMotors.RIGHT:
                        {
                            j.SetVibration(1, rightHand.VibrationForce, rightHand.GrabVibrationDuration);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }

    public void SetHandVisible(HandTryOn hand, bool isVisible, bool sameForSkin = true)
    {
        if (hand.IsLeft)
        {
            LeftHandBase.enabled = isVisible;
            if (sameForSkin)
            {
                LeftHandSkin.enabled = isVisible;
            }
            isLeftHandVisible = isVisible;
        }
        else
        {
            RightHandBase.enabled = isVisible;
            if (sameForSkin)
            {
                RightHandSkin.enabled = isVisible;
            }
            isRightHandVisible = isVisible;
        }
    }

    public void SetFaceVisible(bool state)
    {
        headSpriteRenderer.enabled = state;
        anim.enabled = state;
        animSpriteRenderer.enabled = state;
        Glasses.enabled = state;
        FacialFeatures.enabled = state;

        isFaceVisible = state;
    }

    public bool IsLeftHandVisible()
    {
        return isLeftHandVisible;
    }

    public bool IsRightHandVisible()
    {
        return isRightHandVisible;
    }

    public bool IsFaceVisible()
    {
        return isFaceVisible;
    }

    public void ChangeColorOutfit(Color newColor)
    {
        LeftHandSkin.color = newColor;
        LeftForearm.color = newColor;
        LeftArm.color = newColor;

        RightHandSkin.color = newColor;
        RightForearm.color = newColor;
        RightArm.color = newColor;

        Hair.color = newColor;
        Glasses.color = newColor;
        FacialFeatures.color = newColor;
    }

    public Sprite GetHandBaseClosedSprite()
    {
        return HandBaseClosedSprite;
    }

    public Sprite GetHandBasePointSprite()
    {
        return HandBasePointSprite;
    }

    public Sprite GetHandBaseOpenSprite()
    {
        return HandBaseSprite;
    }

    public bool IsPlayerGrabbing(bool isLeft)
    {
        return isLeft ? PlayerInput.GetButton(RewiredConsts.Action.Character_GrabLeft) : PlayerInput.GetButton(RewiredConsts.Action.Character_GrabRight);
    }
}
