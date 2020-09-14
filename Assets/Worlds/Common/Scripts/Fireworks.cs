using UnityEngine;

public class Fireworks : MonoBehaviour
{
    public float MinRandom = 0f;
    public float MaxRandom = 1f;

    float timer = 0f;
    float randomTime = 0f;

    Animator anim = null;
    bool isLaunched = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isLaunched)
        {
            timer = Mathf.Min(timer + Time.deltaTime, randomTime);
            if (timer == randomTime)
            {
                anim.SetTrigger("Launch");
                isLaunched = false;
            }
        }
    }

    public void StartFireworks()
    {
        randomTime = Random.Range(MinRandom, MaxRandom);
        isLaunched = true;
    }
}
