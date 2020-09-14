using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour {

    protected ParticleSystem[] particles;

    void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>(true);
    }

	void Update()
    {
        bool toBeDestroyed = true;
        for (int i = 0; i < particles.Length; ++i)
        {
            if (particles[i].IsAlive())
            {
                toBeDestroyed = false;
                return;
            }
        }
        if (toBeDestroyed)
        {
            Destroy(gameObject);
        }
    }
}
