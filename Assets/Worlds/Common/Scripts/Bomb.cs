using UnityEngine;

public class Bomb : MonoBehaviour {

    public float ExplosionForce = 100f;
    public float ExplosionRadius = 1f;
    public float VelocityChocExplode = 10f;

    Rigidbody2D rb = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Explode()
    {
        UnHookHoldingHands();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), ExplosionRadius);
        foreach (Collider2D col in colliders)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Rigidbody2DExtension.AddExplosionForce(rb, ExplosionForce, transform.position, ExplosionRadius);
            }
        }
        Destroy(gameObject);
    }

    public void UnHookHoldingHands()
    {
        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();

        foreach (FixedJoint2D joint in joints)
        {
            Hand hand = joint.connectedBody.GetComponent<Hand>();
            if (hand != null)
            {
                hand.UnHook();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.magnitude >= VelocityChocExplode)
        {
            Explode();
        }
    }
}
