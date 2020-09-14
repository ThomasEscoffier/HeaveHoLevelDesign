using UnityEngine;

public class DestroyAfterAnim : MonoBehaviour {

    Animator anim = null;
    float animLength = 0f;
    float timer = 0f;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>(true);
        animLength = anim.GetCurrentAnimatorStateInfo(0).length;
    }

    void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, animLength);
        if (timer == animLength)
        {
            Destroy(gameObject);
        }
	}
}
