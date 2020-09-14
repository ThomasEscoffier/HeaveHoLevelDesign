using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletPrefab;
    public bool coinCannon = false;

    public float bulletForce = 20f;
    public float fireRate = 3f;
    private float timer;
    private GameObject bullet;


    void Start()
    {
        timer = fireRate;
    }


    // Update is called once per frame
    void Update()
    {

        timer -= 1 * Time.deltaTime;

        if(coinCannon)
        {
            if (bullet == null)
            {

                if (timer <= 0)
                {
                    Shoot();
                }

            }
        }else
        {
            if (timer <= 0)
            {
                 Shoot();
            }
        }

        
    }

    void Shoot()
    {
        bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        timer = fireRate;
    }



}
