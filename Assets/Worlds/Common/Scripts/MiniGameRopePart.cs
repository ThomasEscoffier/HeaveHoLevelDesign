using UnityEngine;

public class MiniGameRopePart : MonoBehaviour
{
    SpriteRenderer spriteRend = null;
    Rigidbody2D rb = null;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.enabled = false;
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetSpriteOn(bool state)
    {
        spriteRend.enabled = state;
        gameObject.tag = "Grab";
    }

    public void ChangeRigibodyType(RigidbodyType2D type2D)
    {
        rb.bodyType = type2D;
    }
}
