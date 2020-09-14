using Managers;
using Rewired;
using System.Collections.Generic;
#if UNITY_SWITCH
using Rewired.Platforms.Switch;
#endif
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour {

    Player player = null;

    [System.Serializable]
    public struct Outfit
    {
        public Color BodyColor;

        public Sprite HairSprite;
        public Sprite GlassesSprite;
        public Sprite FacialFeaturesSprite;

        public Sprite LeftArmSprite;
        public Sprite LeftForearmSprite;
        public Sprite LeftHandSprite;
        public Sprite LeftHandPointSprite;
        public Sprite LeftHandClosedSprite;

        public Sprite RightArmSprite;
        public Sprite RightForearmSprite;
        public Sprite RightHandSprite;
        public Sprite RightHandPointSprite;
        public Sprite RightHandClosedSprite;

        public Sprite BackSprite;

        public ePersonality Personality;
        public RuntimeAnimatorController PersonalityAnim;

        public bool IsRobot;
        public bool IsMask;

        public bool IsHideHands;
        public bool IsHideFace;

        public Outfit(Color BodyColor_, Sprite HairSprite_, Sprite GlassesSprite_, Sprite FacialFeaturesSprite_,
            Sprite LeftArmSprite_, Sprite LeftForearmSprite_, Sprite LeftHandSprite_, Sprite LeftHandPointSprite_, Sprite LeftHandClosedSprite_,
            Sprite RightArmSprite_, Sprite RightForearmSprite_, Sprite RightHandSprite_, Sprite RightHandPointSprite_, Sprite RightHandClosedSprite_, Sprite BackSprite_,
            ePersonality Personality_, RuntimeAnimatorController PersonalityAnim_,
            bool IsRobot_, bool IsMask_,
            bool IsHideHands_, bool IsHideFace_)
        {
            BodyColor = BodyColor_;
            HairSprite = HairSprite_;
            GlassesSprite = GlassesSprite_;
            FacialFeaturesSprite = FacialFeaturesSprite_;
            LeftArmSprite = LeftArmSprite_;
            LeftForearmSprite = LeftForearmSprite_;
            LeftHandSprite = LeftHandSprite_;
            LeftHandPointSprite = LeftHandPointSprite_;
            LeftHandClosedSprite = LeftHandClosedSprite_;
            RightArmSprite = RightArmSprite_;
            RightForearmSprite = RightForearmSprite_;
            RightHandSprite = RightHandSprite_;
            RightHandPointSprite = RightHandPointSprite_;
            RightHandClosedSprite = RightHandClosedSprite_;
            BackSprite = BackSprite_;
            Personality = Personality_;
            PersonalityAnim = PersonalityAnim_;
            IsRobot = IsRobot_;
            IsMask = IsMask_;
            IsHideHands = IsHideHands_;
            IsHideFace = IsHideFace_;
        }

        public bool IsSame(Outfit otherOutfit)
        {
            return BodyColor == otherOutfit.BodyColor && HairSprite == otherOutfit.HairSprite && GlassesSprite == otherOutfit.GlassesSprite && FacialFeaturesSprite == otherOutfit.FacialFeaturesSprite
                && LeftArmSprite == otherOutfit.LeftArmSprite && LeftForearmSprite == otherOutfit.LeftForearmSprite && LeftHandSprite == otherOutfit.LeftHandSprite && LeftHandPointSprite == otherOutfit.LeftHandPointSprite && LeftHandClosedSprite == otherOutfit.LeftHandClosedSprite
                && RightArmSprite == otherOutfit.RightArmSprite && RightForearmSprite == otherOutfit.RightForearmSprite && RightHandSprite == otherOutfit.RightHandSprite && RightHandPointSprite == otherOutfit.RightHandPointSprite && RightHandClosedSprite == otherOutfit.RightHandClosedSprite
                && BackSprite == otherOutfit.BackSprite && Personality == otherOutfit.Personality && IsRobot == otherOutfit.IsRobot && IsMask == otherOutfit.IsMask
                && IsHideHands == otherOutfit.IsHideHands && IsHideFace == otherOutfit.IsHideFace;
        }
    }

    public static Outfit LoadOutfitFromPreset(CharacterPreset preset)
    {        
        ColorPreset colorPreset = preset.BodyColor;
        var characterResources = GameManager.Instance.CharacterAssets;
        Sprite hairSprite = characterResources.GetHair(preset.HairName);
        Sprite glassesSprite = characterResources.GetGlasses(preset.GlassesName);
        Sprite facialFeaturesSprite = characterResources.GetFacialFeatures(preset.FacialFeaturesName);
        Sprite leftArmSprite = characterResources.GetArm(preset.Arms.LeftArmName);
        Sprite leftForearmArmSprite = characterResources.GetForearm(preset.Arms.LeftForearmName);
        Sprite leftHandSprite = characterResources.GetHandOpen(preset.Arms.LeftHandName);
        Sprite leftHandClosedSprite = characterResources.GetHandClosed(preset.Arms.LeftHandClosedName);
        Sprite leftHandPointSprite = characterResources.GetHandPoint(preset.Arms.LeftHandPointName);
        Sprite rightArmSprite = characterResources.GetArm(preset.Arms.RightArmName);
        Sprite rightForearmArmSprite = characterResources.GetForearm(preset.Arms.RightForearmName);
        Sprite rightHandSprite = characterResources.GetHandOpen(preset.Arms.RightHandName);
        Sprite rightHandClosedSprite = characterResources.GetHandClosed(preset.Arms.RightHandClosedName);
        Sprite rightHandPointSprite = characterResources.GetHandPoint(preset.Arms.RightHandPointName);
        Sprite backSprite = characterResources.GetBack(preset.Arms.BackName);
        RuntimeAnimatorController animator = characterResources.GetPersonality(
            "characterpersonalities:" + GetPersonalityAnimatorName(preset.Personality));
        Outfit newOutfit = new Outfit(new Color(colorPreset.r, colorPreset.g, colorPreset.b), hairSprite, glassesSprite, facialFeaturesSprite,
                                    leftArmSprite, leftForearmArmSprite, leftHandSprite, leftHandPointSprite, leftHandClosedSprite,
                                    rightArmSprite, rightForearmArmSprite, rightHandSprite, rightHandPointSprite, rightHandClosedSprite,
                                    backSprite, preset.Personality, animator,
                                    preset.IsRobot, preset.IsMask, preset.Arms.IsHideHands, preset.IsHideFace);
        return newOutfit;
    }

    Outfit currentOutfit;
    //Used to apply an ambiant color on sprites. Changes between levels
    Color currentTint = Color.white;
    Color currentBodyTint = Color.white;
    public SpriteRenderer[] TintSprites;

    //Force vector based on joysticks inputs
    Vector2 MoveVectorLeft = Vector2.zero;
    Vector2 MoveVectorRight = Vector2.zero;

    //Rigidbody2D Head
    Rigidbody2D rb;
    //Rigidbodies all character
    Rigidbody2D[] rbs;
    bool isControlBlocked = false;

    Animator animFace;

    public Hand LeftHand = null;
    public Hand RightHand = null;
    public Head Head = null;
    public Arm LeftArm = null;
    public Arm RightArm = null;

    bool isDead = false;
    List<GameObject> toDestroyWhenDead = new List<GameObject>();

    LevelManager levelManager = null;
    CameraEffects cam = null;
    bool isFirstCharacterSpawned = false;

    bool isInVictoryTrigger = false;
    bool isOutsideCamera = false;
    float timerOutsideCamera = 0f;
    Vector2 lastPositionBeforeOutsideCamera = Vector2.zero;

    public GameObject BumpFXPrefab = null;
    public GameObject DeathFXPrefab = null;
    GameObject currentDeathFX = null;

    public BloodParticles BloodParticlePrefab;
    public BloodParticles BloodParticlePrefabSoft;
    BloodParticles currentParticles = null;

    float offsetPercentXBloodParticlesPos;
    float offsetPercentYBloodParticlesPos;

    [Tooltip("Vibration on left motor when left hand starts grabbing something")]
    public float LeftVibrationForce = 1f;
    [Tooltip("Vibration on right motor when right hand starts grabbing something")]
    public float RightVibrationForce = 1f;

    public float SwitchVibrationScale = 0.5f;
    
    [Tooltip("Vibration duration when starting to grab something")]
    public float GrabVibrationDuration = 0.5f;

    [Tooltip("Vibration when a chain of characters is undergoing tension while grabbing a movable platform")]
    public float CharacterStuckVibration = 0.1f;
    [Tooltip("Vibration when an arm is breaking")]
    public float BreakVibration = 5f;
    [Tooltip("Vibration duration when an arm is breaking")]
    public float BreakVibrationDuration = 0.1f;

    [Tooltip("Vibration when character dying")]
    public float DeathVibrationForce = 5f;
    [Tooltip("Vibration duration when character dying")]
    public float DeathVibrationDuration = 0.1f;

    public enum eControllerMotors
    {
        LEFT = 0,
        RIGHT,
    }

    public float ForceMovement = 50f;
    public float ForcePull = 500f;

    [Tooltip("Distance between the character and the mouse cursor used to interpolate between 0 and 1")]
    public float RadiusSizeForMouseControl = 3f;
    public float MinDistanceBeforeMovingCharacter = 10f;
    public float MaxDistanceBeforeMovingCharacter = 100f;
    Vector2 previousMousePos = Vector2.zero;
    Vector2 refMousePoint = Vector2.zero;
    Vector2 virtualCursorPosition = Vector2.zero;

    /*public float ForceJump = 200f;

    public float JumpConeAngle = 45f;
    public float JumpCooldown = 2f;
    float timerJumpCooldown = 0f;
    bool hasTouchedPlatform = false;
    bool isJumping = false;*/

    public enum ePersonality
    {
        HAPPY,
        WEIRDO,
        GRUMPY,
        SCARED,
    }

    const string happyAnimatorName = "HappyOverride";
    const string weirdoAnimatorName = "WeirdoOverride";
    const string grumpyAnimatorName = "GrumpyOverride";
    const string scaredAnimatorName = "ScaredOverride";

    const string happyPersonalityName = "Happy";
    const string weirdoPersonalityName = "Weirdo";
    const string grumpyPersonalityName = "Grumpy";
    const string scaredPersonalityName = "Scared";

    ePersonality personality = ePersonality.HAPPY;

    enum eFaceAnim
    {
        IDLE,
        WAITING,
        FALL,
        EFFORT1,
        EFFORT2,
        EFFORT3,
        BUMP_CHARACTER,
        BUMP_PLATFORM,
        VICTORY,
        SING,
        LOSE_ARM,
        ELECTRIC,
    }

    eFaceAnim currentAnimState = eFaceAnim.IDLE;
    eFaceAnim previousAnimState = eFaceAnim.IDLE;

    [System.Serializable]
    public struct BumpInfo
    {
        [Tooltip("Intensity of collision needed to launch Bump effects")]
        public float VelocityToLaunch;

        public float ShakeTime;
        public float ShakeFrequency;
        public AnimationCurve ShakeAmplitude;

        public float VibrationIntensity;
        public float VibrationDuration;

        [Tooltip("Force applied on the character")]
        public float ForcePushBack;

        public float FXSize;

        public bool PlayAnimFace;

        [Tooltip("Parameter for sound")]
        public int Intensity;
    }

    public BumpInfo BumpSmall;
    public BumpInfo BumpMedium;
    public BumpInfo BumpHard;
    public BumpInfo BumpHudge;

    [Tooltip("Velocity to reach when new hook to activate effort animation")]
    public float ForceVelocitySoundEffort = 5f;
    [Tooltip("Time to reach to go form EFFORT2 to EFFORT3")]
    public float TimeEffort = 8f;
    float timerEffort = 0f;

    [Tooltip("Time animation Bump before returning to other animations")]
    public float TimeAnimBumpPlatform = 4f;
    float timerAnimBumpPlatform = 0f;
    [Tooltip("Time animation Bump before returning to other animations")]
    public float TimeAnimBumpCharacter = 4f;
    float timerAnimBumpCharacter = 0f;
    [Tooltip("Time animation Lose Arm before returning to other animations")]
    public float TimeAnimLoseArm = 2f;
    float timerAnimLoseArm = 0f;

    [Tooltip("Time character in the air before considering it falling")]
    public float TimeBeforeFall = 1f;
    float timerBeforeFall = 0f;
    bool isCheckingFall = false;
    bool isFalling = false;

    bool isBumped = false;
    bool isUnderTension = false;
    bool isElectric = false;

    [Tooltip("Time character doing nothing before waiting animation")]
    public float TimeNoHook = 10f;
    float timerNoHook = 0f;

    [Tooltip("Timer to avoid spamming Point animation")]
    public float TimeBeforeNextPoint = 3f;
    float timerBeforeNextPoint = 0f;

    [Tooltip("Timer to avoid spamming Face animation")]
    public float TimeBeforeNextFaceEmotion = 3f;
    float timerBeforeNextFaceEmotion = 0f;

    float timerInputImpulse = 0f;
    bool isInputImpulsePressing = false;

    [Tooltip("Release imput impulse time Min")]
    public float ReleaseImpulseTimeMin = 0.1f;
    [Tooltip("Release imput impulse time Max")]
    public float ReleaseImpulseTimeMax = 0.5f;

    public float ReleaseImpulseRadius = 5f;
    [Tooltip("Release from other characters Force impulse on self Min")]
    public float ReleaseImpulseForceOnItselfMin = 10f;
    [Tooltip("Release from other characters Force impulse on self Max")]
    public float ReleaseImpulseForceOnItselfMax = 100f;
    [Tooltip("Release from other characters Force impulse on holding characters Min")]
    public float ReleaseImpulseForceOnOthersMin = 50f;
    [Tooltip("Release from other characters Force impulse on holding characters Max")]
    public float ReleaseImpulseForceOnOthersMax = 500f;
    [Tooltip("Release from other characters cooldown")]
    public float ReleaseImpulseCooldown = 2f;
    float timerReleaseImpulseCoolDown = 0f;
    [Tooltip("Release from other characters vibration Max")]
    public float ReleaseImpulseVibrationMax = 5f;
    [Tooltip("Release from other characters vibration on release input")]
    public float ReleaseImpulseVibrationFinish = 10f;
    [Tooltip("Release from other characters vibration on other characters")]
    public float ReleaseImpulseVibrationOnOthersFinish = 5f;
    [Tooltip("Release from other characters vibration duration")]
    public float ReleaseImpulseVibrationDurationFinish = 0.2f;

    public float ReleaseImpulseDisableHandTime = 0.5f;

    public float ShakeHeadStartFrequence = 0.05f;
    public float ShakeHeadEndFrequence = 0.01f;

    public float ShakeHeadStartAmplitude = 0.01f;
    public float ShakeHeadEndAmplitude = 0.05f;

    [Tooltip("All body parts of the character")]
    public List<BodyPart> Parts = new List<BodyPart>();

    public AAccessory AssistanceLeftHand = null;
    public AAccessory AssistanceRightHand = null;

    CharacterSoundModule soundModule;

    public CharracterParamEvent DeadEvent = new CharracterParamEvent();
    public class CharracterParamEvent : UnityEvent<Character> { }

    bool isPaused = false;

    void Awake()
    {
        animFace = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rbs = GetComponentsInChildren<Rigidbody2D>();
        levelManager = FindObjectOfType<LevelManager>();
        cam = Camera.main.GetComponent<CameraEffects>();
        soundModule = GetComponent<CharacterSoundModule>();
        soundModule.AddParameter("impacts", "ImpactIntensity", 0f);
        soundModule.AddParameter("impacts", "ImpactSurface", 0f);
        soundModule.InitEvent("impacts");
        //timerJumpCooldown = JumpCooldown;

        offsetPercentXBloodParticlesPos = Screen.width * 0.2f;
        offsetPercentYBloodParticlesPos = Screen.height * 0.2f;
    }

    void Start()
    {
        //Use to debug
        if (player == null)
        {
            Player newPlayer = Instantiate(GameManager.Instance.PlayerPrefab);
            newPlayer.ChangePlayerId(GameManager.Instance.GetUnusedPlayerIdInGame());
            newPlayer.GetControls().SetCurrentLayout(PlayerControls.eLayout.ASSISTED);
            if (ReInput.players.GetPlayer(newPlayer.GetControls().GetPlayerId()).controllers.hasKeyboard)
            {
                newPlayer.GetControls().SetControlsMode(PlayerControls.eControlsMode.KEYBOARD);
            }
            newPlayer.GetControls().SetMapEnabled(true, "Character");
            Outfit outfit = LoadOutfitFromPreset(GameManager.Instance.GetCharactersPresets().GetRandomOutfit(GameManager.Instance.GetSaveManager().GameSaveData.UnlockedOutfits));
            InitAppearanceAndPersonality(outfit);
            newPlayer.SetCharacterOutfit(outfit);
            newPlayer.SetCurrentCharacter(this);
            player = newPlayer;
            GameManager.Instance.AddPlayer(newPlayer);
        }

        if (levelManager.LevelRules != null)
        {
            levelManager.LevelRules.InitCharacter(this);
        }

        if (isFirstCharacterSpawned)
        {
            GameManager.Instance.GetMusicManager().LaunchStartMusicEvent();
            isFirstCharacterSpawned = false;
        }

        InitPaintMaterial();
    }

    void InitPaintMaterial()
    {
        float stencilID = 0;

        SpriteRenderer HeadSprite = Head.GetComponent<SpriteRenderer>();
        stencilID = HeadSprite.material.GetFloat("_Stencil");
        HeadSprite.material.SetFloat("_Stencil", stencilID + (player.OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
        stencilID = HeadSprite.material.GetFloat("_Stencil");

        SpriteRenderer Hair = Head.transform.GetChild(0).GetComponent<SpriteRenderer>();
        stencilID = Hair.material.GetFloat("_Stencil");
        Hair.material.SetFloat("_Stencil", stencilID + (player.OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
        stencilID = Hair.material.GetFloat("_Stencil");

        SpriteRenderer Glasses = Head.transform.GetChild(1).GetComponent<SpriteRenderer>();
        stencilID = Glasses.material.GetFloat("_Stencil");
        Glasses.material.SetFloat("_Stencil", stencilID + (player.OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
        stencilID = Glasses.material.GetFloat("_Stencil");

        SpriteRenderer FaceSprite = Head.transform.GetChild(2).GetComponent<SpriteRenderer>();
        stencilID = FaceSprite.material.GetFloat("_Stencil");
        FaceSprite.material.SetFloat("_Stencil", stencilID + (player.OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
        stencilID = FaceSprite.material.GetFloat("_Stencil");

        SpriteRenderer FacialFeatures = Head.transform.GetChild(3).GetComponent<SpriteRenderer>();
        stencilID = FacialFeatures.material.GetFloat("_Stencil");
        FacialFeatures.material.SetFloat("_Stencil", stencilID + (player.OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
        stencilID = FacialFeatures.material.GetFloat("_Stencil");

        SpriteRenderer HeadTint = Head.transform.GetChild(4).GetComponent<SpriteRenderer>();
        stencilID = HeadTint.material.GetFloat("_Stencil");
        HeadTint.material.SetFloat("_Stencil", stencilID + (player.OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
        stencilID = HeadTint.material.GetFloat("_Stencil");
    }

    void DestroyInstancedMaterials()
    {
        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (spriteRenderer.sharedMaterial.name.Contains("Instance"))
            {
                Destroy(spriteRenderer.material);
            }
        }
    }

    public void SetIsFirstCharacterToSpawn()
    {
        isFirstCharacterSpawned = true;
    }

    public bool GetIsFirstCharacterToSpawn()
    {
        return isFirstCharacterSpawned;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }

    public void InitAppearanceAndPersonality(Outfit outfit)
    {
        InitColorCharacter(outfit.BodyColor);
        InitAppearance(outfit);

        LevelProperties levelProperties = GameManager.Instance.GetLevelSelector().GetCurrentLevel();
        if (levelProperties != null)
        {
            ChangeTintCharacter(levelProperties.TintCharacter, levelProperties.TintCharacterBody);
        }
        SetPersonality(outfit.Personality, outfit.IsRobot, outfit.IsMask);
    }

    public void InitAppearance(Outfit outfit, bool keepOldHair = false, bool keepOldGlasses = false, bool keepOldFF = false)
    {
        currentOutfit = outfit;

        SpriteRenderer Hair = Head.transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer Glasses = Head.transform.GetChild(1).GetComponent<SpriteRenderer>();
        SpriteRenderer FacialFeatures = Head.transform.GetChild(3).GetComponent<SpriteRenderer>();

        Head.GetComponent<SpriteRenderer>().enabled = !currentOutfit.IsHideFace;
        Glasses.enabled = !currentOutfit.IsHideFace;
        FacialFeatures.enabled = !currentOutfit.IsHideFace;
        animFace.enabled = !currentOutfit.IsHideFace;
        animFace.GetComponent<SpriteRenderer>().enabled = !currentOutfit.IsHideFace;

        if (!keepOldHair)
        {
            Hair.sprite = outfit.HairSprite;
        }
        if (!keepOldGlasses)
        {
            Glasses.sprite = outfit.GlassesSprite;
        }
        if (!keepOldFF)
        {
            FacialFeatures.sprite = outfit.FacialFeaturesSprite;
        }

        if (LeftArm != null)
        {
            LeftArm.Forearm.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = outfit.LeftForearmSprite;
            LeftArm.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = outfit.LeftArmSprite;
            LeftHand.SetHandOutfit(outfit.LeftHandSprite, outfit.LeftHandClosedSprite, outfit.LeftHandPointSprite);
            LeftHand.SetHandVisible(!outfit.IsHideHands, false);
        }

        if (RightArm != null)
        {
            RightArm.Forearm.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = outfit.RightForearmSprite;
            RightArm.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = outfit.RightArmSprite;
            RightHand.SetHandOutfit(outfit.RightHandSprite, outfit.RightHandClosedSprite, outfit.RightHandPointSprite);
            RightHand.SetHandVisible(!outfit.IsHideHands, false);
        }

        transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = outfit.BackSprite;
    }

    void InitColorCharacter(Color newColor)
    {
        Color bodyColor = newColor;
        Head.GetComponent<SpriteRenderer>().color = bodyColor;
        for (int i = 1; i < transform.childCount - 1; ++i) // Stats at 1 to avoid changing HeadTint
        {
            SpriteRenderer spriteRend = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (spriteRend)
            {
                spriteRend.color = bodyColor;
            }
        }
        if (LeftHand != null)
        {
            LeftHand.SetHandColor(newColor);
        }
        if (RightHand != null)
        {
            RightHand.SetHandColor(newColor);
        }
    }

    public void ChangeTintCharacter(Color newTint, Color newBodyTint)
    {
        currentTint = newTint;
        currentBodyTint = newBodyTint;

        for (int i = 0; i < TintSprites.Length; ++i)
        {
            if (TintSprites[i] != null) // May be null if arm is removed
            {
                TintSprites[i].color = currentBodyTint;
            }
        }

        Head.transform.GetChild(0).GetComponent<SpriteRenderer>().color = currentTint;
        Head.transform.GetChild(1).GetComponent<SpriteRenderer>().color = currentTint;
        Head.transform.GetChild(2).GetComponent<SpriteRenderer>().color = currentTint;
        Head.transform.GetChild(3).GetComponent<SpriteRenderer>().color = currentTint;

        if (LeftHand != null)
        {
            LeftHand.SetTint(currentTint, currentBodyTint);
            LeftArm.Forearm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = currentTint;
            LeftArm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = currentTint;
        }
        if (RightHand != null)
        {
            RightHand.SetTint(currentTint, currentBodyTint);
            RightArm.Forearm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = currentTint;
            RightArm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = currentTint;
        }

        transform.GetChild(7).GetChild(0).GetComponent<SpriteRenderer>().color = currentTint;
    }

    public void ChangeColorOutfit(Color newColor)
    {
        if (Head != null)
        {
            Head.transform.GetChild(0).GetComponent<SpriteRenderer>().color = newColor;
            Head.transform.GetChild(1).GetComponent<SpriteRenderer>().color = newColor;
            Head.transform.GetChild(3).GetComponent<SpriteRenderer>().color = newColor;
        }

        if (LeftHand != null)
        {
            LeftArm.Forearm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = newColor;
            LeftArm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = newColor;
            LeftHand.SetHandOutfitColor(newColor);
        }
        if (RightHand != null)
        {
            RightHand.SetHandOutfitColor(newColor);
            RightArm.Forearm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = newColor;
            RightArm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    public void PutPreviousColor()
    {
        ChangeColorOutfit(currentTint);
    }

    public Outfit GetCurrentOutfit()
    {
        return currentOutfit;
    }

    public CharacterSoundModule GetSoundModule()
    {
        return soundModule;
    }

    public ePersonality GetPersonality()
    {
        return personality;
    }

    public static string GetPersonalityAnimatorName(ePersonality personality)
    {
        string animatorName = "";
        switch (personality)
        {
            case ePersonality.HAPPY:
                animatorName = happyAnimatorName;
                break;
            case ePersonality.WEIRDO:
                animatorName = weirdoAnimatorName;
                break;
            case ePersonality.GRUMPY:
                animatorName = grumpyAnimatorName;
                break;
            case ePersonality.SCARED:
                animatorName = scaredAnimatorName;
                break;
            default:
                break;
        }
        return animatorName;
    }

    public static string GetPersonalityAnimatorUIName(ePersonality personality)
    {
        string animatorName = "";
        switch (personality)
        {
            case ePersonality.HAPPY:
                animatorName = happyAnimatorName + "UI";
                break;
            case ePersonality.WEIRDO:
                animatorName = weirdoAnimatorName + "UI";
                break;
            case ePersonality.GRUMPY:
                animatorName = grumpyAnimatorName + "UI";
                break;
            case ePersonality.SCARED:
                animatorName = scaredAnimatorName + "UI";
                break;
            default:
                break;
        }
        return animatorName;
    }

    public static string GetPersonalityName(ePersonality personality)
    {
        string personalityName = "";
        switch (personality)
        {
            case ePersonality.HAPPY:
                personalityName = happyPersonalityName;
                break;
            case ePersonality.WEIRDO:
                personalityName = weirdoPersonalityName;
                break;
            case ePersonality.GRUMPY:
                personalityName = grumpyPersonalityName;
                break;
            case ePersonality.SCARED:
                personalityName = scaredPersonalityName;
                break;
            default:
                break;
        }
        return personalityName;
    }

    public static ePersonality GetPersonalityFromName(string name)
    {
        switch (name)
        {
            case "Happy":
                return ePersonality.HAPPY;
            case "Grumpy":
                return ePersonality.GRUMPY;
            case "Scared":
                return ePersonality.SCARED;
            case "Weirdo":
                return ePersonality.WEIRDO;
            default:
                break;
        }
        return ePersonality.HAPPY;
    }

    public static ePersonality GetRandomPersonality()
    {
        return (ePersonality)Random.Range(0, System.Enum.GetValues(typeof(ePersonality)).Length);
    }

    public void SetPersonality(ePersonality state, bool isRobot, bool isMask)
    {
        personality = state;

        animFace.runtimeAnimatorController =
            GameManager.Instance.CharacterAssets.GetPersonality(
                GetPersonalityAnimatorName(personality));
        soundModule.InitPersonality(personality, isRobot, isMask, true);
    }

    public void SetIsOutsideCamera(bool state)
    {
        isOutsideCamera = state;
        lastPositionBeforeOutsideCamera = transform.position;
    }

    public bool IsPlayerGrabbing(bool isLeft)
    {
        return isLeft ? player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabLeft) : player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabRight);
    }

    public bool IsPlayerAskingForSkip()
    {
        return player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Menu_Skip);
    }

    public void StartVibrate(float intensity)
    {
        Joystick activeJoystick = GetLastActiveJoystick();
        
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            VibrateController(activeJoystick, 0, intensity, 0);
            VibrateController(activeJoystick, 1, intensity, 0);
        }
    }

    public void StartVibrate(float intensity, eControllerMotors motorIndex)
    {
        Joystick activeJoystick = GetLastActiveJoystick();
        
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            switch (motorIndex)
            {
                case eControllerMotors.LEFT:
                {
                    VibrateController(activeJoystick, 0, intensity, 0);
                    break;
                }
                case eControllerMotors.RIGHT:
                {
                    VibrateController(activeJoystick, 1, intensity, 0);
                    break;
                }
                default:
                    break;
            }
        }
    }

    public void Vibrate(float intensity, float duration)
    {
        Joystick activeJoystick = GetLastActiveJoystick();
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            VibrateController(activeJoystick, 0, intensity, duration);
            VibrateController(activeJoystick, 1, intensity, duration);
        }
    }

    private void VibrateController(Joystick joystick, int motorIndex, float intensity, float duration)
    {
#if UNITY_SWITCH
        intensity *= SwitchVibrationScale;
        if (motorIndex >= joystick.vibrationMotorCount)
        {
            motorIndex = 0;
        }
#endif
        joystick.SetVibration(motorIndex, intensity, duration);
    }

    public void Vibrate(float intensity, float duration, eControllerMotors motorIndex)
    {
        Joystick activeJoystick = GetLastActiveJoystick();
        
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            switch (motorIndex)
            {
                case eControllerMotors.LEFT:
                {
                    VibrateController(activeJoystick, 0, intensity, duration);
                    break;
                }
                case eControllerMotors.RIGHT:
                {
                    VibrateController(activeJoystick, 1, intensity, duration);
                    break;
                }
                default:
                    break;
            }
        }
    }

    public void GrabVibrate(eControllerMotors motorIndex)
    {
        Joystick activeJoystick = GetLastActiveJoystick();
        
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            switch (motorIndex)
            {
                case eControllerMotors.LEFT:
                {
                    VibrateController(activeJoystick, 0, LeftVibrationForce, GrabVibrationDuration);
                    break;
                }
                case eControllerMotors.RIGHT:
                {
                    VibrateController(activeJoystick, 1, RightVibrationForce, GrabVibrationDuration);
                    break;
                }
                default:
                    break;
            }
        }
    }
    
    //If there is more than 1 joystick assigned to a player, as is the case when using handheld switch + pro-Controller
    //this will return the most recently used controller, this is for making sure only 1 of them will vibrate.
    Joystick GetLastActiveJoystick()
    {
        var playerJoysticks = player.GetControls().GetPlayerInput().controllers.Joysticks;
        //For debug when multiples players are present but the tester uses only one joystick
        if (playerJoysticks.Count == 0)
            return null;
        var lastActiveJoystick = playerJoysticks[0];
          
        if (playerJoysticks.Count == 1)
        {
            return lastActiveJoystick;
        }
      
        var lastJoystickInputTime = Mathf.Max(lastActiveJoystick.GetLastTimeAnyAxisActive(),
            lastActiveJoystick.GetLastTimeAnyButtonChanged());
        
        foreach (var joystick in playerJoysticks)
        {
            var joystickInputTime = Mathf.Max(joystick.GetLastTimeAnyAxisActive(), 
                joystick.GetLastTimeAnyButtonChanged());

            if(joystickInputTime >= lastJoystickInputTime)
            {
                lastActiveJoystick = joystick;
                lastJoystickInputTime = joystickInputTime;
            }
        }
       
        return lastActiveJoystick;
    }
    
    public void StuckVibrate(eControllerMotors motorIndex)
    {
        Joystick activeJoystick = GetLastActiveJoystick();
        
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            switch (motorIndex)
            {
                case eControllerMotors.LEFT:
                {
                    VibrateController(activeJoystick, 0, CharacterStuckVibration, 0);
                    break;
                }
                case eControllerMotors.RIGHT:
                {
                    VibrateController(activeJoystick, 1, CharacterStuckVibration, 0);
                    break;
                }
                default:
                    break;
            }
        }
    }

    public void BreakVibrate(eControllerMotors motorIndex)
    {
        Joystick activeJoystick = GetLastActiveJoystick();
       
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            switch (motorIndex)
            {
                case eControllerMotors.LEFT:
                {
                    VibrateController(activeJoystick, 0, BreakVibration, BreakVibrationDuration);
                    break;
                }
                case eControllerMotors.RIGHT:
                {
                    VibrateController(activeJoystick, 1, BreakVibration, BreakVibrationDuration);
                    break;
                }
                default:
                    break;
            }
        }
    }

    public void DeathVibrate()
    {
        Joystick activeJoystick = GetLastActiveJoystick();
        
        if (activeJoystick != null && activeJoystick.supportsVibration)
        {
            VibrateController(activeJoystick, 0, DeathVibrationForce, DeathVibrationDuration);
            VibrateController(activeJoystick, 1, DeathVibrationForce, DeathVibrationDuration);
        }
    }

    public void StopVibration()
    {
        foreach (Joystick j in player.GetControls().GetPlayerInput().controllers.Joysticks)
        {
            if (j.supportsVibration)
            {
                j.StopVibration();
            }
        }
    }

    void HandlePointHand(Hand hand)
    {
        if (hand == null || hand.GetIsHooked())
            return;

        if (player.GetControls().GetPlayerInput().GetButton(hand.IsLeft ? RewiredConsts.Action.Character_PointLeft : RewiredConsts.Action.Character_PointRight) && !hand.GetIsPointing())
        {
            hand.Point();
            if (currentAnimState != eFaceAnim.VICTORY && timerBeforeNextPoint == 0)
            {
                animFace.SetTrigger("Point");
                soundModule.StopAllPlayingEvents();
                soundModule.PlayCharacterSound(CharacterSoundModule.eState.POINT);
                timerBeforeNextPoint = TimeBeforeNextPoint;
            }
        }
        else if (!player.GetControls().GetPlayerInput().GetButton(hand.IsLeft ? RewiredConsts.Action.Character_PointLeft : RewiredConsts.Action.Character_PointRight) && hand.GetIsPointing())
        {
            hand.SetIsPointing(false);
            if (player.GetControls().GetPlayerInput().GetButton(hand.IsLeft ? RewiredConsts.Action.Character_GrabLeft : RewiredConsts.Action.Character_GrabRight))
            {
                hand.CloseHand();
            }
            else
            {
                hand.OpenHand();
            }
        }
    }

    public bool IsCurrentFaceAnimJoy()
    {
        return animFace.GetCurrentAnimatorStateInfo(0).IsName("Joy");
    }

    void HandleExpressions()
    {
        if (player.GetControls().GetPlayerInput().GetButtonDown(RewiredConsts.Action.Character_Joy))
        {
            if (timerBeforeNextFaceEmotion == 0)
            {
                animFace.SetTrigger("Joy");
                soundModule.StopAllPlayingEvents();
                soundModule.PlayCharacterSound(CharacterSoundModule.eState.JOY);
                timerBeforeNextFaceEmotion = TimeBeforeNextFaceEmotion;
            }

            if (currentAnimState == eFaceAnim.WAITING)
            {
                SetFaceAnimState(eFaceAnim.IDLE);
            }
            timerNoHook = 0f;
        }
        else if (player.GetControls().GetPlayerInput().GetButtonDown(RewiredConsts.Action.Character_Angry))
        {
            if (timerBeforeNextFaceEmotion == 0)
            {
                animFace.SetTrigger("Angry");
                soundModule.StopAllPlayingEvents();
                soundModule.PlayCharacterSound(CharacterSoundModule.eState.ANGRY);
                timerBeforeNextFaceEmotion = TimeBeforeNextFaceEmotion;
            }

            if (currentAnimState == eFaceAnim.WAITING)
            {
                SetFaceAnimState(eFaceAnim.IDLE);
            }
            timerNoHook = 0f;
        }

        if (timerBeforeNextFaceEmotion > 0f)
        {
            timerBeforeNextFaceEmotion = Mathf.Max(timerBeforeNextFaceEmotion - Time.deltaTime, 0f);
        }
    }

    public void HandleEffortHook(Hand hand)
    {
        if (hand.GetHook().CompareTag("Character"))
        {
            if (currentAnimState != eFaceAnim.EFFORT2 && currentAnimState != eFaceAnim.EFFORT3)
            {
                SetFaceAnimState(eFaceAnim.EFFORT2);
            }
            else
            {
                RePlayCurrentAnim();
            }
            Character character = GetCharacterFromGameObject(hand.GetHook().gameObject);
            if (character != null && (character.LeftHand != null && character.LeftHand.GetIsHooked() || character.RightHand != null && character.RightHand.GetIsHooked()))
            {
                character.SetEffortFaceWhenHeldByCharacter();
            }
        }
        else if (GetIsHeldByCharacters().Count > 0)
        {
            SetEffortFaceWhenHeldByCharacter();
        }
    }

    public void HandleEffortUnhook(Hand hand)
    {
        if (hand.GetHook() != null)
        {
            Character character = GetCharacterFromGameObject(hand.GetHook().gameObject);
            if (character != null && (character.LeftHand != null && character.LeftHand.GetIsHooked() || character.RightHand != null && character.RightHand.GetIsHooked()))
            {
                character.SetEffortFaceWhenReleasedByCharacter();
            }
        }

        Hand otherHand = GetOtherHand(hand);
        if (currentAnimState == eFaceAnim.EFFORT2 || currentAnimState == eFaceAnim.EFFORT3)
        {
            if (otherHand == null || !otherHand.GetIsHooked())
            {
                SetFaceAnimState(eFaceAnim.IDLE);
            }
            else if (!otherHand.GetHook().CompareTag("Character") && GetIsHeldByCharacters().Count <= 0)
            {
                SetFaceAnimState(eFaceAnim.EFFORT1);
            }
        }
    }

    public bool GetIsUnderTension()
    {
        return isUnderTension;
    }

    public void SetIsUnderTension(bool state)
    {
        isUnderTension = state;
        if (isUnderTension)
        {
            SetFaceAnimState(eFaceAnim.EFFORT3);
        }
        else
        {
            SetFaceAnimState(eFaceAnim.LOSE_ARM);
        }
    }

    /*public void SetFaceAnimElectric()
    {
        SetFaceAnimState(eFaceAnim.ELECTRIC, false, true);
    }*/

    public void RefreshAnimEffort()
    {
        if (LeftHand && LeftHand.GetIsHooked() && LeftHand.GetHook().CompareTag("Character") || RightHand && RightHand.GetIsHooked() && RightHand.GetHook().CompareTag("Character"))
        {
            SetFaceAnimState(eFaceAnim.EFFORT2);
        }
        else if (LeftHand && LeftHand.GetIsHooked() && LeftHand.GetHook().CompareTag("Grab") || RightHand && RightHand.GetIsHooked() && RightHand.GetHook().CompareTag("Grab"))
        {
            SetFaceAnimState(eFaceAnim.EFFORT1);
        }
        else
        {
            SetFaceAnimState(eFaceAnim.IDLE);
        }
    }

    void SetFaceAnimState(eFaceAnim state, bool skipSound = false, bool forceReplay = false)
    {
        if ((!forceReplay && state == currentAnimState) || (!forceReplay && isElectric) || currentAnimState == eFaceAnim.VICTORY)
            return;

        //Exit
        switch (currentAnimState)
        {
            case eFaceAnim.IDLE:
                break;
            case eFaceAnim.WAITING:
                animFace.SetBool("Waiting", false);
                break;
            case eFaceAnim.FALL:
                animFace.SetBool("Falling", false);
                break;
            case eFaceAnim.EFFORT1:
                animFace.SetBool("Effort1", false);
                break;
            case eFaceAnim.EFFORT2:
                animFace.SetBool("Effort2", false);
                break;
            case eFaceAnim.EFFORT3:
                animFace.SetBool("Effort3", false);
                break;
            case eFaceAnim.BUMP_CHARACTER:
                timerAnimBumpCharacter = 0f;
                animFace.SetBool("BumpCharacter", false);
                break;
            case eFaceAnim.BUMP_PLATFORM:
                timerAnimBumpPlatform = 0f;
                animFace.SetBool("BumpPlatform", false);
                break;
            case eFaceAnim.ELECTRIC:
                break;
            default:
                break;
        }

        previousAnimState = currentAnimState;
        currentAnimState = state;

        //New
        switch (currentAnimState)
        {
            case eFaceAnim.IDLE:
                timerNoHook = 0f;
                break;
            case eFaceAnim.WAITING:
                animFace.SetBool("Waiting", true);
                timerNoHook = TimeNoHook;
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.WAIT);
                }
                break;
            case eFaceAnim.FALL:
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.FALL);
                    animFace.SetTrigger("StartFall");
                    animFace.SetBool("Falling", true);
                }
                break;
            case eFaceAnim.EFFORT1:
                animFace.SetTrigger("StartEffort1");
                animFace.SetBool("Effort1", true);
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.EFFORT1);
                }                
                break;
            case eFaceAnim.EFFORT2:
                timerEffort = 0f;
                animFace.SetTrigger("StartEffort2");
                animFace.SetBool("Effort2", true);
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.EFFORT2);
                }
                break;
            case eFaceAnim.EFFORT3:
                animFace.SetBool("Effort3", true);
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.EFFORT3);
                }
                break;
            case eFaceAnim.BUMP_CHARACTER:
                animFace.SetTrigger("StartBumpCharacter");
                animFace.SetBool("BumpCharacter", true);
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.HIT);
                }
                break;
            case eFaceAnim.BUMP_PLATFORM:
                animFace.SetTrigger("StartBumpPlatform");
                animFace.SetBool("BumpPlatform", true);
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.HIT);
                }
                break;
            case eFaceAnim.VICTORY:
                animFace.SetBool("Victory", true);
                if (!skipSound)
                {
                    soundModule.StopAllPlayingEvents();
                    soundModule.PlayCharacterSound(CharacterSoundModule.eState.VICTORIOUS);
                }
                break;
            case eFaceAnim.LOSE_ARM:
                animFace.SetTrigger("StartBumpPlatform");
                animFace.SetBool("BumpPlatform", true); // to do change in anim arm break
                soundModule.PlayCharacterSound(CharacterSoundModule.eState.LOSE_ARM);
                soundModule.PlayOneShot("ArmRip", gameObject);
                break;
            case eFaceAnim.ELECTRIC:
                //animFace.SetBool("Effort3", false);
                if (!skipSound)
                {
                    SetIsElectric(true);
                    soundModule.PlayElectric();
                    //soundModule.StopAllPlayingEvents();
                }
                break;
            default:
                break;
        }
    }

    void UpdateFaceAnim()
    {
        switch (currentAnimState)
        {
            case eFaceAnim.IDLE:
                if (!isInputImpulsePressing)
                {
                    timerNoHook = Mathf.Min(timerNoHook + Time.deltaTime, TimeNoHook);
                    if (timerNoHook >= TimeNoHook)
                    {
                        SetFaceAnimState(eFaceAnim.WAITING);
                    }
                }
                break;
            case eFaceAnim.WAITING:
                timerNoHook = Mathf.Max(timerNoHook - Time.deltaTime, 0f);
                if (timerNoHook == 0f || isInputImpulsePressing)
                {
                    SetFaceAnimState(eFaceAnim.IDLE);
                }
                break;
            case eFaceAnim.FALL:
                if (IsCharacterLayingOnGrabSurface() || GetIsHeldByCharacters().Count > 0)
                {
                    SetFaceAnimState(eFaceAnim.IDLE);
                }
                break;
            case eFaceAnim.EFFORT1:
                break;
            case eFaceAnim.EFFORT2:
                timerEffort = Mathf.Min(timerEffort + Time.deltaTime, TimeEffort);
                if (timerEffort >= TimeEffort)
                {
                    SetFaceAnimState(eFaceAnim.EFFORT3);
                }
                break;
            case eFaceAnim.EFFORT3:
                break;
            case eFaceAnim.BUMP_CHARACTER:
                if (!isBumped && animFace.GetCurrentAnimatorStateInfo(0).IsName("BumpCharacter"))
                {
                    isBumped = true;
                }
                timerAnimBumpCharacter = Mathf.Min(timerAnimBumpCharacter + Time.deltaTime, TimeAnimBumpCharacter);
                if (timerAnimBumpCharacter >= TimeAnimBumpCharacter)
                {
                    isBumped = false;
                    SetFaceAnimState(previousAnimState, true);
                }
                break;
            case eFaceAnim.BUMP_PLATFORM:
                if (!isBumped && animFace.GetCurrentAnimatorStateInfo(0).IsName("BumpPlatform"))
                {
                    isBumped = true;
                }
                timerAnimBumpPlatform = Mathf.Min(timerAnimBumpPlatform + Time.deltaTime, TimeAnimBumpPlatform);
                if (timerAnimBumpPlatform >= TimeAnimBumpPlatform)
                {
                    isBumped = false;
                    SetFaceAnimState(previousAnimState, true);
                }
                break;
            case eFaceAnim.VICTORY:
                break;
            case eFaceAnim.SING:
                break;
            case eFaceAnim.LOSE_ARM:
                timerAnimLoseArm = Mathf.Min(timerAnimLoseArm + Time.deltaTime, TimeAnimLoseArm);
                if (timerAnimLoseArm >= TimeAnimLoseArm)
                {
                    SetFaceAnimState(eFaceAnim.IDLE);
                }
                break;
            case eFaceAnim.ELECTRIC:
                break;
            default:
                break;
        }
    }

    public void RePlayCurrentAnim()
    {
        SetFaceAnimState(currentAnimState, false, true);
    }

    public BloodParticles SpawnParticles(Vector2 pos, Vector2 direction, bool isSoft = false, bool isAttachedToCamera = false)
    {
        direction.Normalize();
        float newRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        BloodParticles blood = Instantiate(isSoft ? BloodParticlePrefabSoft : BloodParticlePrefab, pos, Quaternion.Euler(0f, 0f, newRot), isAttachedToCamera ? Camera.main.transform : null);
        blood.SetColor(currentOutfit.BodyColor);
        //blood.SetParticlesOrientation(newRot > 0 && newRot < 90 ? newRot - 90f : -(newRot - 90f));
        blood.SetParticlesOrientation(-(newRot - 90f));
        return blood;
    }

    public GameObject SpawnDeathFX(Vector2 pos, Quaternion rot, bool isAttachedToCamera = false)
    {
        return Instantiate(DeathFXPrefab, pos, rot, isAttachedToCamera ? Camera.main.transform : null);
    }

    void UpdateFalling()
    {
        if (!isFalling && !isCheckingFall)
        {
            if ((LeftHand == null || !LeftHand.GetIsHooked() || (LeftHand.GetIsHooked() && (LeftHand.GetHook().CompareTag("Character") || LeftHand.GetHook().CompareTag("Collectible") || LeftHand.GetHook().CompareTag("Toy"))))
                && (RightHand == null || !RightHand.GetIsHooked() || (RightHand.GetIsHooked() && (RightHand.GetHook().CompareTag("Character") || RightHand.GetHook().CompareTag("Collectible") || RightHand.GetHook().CompareTag("Toy"))))
                && !AreTouchingCharactersOnGrabSurface(this, new List<Character>()) && GetIsHeldByCharacters().Count <= 0)
            {
                isCheckingFall = true;
                timerBeforeFall = 0f;
            }
        }
        else if (isFalling || isCheckingFall)
        {
            if (LeftArm != null && GameManager.Instance.GetChainsManager().IsHookEnchored(LeftArm) || RightArm != null && GameManager.Instance.GetChainsManager().IsHookEnchored(RightArm)
                || AreTouchingCharactersOnGrabSurface(this, new List<Character>()) || GetIsHeldByCharacters().Count > 0)
            {
                isCheckingFall = false;
                isFalling = false;
                soundModule.StopCharacterEvent();
            }
            else if (!isFalling)
            {
                timerBeforeFall = Mathf.Min(timerBeforeFall + Time.deltaTime, TimeBeforeFall);
                if (timerBeforeFall >= TimeBeforeFall)
                {
                    isFalling = true;
                    SetFaceAnimState(eFaceAnim.FALL);
                    isCheckingFall = false;
                }
            }
        }
    }

    void UpdateOutsideCamera()
    {
        if (isOutsideCamera)
        {
            if (LevelManager.IsObjectInsideCamera(gameObject))
            {
                isOutsideCamera = false;
                timerOutsideCamera = 0f;
                if (currentParticles != null)
                {
                    Destroy(currentParticles.gameObject);
                }
                if (currentDeathFX != null)
                {
                    Destroy(currentDeathFX);
                }
            }
            else
            {
                if (levelManager.GetIsOutsideLimit(gameObject)
                    && !GameManager.Instance.GetChainsManager().IsEnchoredVisible(this))
                //&& (LeftArm == null || !GameManager.Instance.GetChainsManager().IsEnchoredVisible(LeftArm))
                //&& (RightArm == null || !GameManager.Instance.GetChainsManager().IsEnchoredVisible(RightArm)))
                {
                    timerOutsideCamera = Mathf.Min(timerOutsideCamera + Time.deltaTime, levelManager.TimeOutsideCameraBeforeDeath);
                    if (timerOutsideCamera >= levelManager.TimeOutsideCameraBeforeDeath)
                    {
                        if (currentParticles != null) // can be null when returning from a minigame
                        {
                            currentParticles.gameObject.SetActive(true);
                        }
                        if (currentDeathFX != null) // can be null when returning from a minigame
                        {
                            currentDeathFX.gameObject.SetActive(true);
                        }

                        Die();
                        soundModule.StopAllPlayingEvents();
                        soundModule.PlayOneShot("Death", gameObject);
                        DeathVibrate();
                    }
                }
            }
        }
        else
        {
            if (!LevelManager.IsObjectInsideCamera(gameObject))
            {
                isOutsideCamera = true;
                lastPositionBeforeOutsideCamera = transform.position;
                Vector3 centerOffset = new Vector3(Random.Range(Screen.width / 2f - offsetPercentXBloodParticlesPos, Screen.width / 2f + offsetPercentXBloodParticlesPos),
                                                    Random.Range(Screen.height / 2f - offsetPercentYBloodParticlesPos, Screen.height / 2f + offsetPercentYBloodParticlesPos), 0f);
                Vector3 diff = centerOffset - Camera.main.WorldToScreenPoint(lastPositionBeforeOutsideCamera);
                Vector3 lastPosOnCamera = Camera.main.WorldToScreenPoint(lastPositionBeforeOutsideCamera);
                /*if (lastPosOnCamera.x < 0f)
                {
                    diff = Vector3.right;
                }
                else if (lastPosOnCamera.x > Screen.width)
                {
                    diff = Vector3.left;
                }
                else if (lastPosOnCamera.y < 0f)
                {
                    diff = Vector3.up;
                }
                else
                {
                    diff = Vector3.down;
                }*/
                currentParticles = SpawnParticles(lastPositionBeforeOutsideCamera, diff, false, true);
                currentParticles.gameObject.SetActive(false);

                float newRot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                currentDeathFX = SpawnDeathFX(lastPositionBeforeOutsideCamera, Quaternion.Euler(0f, 0f, newRot - 90f), false);
                currentDeathFX.gameObject.SetActive(false);
            }
        }
    }

    float GetMouseHorizontalValue()
    {
        float distance = player.GetControls().GetPlayerMouseInput().screenPosition.x;
        if (distance > 0)
        {
            return Mathf.InverseLerp(0f, RadiusSizeForMouseControl, distance);
        }
        return -Mathf.InverseLerp(0f, -RadiusSizeForMouseControl, distance);
    }

    float GetMouseVerticalValue()
    {
        float distance = player.GetControls().GetPlayerMouseInput().screenPosition.y - previousMousePos.y;
        if (distance > 0)
        {
            return Mathf.InverseLerp(0f, RadiusSizeForMouseControl, distance);
        }
        return -Mathf.InverseLerp(0f, -RadiusSizeForMouseControl, distance);
    }

    Vector2 ComputeMouseVector()
    {
        virtualCursorPosition += new Vector2(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_LeftArmMovementX), player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_LeftArmMovementY));
        Debug.Log("virtualCursorPosition raw : " + virtualCursorPosition);
        float distance = Vector2.Distance(refMousePoint, virtualCursorPosition);
        if (distance > RadiusSizeForMouseControl)
        {
            Vector2 fromOriginToObject = virtualCursorPosition - refMousePoint;
            fromOriginToObject *= RadiusSizeForMouseControl / distance;
            virtualCursorPosition = refMousePoint + fromOriginToObject;
        }
        Debug.Log("virtualCursorPosition computed : " + virtualCursorPosition);
        Debug.Log("virtualCursorPosition normalized : " + virtualCursorPosition.normalized);
        return virtualCursorPosition.normalized;
    }

    bool GetIsLeftGrabButton()
    {
        return player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabLeft);
    }

    bool GetIsRightGrabButton()
    {
        return player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabRight);
    }

    bool GetIsLeftGrabButtonUp()
    {
        return player.GetControls().GetPlayerInput().GetButtonUp(RewiredConsts.Action.Character_GrabLeft);
    }

    bool GetIsRightGrabUp()
    {
        return player.GetControls().GetPlayerInput().GetButtonUp(RewiredConsts.Action.Character_GrabRight);
    }

    public void SetPreviousMousePos(Vector2 pos)
    {
        previousMousePos = pos;
    }

    void Update()
    {
        if (isControlBlocked || player == null || isPaused)
            return;

        //CONTROLS
        if (player.GetControls().GetPlayerInput().GetButtonDown(RewiredConsts.Action.Character_Reset)) //Maybe to put in debug
        {
            GameObject ResObj = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (ResObj)
            {
                RespawnPoint respawnPoint = ResObj.GetComponent<RespawnPoint>();
                if (respawnPoint)
                {
                    respawnPoint.AddPlayerToRespawnQueue(player);
                    Destroy(gameObject);
                }
            }
        }

        /*if (hasTouchedPlatform)
        {
            if (PlayerInput.GetButtonDown(RewiredConsts.Action.Character_Jump))
            {
                if (timerJumpCooldown >= JumpCooldown)
                {
                    isJumping = true;
                    hasTouchedPlatform = false;
                    timerJumpCooldown = 0f;
                }
            }
            else
            {
                timerJumpCooldown = Mathf.Min(timerJumpCooldown + Time.deltaTime, JumpCooldown);
            }
        }
        else
        {
            if (LeftHand != null && LeftHand.GetIsHooked() && LeftHand.GetHook().CompareTag("Grab")
                || RightHand != null && RightHand.GetIsHooked() && RightHand.GetHook().CompareTag("Grab"))
            {
                hasTouchedPlatform = true;
            }
        }*/

        /*if (player.GetControls().GetControlsMode() == PlayerControls.eControlsMode.MOUSE)
        {
            MoveVectorLeft = ComputeMouseVector();
            MoveVectorRight = MoveVectorLeft;
            Debug.Log("Left x : " + MoveVectorLeft.x + " y : " + MoveVectorLeft.y);
        }
        else
        {*/
            if (Mathf.Abs(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_LeftArmMovementX)) > Mathf.Epsilon
                || Mathf.Abs(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_LeftArmMovementY)) > Mathf.Epsilon)
            {
                MoveVectorLeft.x = Mathf.Clamp(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_LeftArmMovementX), -1f, 1f);
                MoveVectorLeft.y = Mathf.Clamp(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_LeftArmMovementY), -1f, 1f);
                //Debug.Log("Left x : " + MoveVectorLeft.x + " y : " + MoveVectorLeft.y);
            }
            else if (MoveVectorLeft != Vector2.zero)
            {
                MoveVectorLeft = Vector2.zero;
            }
            if (Mathf.Abs(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_RightArmMovementX)) > Mathf.Epsilon
                || Mathf.Abs(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_RightArmMovementY)) > Mathf.Epsilon)
            {
                MoveVectorRight.x = Mathf.Clamp(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_RightArmMovementX), -1f, 1f);
                MoveVectorRight.y = Mathf.Clamp(player.GetControls().GetPlayerInput().GetAxis(RewiredConsts.Action.Character_RightArmMovementY), -1f, 1f);
                //Debug.Log("Right x : " + MoveVectorRight.x + " y : " + MoveVectorRight.y);
            }
            else if (MoveVectorRight != Vector2.zero)
            {
                MoveVectorRight = Vector2.zero;
            }
        //}

        //Left Hand Grab
        if (LeftHand != null && !LeftHand.GetIsDisabled() && GetIsLeftGrabButton())
        {
            LeftHand.CloseHand();
            if (!LeftHand.GetIsHooked() && LeftHand.GetHook() == null)
            {
                RaycastHit2D hook = LeftHand.GetClosestHook();
                if (hook.collider != null)
                {
                    LeftHand.StartHook(hook);
                }
            }
        }
        else if (LeftHand != null && !LeftHand.GetIsDisabled() && GetIsLeftGrabButtonUp() && !LeftHand.debugBlockHand)
        {
            if (LeftHand.GetIsHooked())
            {
                if (RightHand && !RightHand.GetIsHooked())
                {
                    SetFaceAnimState(eFaceAnim.IDLE);
                }
                LeftHand.UnHook();
            }
            else
            {
                LeftHand.CancelHooking();
            }
        }

        //Right Hand Grab
        if (RightHand != null && !RightHand.GetIsDisabled() && GetIsRightGrabButton())
        {
            RightHand.CloseHand();
            if (!RightHand.GetIsHooked() && RightHand.GetHook() == null)
            {
                RaycastHit2D hook = RightHand.GetClosestHook();
                if (hook.collider != null)
                {
                    RightHand.StartHook(hook);
                }
            }
            
        }
        else if (RightHand != null && !RightHand.GetIsDisabled() && GetIsRightGrabUp() && !RightHand.debugBlockHand)
        {
            if (RightHand.GetIsHooked())
            {
                if (LeftHand && !LeftHand.GetIsHooked())
                {
                    SetFaceAnimState(eFaceAnim.IDLE);
                }
                RightHand.UnHook();
            }
            else
            {
                RightHand.CancelHooking();
            }
        }

        HandlePointHand(LeftHand);
        HandlePointHand(RightHand);

        HandleExpressions();

        UpdateFaceAnim();
        if (levelManager.GetVictoryTrigger() == null || !levelManager.GetVictoryTrigger().IsValidated())
        {
            UpdateOutsideCamera();
        }

        UpdateFalling();

        if (timerBeforeNextPoint > 0f)
        {
            timerBeforeNextPoint = Mathf.Max(timerBeforeNextPoint - Time.deltaTime, 0f);
        }
    }

    void FixedUpdate ()
    {
        if (isPaused)
            return;

        Vector2 DirectionForcePull = Vector2.zero;
        bool isPullingLeft = false;
        bool isPullingRight = false;

        /*if (isJumping)
        {
            RandomJump();
            isJumping = false;
        }*/

        if (LeftHand != null && MoveVectorLeft != Vector2.zero)
        {
            if (LeftHand.GetIsHooked() && GameManager.Instance.GetChainsManager().IsHookEnchored(LeftHand.GetArm()) || GameManager.Instance.GetChainsManager().IsAtLeatOneEnchored(LeftArm.GetIsArmHeldByOtherCharacter()))
            {
                isPullingLeft = true;
                DirectionForcePull += MoveVectorLeft;
                if (GetIsHeldByCharacters().Count <= 0 && !isUnderTension)
                {
                    if (LeftHand.GetIsHookNew() && rb.velocity.magnitude >= ForceVelocitySoundEffort)
                    {
                        if (currentAnimState != eFaceAnim.EFFORT1 && currentAnimState != eFaceAnim.EFFORT2 && currentAnimState != eFaceAnim.EFFORT3)
                        {
                            SetFaceAnimState(eFaceAnim.EFFORT1);
                        }
                        else
                        {
                            RePlayCurrentAnim();
                        }
                        LeftHand.ConsumeNewHook();
                    }
                }
                if (LeftArm.GetIsArmHeldByOtherCharacter().Count > 0)
                {
                    LeftHand.AddForce(MoveVectorLeft * ForceMovement);
                }
            }
            /*else if (LeftHand.GetIsHooked() && !LeftHand.GetHook().CompareTag("Collectible"))
            {
                // Des fois Hook == null - a corriger
                LeftHand.AddForce(Vector2.zero);
            }*/
            else
            {
                LeftHand.AddForce(MoveVectorLeft * ForceMovement);
            }
        }

        if (RightHand != null && MoveVectorRight != Vector2.zero)
        {
            if (RightHand.GetIsHooked() && GameManager.Instance.GetChainsManager().IsHookEnchored(RightHand.GetArm()) || GameManager.Instance.GetChainsManager().IsAtLeatOneEnchored(RightArm.GetIsArmHeldByOtherCharacter()))
            {
                isPullingRight = true;
                DirectionForcePull += MoveVectorRight;
                if (GetIsHeldByCharacters().Count <= 0 && !isUnderTension)
                {
                    if (RightHand.GetIsHookNew() && rb.velocity.magnitude >= ForceVelocitySoundEffort)
                    {
                        if (currentAnimState != eFaceAnim.EFFORT1 && currentAnimState != eFaceAnim.EFFORT2 && currentAnimState != eFaceAnim.EFFORT3)
                        {
                            SetFaceAnimState(eFaceAnim.EFFORT1);
                        }
                        else
                        {
                            RePlayCurrentAnim();
                        }
                        RightHand.ConsumeNewHook();
                    }
                }
                if (RightArm.GetIsArmHeldByOtherCharacter().Count > 0)
                {
                    RightHand.AddForce(MoveVectorRight * ForceMovement);
                }
            }
            /*else if (RightHand.GetIsHooked() && !RightHand.GetHook().CompareTag("Collectible"))
            {
                // Des fois Hook == null - a corriger
                RightHand.AddForce(Vector2.zero);
            }*/
            else
            {
                RightHand.AddForce(MoveVectorRight * ForceMovement);
            }
        }

        if (isPullingLeft && isPullingRight)
        {
            rb.AddForce(DirectionForcePull * ForcePull/ 2f);
        }
        else if (isPullingLeft || isPullingRight)
        {
            rb.AddForce(DirectionForcePull * ForcePull);
        }

        timerReleaseImpulseCoolDown = Mathf.Min(timerReleaseImpulseCoolDown + Time.fixedDeltaTime, ReleaseImpulseCooldown);

        if (player == null)
            return;

        HandleImpulse();
    }

    void HandleImpulse()
    {
        if (!isElectric && player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_Impulse))
        {
            isInputImpulsePressing = true;
            timerNoHook = 0f;
        }
        else if (isElectric && isInputImpulsePressing)
        {
            isInputImpulsePressing = false;
            timerInputImpulse = 0f;
            animFace.SetBool("FocusImpulse", false);
            Head.StopShake();
            return;
        }

        if (isInputImpulsePressing && !player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_Impulse))
        {
            if (timerReleaseImpulseCoolDown == ReleaseImpulseCooldown)
            {
                timerReleaseImpulseCoolDown = 0f;
            }
            if (timerInputImpulse > ReleaseImpulseTimeMin)
            {
                ReleaseCharactersInRadius();
                soundModule.PlayCharacterSound(CharacterSoundModule.eState.RELEASED_CHARGE);
                animFace.SetTrigger("Impulse");
            }
            StopVibration();
            timerInputImpulse = 0f;
            animFace.SetBool("FocusImpulse", false);
            //soundModule.StopCharge();
            isInputImpulsePressing = false;
            Head.StopShake();
        }
        //Launches character impulse
        else if (isInputImpulsePressing && timerReleaseImpulseCoolDown == ReleaseImpulseCooldown)
        {
            if (timerInputImpulse == 0)
            {
                animFace.SetBool("FocusImpulse", true);
                Head.StartShake(ShakeHeadStartFrequence, ShakeHeadEndFrequence, ShakeHeadStartAmplitude, ShakeHeadEndAmplitude, ReleaseImpulseTimeMax - ReleaseImpulseTimeMin);
            }

            timerInputImpulse = Mathf.Min(timerInputImpulse + Time.fixedDeltaTime, ReleaseImpulseTimeMax);
            if (timerInputImpulse > ReleaseImpulseTimeMin)
            {
                StartVibrate((timerInputImpulse * ReleaseImpulseVibrationMax) / ReleaseImpulseTimeMax);
            }
        }
    }

    public void SetIsControlBlocked(bool isBlocked)
    {
        isControlBlocked = isBlocked;
        if (!isControlBlocked)
        {
            if (LeftHand != null && LeftHand.GetIsHooked() && !player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabLeft))
            {
                LeftHand.UnHook();
            }
            if (RightHand != null && RightHand.GetIsHooked() && !player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabRight))
            {
                RightHand.UnHook();
            }
        }
    }

    public bool GetIsControlBlocked()
    {
        return isControlBlocked;
    }

    public void SetIsStatic(bool isStatic)
    {
        foreach (BodyPart bodyPart in Parts)
        {
            bodyPart.GetRigidbody().bodyType = isStatic ?  RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        }
    }

    public bool GetIsStatic()
    {
        return rb.bodyType == RigidbodyType2D.Static;
    }

    private void RemoveArm(Arm arm, Hand hand)
    {
        //If multiple joints have broken at the same time, this would cause a null reff
        if (hand == null || arm == null)
        {
            return;
        }
        
        hand.UnHook();
        hand.DestroyDistanceJointOnHand();
        BloodParticles particles = SpawnParticles(transform.position, transform.position - arm.transform.position, true);
        particles.transform.parent = transform;

        SetFaceAnimState(eFaceAnim.LOSE_ARM);

        toDestroyWhenDead.Add(arm.gameObject);
        toDestroyWhenDead.Add(arm.Forearm.gameObject);
        toDestroyWhenDead.Add(hand.gameObject);

        foreach (var collider in arm.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }

        foreach (var collider in arm.Forearm.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }

        arm.Forearm.transform.parent = null;
        arm.Forearm = null;
        arm.transform.parent = null;
        hand.transform.parent = null;
    }
    
    
    public void RemoveRightArm()
    {
        RemoveArm(RightArm, RightHand);
        RightArm = null;
        RightHand = null;
    }
    
    public void RemoveLeftArm()
    {
        RemoveArm(LeftArm, LeftHand);
        LeftArm = null;
        LeftHand = null;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grab") || collision.gameObject.CompareTag("Ground"))
        {
            //hasTouchedPlatform = true;
            BumpInfo bumpInfo = GetBumpIntensity(collision.relativeVelocity.magnitude);
            if (bumpInfo.VelocityToLaunch > 0)
            {
                Platform platform = collision.gameObject.GetComponent<Platform>();
                BumpPlatform(collision.GetContact(0), platform != null ? platform.Type : Platform.ePlatformType.NONE, bumpInfo);
            }
        }
        if (collision.gameObject.CompareTag("Character") && !IsGameObjectFromCharacter(collision.gameObject))
        {
            BumpInfo bumpInfo = GetBumpIntensity(collision.relativeVelocity.magnitude);
            if (bumpInfo.VelocityToLaunch > 0)
            {
                Platform platform = collision.gameObject.GetComponent<Platform>();
                BumpCharacter(collision.GetContact(0), platform != null ? platform.Type : Platform.ePlatformType.NONE, bumpInfo);
            }
        }
    }

    public BumpInfo GetBumpIntensity(float velocity)
    {
        if (velocity > BumpSmall.VelocityToLaunch && velocity <= BumpMedium.VelocityToLaunch)
        {
            return BumpSmall;
        }
        else if (velocity > BumpMedium.VelocityToLaunch && velocity <= BumpHard.VelocityToLaunch)
        {
            return BumpMedium;
        }
        else if (velocity > BumpHard.VelocityToLaunch && velocity <= BumpHudge.VelocityToLaunch)
        {
            return BumpHard;
        }
        else if (velocity > BumpHudge.VelocityToLaunch)
        {
            return BumpHudge;
        }
        return new BumpInfo();
    }

    void BumpEffect(ContactPoint2D contactPoint, BumpInfo bumpInfo)
    {
        if (bumpInfo.FXSize > 0f)
        {
            GameObject FX = Instantiate(BumpFXPrefab, contactPoint.point, Quaternion.Euler(0f, 0f, Random.Range(0f, 359f)));
            FX.transform.localScale = Vector2.one * bumpInfo.FXSize;
        }
        Vibrate(bumpInfo.VibrationIntensity, bumpInfo.VibrationDuration);

        if (bumpInfo.ShakeTime > 0f)
        {
            cam.Shake(bumpInfo.ShakeTime, bumpInfo.ShakeFrequency, bumpInfo.ShakeAmplitude);
        }

        rb.AddForce(bumpInfo.ForcePushBack * contactPoint.normal, ForceMode2D.Impulse);
    }

    public void BumpCharacter(ContactPoint2D contactPoint, Platform.ePlatformType platformType, BumpInfo bumpInfo)
    {
        BumpEffect(contactPoint, bumpInfo);
        soundModule.SetParameterValue("impacts", "Surface", 1f); // Surface soft
        soundModule.SetParameterValue("impacts", "ImpactIntensity", bumpInfo.Intensity);
        soundModule.PlayEvent("impacts");
        if (bumpInfo.Intensity > 1)
        {
            GameManager.Instance.GetMusicManager().LaunchBumpCharacterBodyMusicEvent(bumpInfo.Intensity);
        }
        if (bumpInfo.PlayAnimFace)
        {
            SetFaceAnimState(eFaceAnim.BUMP_CHARACTER);
        }
    }

    public void BumpPlatform(ContactPoint2D contactPoint, Platform.ePlatformType platformType, BumpInfo bumpInfo)
    {
        BumpEffect(contactPoint, bumpInfo);
        soundModule.SetParameterValue("impacts", "Surface", 2f); // Surface hard
        soundModule.SetParameterValue("impacts", "ImpactIntensity", bumpInfo.Intensity);
        soundModule.PlayEvent("impacts");
        if (bumpInfo.Intensity > 1)
        {
            GameManager.Instance.GetMusicManager().LaunchBumpCharacterMusicEvent(bumpInfo.Intensity);
        }
        if (bumpInfo.PlayAnimFace)
        {
            SetFaceAnimState(eFaceAnim.BUMP_PLATFORM);
        }
    }

    /*void OnCollisionStay2D(Collision2D collision)
    {
        if (!hasTouchedPlatform && (collision.gameObject.CompareTag("Grab") || collision.gameObject.CompareTag("Ground")))
        {
            hasTouchedPlatform = true;
        }
    }*/

    public static Character GetCharacterFromGameObject(GameObject obj)
    {
        Character character = obj.GetComponent<Character>();
        if (character == null)
        {
            character = obj.GetComponentInParent<Character>();
        }
        return character;
    }

    public bool IsCharacterLayingOnGrabSurface()
    {
        foreach (BodyPart part in Parts)
        { 
            if (part.GetIsTouchingGrabSurface())
            {
                return true;
            }
        }
        return false;
    }

    public bool AreTouchingCharactersOnGrabSurface(Character character, List<Character> checkedCharacters)
    {
        bool isTouching = false;

        if (!checkedCharacters.Contains(character))
        {
            checkedCharacters.Add(character);
            foreach (BodyPart part in character.Parts)
            {
                if (part.GetIsTouchingGrabSurface())
                {
                    return true;
                }
                else if (part.GetIsTouchingOtherCharacter())
                {
                    List<Character> touchingCharacters = part.GetTouchingCharacters();
                    foreach (Character touchingCharacter in touchingCharacters)
                    {
                        isTouching = AreTouchingCharactersOnGrabSurface(touchingCharacter, checkedCharacters);
                        if (isTouching)
                            return isTouching;
                    }
                }
            }
        }
        return isTouching;
    }

    public FixedJoint2D GetJointInOtherHand(Hand hand)
    {
        if (RightHand != null && hand == LeftHand)
        {
            return RightHand.GetHook();
        }

        if (LeftHand != null && hand == RightHand)
        {
            return LeftHand.GetHook();
        }

        return null;
    }

    public Hand GetOtherHand(Hand hand)
    {
        if (hand == LeftHand)
        {
            return RightHand;
        }

        if (hand == RightHand)
        {
            return LeftHand;
        }

        return null;
    }

    public void Die(bool respawn = true)
    {
        if (isDead)
            return;

        isDead = true;

        DestroyInstancedMaterials();

        //soundModule.PlayOneShot("Death");
        if (GameManager.Instance.GetMusicManager().GetIsDynamic())
        {
            GameManager.Instance.GetMusicManager().LaunchDeathMusicEvent();
        }

        /*if (isElectric)
        {
            SetIsElectric(false);
        }
        soundModule.StopCharge();*/

        //Stop holding stuff before dying
        if (LeftHand)
        {
            LeftHand.UnHook();
        }
        if (RightHand)
        {
            RightHand.UnHook();
        }

        //Stop Being hold by other characters
        List<Character> holdingCharacters = GetIsHeldByCharacters();
        if (holdingCharacters.Count > 0)
        {
            foreach (Character character in holdingCharacters)
            {
                if (character.LeftHand != null && character.LeftHand.GetIsHooked() && character.LeftHand.GetHook().gameObject == gameObject)
                {
                    character.LeftHand.UnHook();
                }

                if (character.RightHand != null && character.RightHand.GetIsHooked() && character.RightHand.GetHook().gameObject == gameObject)
                {
                    character.RightHand.UnHook();
                }
            }
        }

        //Stops Vibrations
        StopVibration();

        //If in Respawnzones trigger
        RespawnPoint[] respawnPoints = FindObjectsOfType<RespawnPoint>();
        for (int i = 0; i < respawnPoints.Length; ++i)
        {
            foreach (BodyPart bodyPart in Parts)
            {
                respawnPoints[i].RemoveCharacterFromTrigger(bodyPart);
            }
        }

        // Respawn
        if (respawn)
        {
            if (player != null && (player.GetCheckPoint() == null || player.GetCheckPoint().IsCheckpointInSafeZone()))
            {
                if (!GameManager.Instance.GetLevelSelector().GetCurrentLevel().LevelName.Contains("Ceremony"))
                {
                    GameManager.Instance.GetMetricsManager().AddDeathToPlayerMetrics(player);
                }
                player.Respawn();
            }
        }

        foreach (GameObject obj in toDestroyWhenDead)
        {
            Destroy(obj);
        }
        DeadEvent.Invoke(this);
        Destroy(gameObject);
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public List<Character> GetIsHeldByCharacters()
    {
        List<Character> characters = new List<Character>();
        foreach (BodyPart part in Parts)
        {
            if (part == null)
                continue;

            FixedJoint2D[] joints = part.GetComponents<FixedJoint2D>();
            if (joints.Length > 0)
            {
                foreach (FixedJoint2D joint in joints)
                {
                    if (joint.connectedBody && !IsGameObjectFromCharacter(joint.connectedBody.gameObject))
                    {
                        characters.Add(joint.connectedBody.GetComponentInParent<Character>());
                    }
                }
            }
        }
        return characters;
    }

    public List<Arm> GetIsBodyHeldByArms()
    {
        List<Arm> arms = new List<Arm>();
        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();
        if (joints.Length > 0)
        {
            foreach (FixedJoint2D joint in joints)
            {
                if (joint.connectedBody && !IsGameObjectFromCharacter(joint.connectedBody.gameObject))
                {
                    arms.Add(joint.connectedBody.GetComponent<Hand>().GetArm());
                }
            }
        }
        return arms;
    }

    public bool IsGameObjectFromCharacter(GameObject obj)
    {
        BodyPart bodyPart = obj.GetComponent<BodyPart>();

        if (bodyPart)
        {
            List<BodyPart> partsToCheck = new List<BodyPart>();
            partsToCheck.AddRange(Parts);

            if (player == null)
            {
                return false;
            }

            return partsToCheck.Contains(bodyPart);
        }
        return false;
    }

    public void SetIsInVictoryTrigger(bool state)
    {
        isInVictoryTrigger = state;
    }

    public void ManageVictory()
    {
        if (isInVictoryTrigger && animFace != null)
        {
            SetFaceAnimState(eFaceAnim.VICTORY);
        }
    }

    public void SetEffortFaceWhenHeldByCharacter()
    {
        if (currentAnimState != eFaceAnim.EFFORT2 && currentAnimState != eFaceAnim.EFFORT3)
        {
            SetFaceAnimState(eFaceAnim.EFFORT2);
        }
        else
        {
            RePlayCurrentAnim();
        }
    }

    public void SetEffortFaceWhenReleasedByCharacter()
    {
        if (GetIsHeldByCharacters().Count <= 1 && !GetIsHoldingCharacter())
        {
            if (LeftHand != null && LeftHand.GetIsHooked() || RightHand != null && RightHand.GetIsHooked())
            {
                SetFaceAnimState(eFaceAnim.EFFORT1);
            }
            else
            {
                SetFaceAnimState(eFaceAnim.IDLE);
            }
        }
    }

    public bool GetIsHoldingCharacter()
    {
        return LeftHand != null && LeftHand.GetIsHooked() && LeftHand.GetHook().CompareTag("Character") || RightHand != null && RightHand.GetIsHooked() && RightHand.GetHook().CompareTag("Character");
    }

    public void Push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    /*public void SetIsIgnoringOtherCharacters(bool state)
    {
        if (LeftHand != null)
        {
            LeftHand.SetIsIgnoringOtherCharacters(state);
        }
        if (RightHand != null)
        {
            RightHand.SetIsIgnoringOtherCharacters(state);
        }
    }*/

    public static List<Character> GetCharactersHolding(GameObject obj)
    {
        List<Character> characters = new List<Character>();
        FixedJoint2D[] joints = obj.GetComponents<FixedJoint2D>();

        foreach (FixedJoint2D joint in joints)
        {
            Character character = GetCharacterFromGameObject(joint.connectedBody.gameObject);
            if (character != null && !characters.Contains(character))
            {
                characters.Add(character);
            }
        }

        return characters;
    }

    /*public static List<Hand> GetHoldingHands(Character character) hand no more part of Parts
    {
        List<Hand> hands = new List<Hand>();

        foreach (BodyPart part in character.Parts)
        {
            if (part == null)
                continue;

            FixedJoint2D[] joints = part.GetComponents<FixedJoint2D>();
            foreach (FixedJoint2D joint in joints)
            {
                Hand hand = joint.connectedBody.GetComponent<Hand>();
                if (hand != null && hand != character.LeftHand && hand != character.RightHand)
                {
                    hands.Add(hand);
                }
            }
        }

        return hands;
    }*/

    public void ReleaseCharactersInRadius()
    {
        float impulseForce = (timerInputImpulse * ReleaseImpulseForceOnItselfMax) / ReleaseImpulseTimeMax;
        rb.AddForce(Vector2.up * impulseForce, ForceMode2D.Impulse);
        if (LeftHand != null)
        {
            LeftHand.AddForce(LeftHand.transform.up * impulseForce, ForceMode2D.Impulse);
        }
        if (RightHand != null)
        {
            RightHand.AddForce(RightHand.transform.up * impulseForce, ForceMode2D.Impulse);
        }
        Vibrate(ReleaseImpulseVibrationFinish, ReleaseImpulseVibrationDurationFinish);

        Instantiate(BumpFXPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 359f)));

        List<Character> charactersInRadius = new List<Character>();
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, ReleaseImpulseRadius, transform.forward, float.MaxValue, LayerMask.GetMask("Characters"));
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Character"))
            {
                Character character = GetCharacterFromGameObject(hit.collider.gameObject);
                if (character != null && character != this)
                {
                    charactersInRadius.Add(character);
                    if (character.LeftHand != null && character.LeftHand.GetIsHooked() && character.LeftHand.GetHook().CompareTag("Character"))
                    {
                        Character heldCharacter = GetCharacterFromGameObject(character.LeftHand.GetHook().gameObject);
                        if (heldCharacter == this)
                        {
                            character.LeftHand.UnHook();
                            character.LeftHand.DisableHand(ReleaseImpulseDisableHandTime);
                        }
                    }
                    else if (character.RightHand != null && character.RightHand.GetIsHooked() && character.RightHand.GetHook().CompareTag("Character"))
                    {
                        Character heldCharacter = GetCharacterFromGameObject(character.RightHand.GetHook().gameObject);
                        if (heldCharacter == this)
                        {
                            character.RightHand.UnHook();
                            character.RightHand.DisableHand(ReleaseImpulseDisableHandTime);
                        }
                    }
                }
            }
        }
        ImpulseOnReleased(charactersInRadius);
    }

    void ImpulseOnReleased(List<Character> characters)
    {
        float impulseForce = (timerInputImpulse * ReleaseImpulseForceOnOthersMax) / ReleaseImpulseTimeMax;
        foreach (Character character in characters)
        {
            character.Push((character.transform.position - transform.position) * impulseForce);
            character.Vibrate(ReleaseImpulseVibrationOnOthersFinish, ReleaseImpulseVibrationDurationFinish);
        }
    }

    public Vector2 GetCurrentVelocity()
    {
        return rb.velocity;
    }

    public void StopVelocity()
    {
        rb.velocity = new Vector2(0f, 0f);
        foreach (Rigidbody2D rigidbody in rbs)
        {
            rigidbody.velocity = new Vector2(0f, 0f);
        }
    }

    /*public void RandomJump()
    {
        Vector2 dir = Quaternion.Euler(0, 0, Random.Range(-JumpConeAngle / 2f, JumpConeAngle / 2f)) * Vector2.up;
        rb.AddForce(dir * ForceJump, ForceMode2D.Impulse);
        animFace.SetTrigger("Point");
        soundModule.StopAllPlayingEvents();
        soundModule.PlayCharacterSound(CharacterSoundModule.eState.POINT);
    }*/

    public void ReactivateAfterMinigame()
    {
        cam = Camera.main.GetComponent<CameraEffects>();
    }

    public void SetIsElectric(bool state)
    {
        if (!state)
        {
            soundModule.StopElectric();
        }
        isElectric = state;
    }

    public bool GetIsElectric()
    {
        return isElectric;
    }

    public void Pause()
    {
        isPaused = true;
        soundModule.PauseSounds();
        StopVibration();
    }

    public void UnPause()
    {
        isPaused = false;
        soundModule.UnPauseSounds();
        if (isInputImpulsePressing)
        {
            StartVibrate((timerReleaseImpulseCoolDown * ReleaseImpulseVibrationMax) / ReleaseImpulseTimeMax);
        }

        if (LeftHand != null && (!player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabLeft) || !LeftHand.GetIsHooked()) && LeftHand.GetIsClosed())
        {
            LeftHand.UnHook();
        }
        if (RightHand != null && (!player.GetControls().GetPlayerInput().GetButton(RewiredConsts.Action.Character_GrabRight) || !RightHand.GetIsHooked()) && RightHand.GetIsClosed())
        {
            RightHand.UnHook();
        }
    }

    void OnDestroy()
    {
        soundModule.StopAllPlayingEvents();
        if (isInputImpulsePressing)
        {
            StopVibration();
        }
    }
}
