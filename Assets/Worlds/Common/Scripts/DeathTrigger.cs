using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    protected BloodParticles bloodParticles;
    protected SoundModule soundModule;

    public virtual void Awake()
    {
        soundModule = GetComponent<SoundModule>();
    }

    public virtual void Start() { }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            Character obj = collision.GetComponent<Character>();
            if (obj != null)
            {
                Vector2 direction = obj.transform.position - transform.position;
                bloodParticles = obj.SpawnParticles(obj.transform.position, direction, true);
                obj.SpawnDeathFX(collision.transform.position, Quaternion.identity);
                obj.Die();
                obj.DeathVibrate();
                if (soundModule != null)
                {
                    soundModule.PlayOneShot("Death", gameObject);
                }
                return;
            }
        }

        if (collision.CompareTag("Grab")) // heart balloon have tag Grab to allow them to fly
        {
            RandomHelpObject helpObj = collision.GetComponent<RandomHelpObject>();
            if (helpObj != null)
            {
                helpObj.Die();
                return;
            }
        }

        if (collision.CompareTag("Collectible"))
        {
            RespawnableItem collectible = collision.GetComponent<RespawnableItem>();
            if (collectible != null && !collectible.GetIsStatic())// && !collectible.GetIsTaken())
            {
                collectible.SpawnDestroyParticles(collision.transform.position);
                collectible.Deactivate(true);
            }
        }
    }
}
