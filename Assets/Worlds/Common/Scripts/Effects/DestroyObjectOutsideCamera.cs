using UnityEngine;

public class DestroyObjectOutsideCamera : MonoBehaviour {

    public float Force = 1f;

    void Awake()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Force, ForceMode2D.Impulse);
        }
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (!LevelManager.IsObjectInsideCamera(transform.GetChild(i).transform.position))
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
