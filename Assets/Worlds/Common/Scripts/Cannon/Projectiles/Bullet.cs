using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float timer = 3f;
    public bool coin = false;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider.tag);

        if (!collision.collider.CompareTag("Character"))
        {
            if (!coin)
            {
                if(collision.collider.CompareTag("Destroyer"))
                    Destroy(gameObject);
            }
        }
    }


    private void Update()
    {
        timer -= 1 * Time.deltaTime;

        if(!coin)
        { 

            if(timer <= 0)
            {
                Destroy(gameObject);
            }
        }

    }
}
