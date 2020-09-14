using System.Collections.Generic;
using UnityEngine;

public class BloodParticles : AutoDestroyParticle {

    public int MaxSplashesByPlatform = 4;
    public float ScaleSplashes = 1f;

    public Material PaintMaterial;
    public Material SpriteDefaultMaterial;
    public PaintSplash[] SplashPrefabs = null;

    public float ForceImpact = 100f;

    SoundModule soundModule;

    class ObjectTouched
    {
        public GameObject obj = null;
        public int nbTouched = 0;

        public ObjectTouched(GameObject newObj)
        {
            obj = newObj;
        }
    }

    List<ObjectTouched> objectToucheds = new List<ObjectTouched>();

    ParticleSystem[] bloodParticles;

    Color currentColor = Color.white;
    bool useGradient = false;
    Gradient currentGradient = null;

    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void Awake()
    {
        bloodParticles = GetComponentsInChildren<ParticleSystem>();
        soundModule = GetComponent<SoundModule>();
    }

    void OnParticleCollision(GameObject other)
    {
        for (int j = 0; j < bloodParticles.Length; ++j)
        {
            if (bloodParticles[j] == null)
                continue;

            ParticlePhysicsExtensions.GetCollisionEvents(bloodParticles[j], other, collisionEvents);

            for (int i = 0; i < collisionEvents.Count; i++)
            {
                if (collisionEvents[i].colliderComponent == null || collisionEvents[i].colliderComponent.gameObject == null
                    || collisionEvents[i].colliderComponent.gameObject.CompareTag("Collectible") || collisionEvents[i].colliderComponent.gameObject.CompareTag("Toy"))
                    continue;

                ObjectTouched platform = objectToucheds.Find(x => x.obj == collisionEvents[i].colliderComponent.gameObject);
                if (platform != null && platform.obj != null)
                {
                    if (platform.nbTouched < MaxSplashesByPlatform)
                    {
                        SpawnSplash(collisionEvents[i]);
                        platform.nbTouched++;
                    }
                }
                else
                {
                    ObjectTouched newPlatform = new ObjectTouched(collisionEvents[i].colliderComponent.gameObject);
                    SpawnSplash(collisionEvents[i]);
                    newPlatform.nbTouched++;
                    objectToucheds.Add(newPlatform);
                    soundModule.PlayOneShot("Splatter", collisionEvents[i].intersection);
                }
            }
        }
    }

    public void SetColor(Color characterColor)
    {
        useGradient = false;
        currentColor = characterColor;
        for (int i = 0; i < bloodParticles.Length; ++i)
        {
            ParticleSystem.MainModule main = bloodParticles[i].main;
            main.startColor = currentColor;
        }
    }

    public void SetColorMinMax(Color colorMin, Color colorMax)
    {
        useGradient = true;
        currentGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(colorMin, 0f);
        colorKeys[1] = new GradientColorKey(colorMax, 1f);

        GradientAlphaKey[] colorAlphaKeys = new GradientAlphaKey[1];
        colorAlphaKeys[0] = new GradientAlphaKey(1f, 0f);

        currentGradient.SetKeys(colorKeys, colorAlphaKeys);
    }

    public void SetParticlesOrientation(float rotation)
    {
        for (int i = 0; i < bloodParticles.Length; ++i)
        {
            ParticleSystem.MainModule main = bloodParticles[i].main;
            main.startRotation = rotation * Mathf.Deg2Rad;
        }
    }

    void SpawnSplash(ParticleCollisionEvent collisionEvent)
    {
        if (collisionEvent.colliderComponent == null)
            return;
        SpriteRenderer collisionSprite = collisionEvent.colliderComponent.gameObject.GetComponentInChildren<SpriteRenderer>();
        if (collisionSprite == null || collisionSprite.material.shader == SpriteDefaultMaterial.shader)
            return;

        PaintSplash splash = Instantiate(SplashPrefabs[Random.Range(0, SplashPrefabs.Length)], collisionEvent.intersection, Quaternion.identity, collisionEvent.colliderComponent.transform);
        splash.transform.localRotation = Quaternion.identity;
        splash.transform.localScale = new Vector3(ScaleSplashes / collisionEvent.colliderComponent.transform.localScale.x, ScaleSplashes / collisionEvent.colliderComponent.transform.localScale.y, 1f) * (Random.Range(0, 2) == 0 ? -1f : 1f);

        if (useGradient)
        {
            splash.SetColor(currentGradient.Evaluate(Random.value));
        }
        else
        {
            splash.SetColor(currentColor);
        }

        splash.GetSpriteRenderer().sortingLayerName = collisionSprite.sortingLayerName;
        splash.GetSpriteRenderer().material = PaintMaterial;
        if (splash.GetSpriteRenderer().sortingLayerName.Equals("LD"))
        {
            splash.GetSpriteRenderer().sortingOrder = collisionSprite.sortingOrder + 1;
            splash.GetSpriteRenderer().material.SetFloat("_Stencil", collisionSprite.material.GetFloat("_Stencil"));
        }
        else if (splash.GetSpriteRenderer().sortingLayerName.Equals("Character"))
        {
            collisionEvent.colliderComponent.gameObject.GetComponent<Rigidbody2D>().AddForce(-collisionEvent.normal * ForceImpact);
            if (collisionSprite.sortingOrder >= 3 && collisionSprite.sortingOrder <= 8)
            {
                splash.GetSpriteRenderer().sortingOrder = 9;
            }
            else if (collisionSprite.sortingOrder >= 0 && collisionSprite.sortingOrder <= 2)
            {
                splash.GetSpriteRenderer().sortingOrder = 3;
            }
            else if (collisionSprite.sortingOrder >= 9 && collisionSprite.sortingOrder <= 11
                || collisionSprite.sortingOrder >= 12 && collisionSprite.sortingOrder <= 15)
            {
                splash.GetSpriteRenderer().sortingOrder = 16;
            }
            splash.GetSpriteRenderer().material.SetFloat("_Stencil", collisionSprite.material.GetFloat("_Stencil"));
        }
    }
}
