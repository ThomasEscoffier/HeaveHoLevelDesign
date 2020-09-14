using UnityEngine;

public class WaterBalloon : RespawnableItem {

    public float MaxVelocityBeforeBreak = 20f;
    public Material PaintOnLDMaterial;
    public Material PaintOnCharacterHeadMaterial;
    public Material PaintOnCharacterLeftArmMaterial;
    public Material PaintOnCharacterLeftForearmMaterial;
    public Material PaintOnCharacterRightArmMaterial;
    public Material PaintOnCharacterRightForearmMaterial;
    public PaintSplash[] SplashPrefabs = null;
    public ParticleSystem CollisionEffectPrefab = null;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.magnitude >= MaxVelocityBeforeBreak
            && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Grab")
            || collision.gameObject.CompareTag("Character")|| collision.gameObject.CompareTag("Collectible") || collision.gameObject.CompareTag("Toy")))
        {
            Instantiate(CollisionEffectPrefab, transform.position, Quaternion.Euler(0f, 90f, 0f));

            Vector2 direction = collision.GetContact(0).normal;
            float newRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            PaintSplash splash = Instantiate(SplashPrefabs[Random.Range(0, SplashPrefabs.Length)], collision.GetContact(0).point, Quaternion.Euler(0f, 0f, newRot), collision.transform);

            SpriteRenderer collisionSprite = collision.gameObject.GetComponent<SpriteRenderer>();
            splash.GetSpriteRenderer().sortingLayerName = collisionSprite.sortingLayerName;
            if (splash.GetSpriteRenderer().sortingLayerName.Equals("LD"))
            {
                splash.GetSpriteRenderer().material = PaintOnLDMaterial;
                splash.GetSpriteRenderer().sortingOrder = collisionSprite.sortingOrder + 1;
            }
            else if (splash.GetSpriteRenderer().sortingLayerName.Equals("Character"))
            {
                if (collisionSprite.sortingOrder >= 3 && collisionSprite.sortingOrder <= 8)
                {
                    splash.GetSpriteRenderer().material = PaintOnCharacterHeadMaterial;
                    splash.GetSpriteRenderer().sortingOrder = 9;
                }
                else if (collisionSprite.sortingOrder >= 0 && collisionSprite.sortingOrder <= 2)
                {
                    if (collisionSprite.gameObject.name.Contains("Left"))
                    {
                        splash.GetSpriteRenderer().material = PaintOnCharacterLeftArmMaterial;
                    }
                    else
                    {
                        splash.GetSpriteRenderer().material = PaintOnCharacterRightArmMaterial;
                    }
                    splash.GetSpriteRenderer().sortingOrder = 3;
                }
                else if (collisionSprite.sortingOrder >= 9 && collisionSprite.sortingOrder <= 11
                    || collisionSprite.sortingOrder >= 12 && collisionSprite.sortingOrder <= 14)
                {
                    if (collisionSprite.gameObject.name.Contains("Left"))
                    {
                        splash.GetSpriteRenderer().material = PaintOnCharacterLeftForearmMaterial;
                    }
                    else
                    {
                        splash.GetSpriteRenderer().material = PaintOnCharacterRightForearmMaterial;
                    }
                    splash.GetSpriteRenderer().sortingOrder = 15;
                }
            }
            Deactivate();
        }
    }
}
