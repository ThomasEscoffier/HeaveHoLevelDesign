using UnityEngine;

public class TimedBomb : MonoBehaviour {

    public float TimeBeforeExplode = 10f;
    float timer = 0f;

    public float ExplosionForce = 100f;
    public float ExplosionRadius = 1f;

    bool isStarted = false;

    void Start()
    {
        StartTimer();    
    }

    void Update()
    {
        if (isStarted)
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeExplode);
            if (timer >= TimeBeforeExplode)
            {
                Explode();
            }
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        isStarted = true;
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
}
