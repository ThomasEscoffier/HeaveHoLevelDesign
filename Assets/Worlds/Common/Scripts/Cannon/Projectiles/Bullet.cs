using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float timer = 3f;
    public bool coin = false;
    public bool pic = false;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (pic)
        {
            Destroy(gameObject);
        }
        else
        {
            if (collision.collider.CompareTag("Destroyer"))
            {
                if (!coin)
                {
                    Destroy(gameObject);
                }
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
