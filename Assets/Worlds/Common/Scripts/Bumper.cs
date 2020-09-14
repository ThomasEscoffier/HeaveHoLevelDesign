using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    public float Force = 1000f;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Character")
        {
            anim.SetTrigger("Launch");
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * Force);
        }
    }
}
