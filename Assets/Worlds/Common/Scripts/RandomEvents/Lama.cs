using UnityEngine;

public class Lama : RandomMovingObject
{
    public float PercentScreenAppearParticles = 0.75f;
    public ParticleSystem ParticlesPrefab = null;
    public Transform SpawnPos = null;

    bool isParticleSpawned = false;
    Vector3 posTocheck = Vector3.zero;
    Camera cam = null;
    SoundModule soundModule = null;

    public override void Start()
    {
        cam = Camera.main;
        soundModule = GetComponent<SoundModule>();
        base.Start();
        posTocheck = cam.ScreenToWorldPoint(new Vector3(Screen.width * PercentScreenAppearParticles, 0f, 0f));
    }

    public override void Init()
    {
        base.Init();
        isParticleSpawned = false;  
    }

    public override void UpdatePlaying()
    {
        base.UpdatePlaying();
        if (!isParticleSpawned && transform.position.x > posTocheck.x)
        {
            Instantiate(ParticlesPrefab, SpawnPos.position, SpawnPos.rotation, transform);
            soundModule.PlayOneShot("Fart");
            isParticleSpawned = true;
        }
    }

    public override void Finish()
    {
        base.Finish();
    }
}