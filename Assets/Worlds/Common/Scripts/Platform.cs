using UnityEngine;

public class Platform : MonoBehaviour
{
    public enum ePlatformType
    {
        NONE,
        GRASS,
        ROCK,
        DIRT,
        METAL,
    }

    public ePlatformType Type = ePlatformType.NONE;
    public ParticleSystem DustFXPrefab = null;
    public float ForceCollisionMinFX = 5f;
    public float MinEmission = 0f;
    public float MaxEmission = 200f;
    public float MaxVelocity = 50f;
    [Tooltip("Use this with moving platform to separate paints")]
    public int PlatformID = 0;

    public bool UseRotatingSound = false;
    public float MinSpeedSoundRolling = 5f;
    public float MaxSpeedSoundRolling = 40f;

    SpriteRenderer spriteRend = null;
    float lastPlay;
    private static readonly int Stencil = Shader.PropertyToID("_Stencil");
    const float playCooldown = 1.0f;

    Rigidbody2D rb = null;
    SoundModule soundModule = null;
    bool isRollingSoundPlaying = false;

    const float rollingMinValue = 2f;
    const float rollingMaxValue = 8f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        soundModule = GetComponent<SoundModule>();
        if (UseRotatingSound)
        {
            soundModule.InitEvent("Roll");
            soundModule.AddParameter("Roll", "Speed", 0f);
        }

        spriteRend = GetComponentInChildren<SpriteRenderer>();
        if (spriteRend.material.HasProperty(Stencil))
        {
            spriteRend.material.SetFloat(Stencil, spriteRend.material.GetFloat(Stencil) - PlatformID);
        }
    }

    void Update()
    {
        if (UseRotatingSound)
        {
            UpdateRolling();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (DustFXPrefab == null || lastPlay + playCooldown > Time.time)
            return;
        
        if ((collision.collider.CompareTag("Character") || collision.collider.CompareTag("Collectible"))
            && collision.relativeVelocity.magnitude > ForceCollisionMinFX)
        {
            ParticleSystem particles = Instantiate(DustFXPrefab, collision.GetContact(0).point, Quaternion.Euler(0f, 0f, Random.Range(0f, 359f)));
            ParticleSystem.MainModule main = particles.main;
            main.startColor = spriteRend.color;
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = (collision.relativeVelocity.magnitude / MaxVelocity) * MaxEmission;
            particles.GetComponent<ParticleSystemRenderer>().sortingOrder = spriteRend.sortingOrder;
            lastPlay = Time.time;
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

    void OnDestroy()
    {
        if (UseRotatingSound)
        {
            soundModule.TerminateEventInstance("Roll");
        }
    }
}
