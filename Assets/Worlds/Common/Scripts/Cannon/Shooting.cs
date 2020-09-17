using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public Transform firePoint;
    public Animator cannonAnimator;
    public GameObject bulletPrefab;
    public bool coinCannon = false;

    public float bulletForce = 20f;
    public float fireRate = 3f;
    private float timer;
    private GameObject bullet;
    private bool fired;

    void Start()
    {
        timer = fireRate;

        cannonAnimator = gameObject.GetComponent<Animator>();

    }


    // Update is called once per frame
    void Update()
    {

        if(!fired)
        {
            cannonAnimator.SetBool("Charging", true);
        }

        timer -= 1 * Time.deltaTime;

        if (coinCannon)
        {
            if (bullet == null)
            {

                if (timer <= 0)
                {
                    fired = true;
                    Shoot();
                }

            }
        }else
        {
            if (timer <= 0)
            {
                fired = true;
                Shoot();
            }
        }

        
    }

    void Shoot()
    {
        cannonAnimator.SetBool("Charging", false);

        bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        fired = false;
        timer = fireRate;
    }



}
