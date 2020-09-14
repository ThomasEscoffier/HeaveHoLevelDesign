using UnityEngine;

public class RandomPoopingBird : RandomLevelEvent
{
    public BloodParticles PoopParticlesPrefab = null;
    public Transform PoopSpawnPoint = null;
    public float ProjectileCooldown = 5f;
    public float Speed = 10f;
    public bool IsMovingLeft = false;
    public Vector3 RaycastOffset = Vector3.zero;
    public float MinDelayBetweenSeagullSounds = 3f;
    public float MaxDelayBetweenSeagullSounds = 4f;

    public float TimeBeforePooping = 1f;

    Vector3 startPos = Vector3.zero;
    float timerCoolDown = 0f;
    float timeBeforeNextSound = 1f;
    float timerSound = 0f;
    float timerPoop = 0f;

    bool hasToPoop = false;

    SpriteRenderer spriteRend = null;
    SoundModule sound = null;
    Animator anim = null;

    Camera cachedMainCamera;

    public override void Start()
    {
        cachedMainCamera = Camera.main;
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        sound = GetComponentInChildren<SoundModule>();
        anim = GetComponentInChildren<Animator>();
        startPos = transform.position;
        base.Start();
    }

    public override void Init()
    {
        base.Init();
        anim.enabled = false;
        spriteRend.enabled = false;
        spriteRend.flipX = IsMovingLeft;
    }

    public override void Play()
    {
        base.Play();
        anim.enabled = true;
        spriteRend.enabled = true;
        sound.PlayEvent("Shout");
        timeBeforeNextSound = Random.Range(MinDelayBetweenSeagullSounds, MaxDelayBetweenSeagullSounds);
    }

    public override void UpdatePlaying()
    {
        transform.position += new Vector3(Time.deltaTime * (IsMovingLeft ? -Speed : Speed), 0, 0);

        if (timerCoolDown == 0)
        {
            if (hasToPoop)
            {
                timerPoop = Mathf.Min(timerPoop + Time.deltaTime, TimeBeforePooping);
                if (timerPoop == TimeBeforePooping)
                {
                    Poop();
                    hasToPoop = false;
                    timerPoop = 0f;
                }
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position + RaycastOffset, Vector2.down, float.MaxValue, 1 << LayerMask.NameToLayer("Characters"));
                if (hit.collider != null && hit.collider.CompareTag("Character"))
                {
                    hasToPoop = true;
                }
            }
        }
        else
        {
            sound.AttachInstanceTo("Shout", transform, null);
            timerCoolDown = Mathf.Max(timerCoolDown - Time.deltaTime, 0);
        }
    }

    void Poop()
    {
        BloodParticles particles = Instantiate(PoopParticlesPrefab, PoopSpawnPoint.transform.position, PoopSpawnPoint.rotation, transform);
        ParticleSystem.MainModule main = particles.GetComponent<ParticleSystem>().main;
        particles.SetColorMinMax(main.startColor.colorMin, main.startColor.colorMax);
        sound.PlayOneShot("Poop", gameObject);
        timerCoolDown = ProjectileCooldown;
    }

    public override bool GetIsFinished()
    {
        Vector2 point = IsMovingLeft ? cachedMainCamera.WorldToScreenPoint(transform.position + new Vector3(spriteRend.size.x / 2f, 0, 0)) :
            cachedMainCamera.WorldToScreenPoint(transform.position - new Vector3(spriteRend.size.x / 2f, 0, 0));
        return IsMovingLeft ? point.x < 0 : point.x > Screen.width;
    }

    public override void Finish()
    {
        anim.enabled = false;
        timerCoolDown = 0f;
        transform.position = startPos;
        spriteRend.enabled = false;
        spriteRend.flipX = IsMovingLeft;
        sound.StopEvent("Shout");
        base.Finish();
    }
}