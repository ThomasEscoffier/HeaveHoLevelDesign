using UnityEngine;

public class Rope : MonoBehaviour
{
    public Rigidbody2D LastPieceOfRopeRB = null;

    public float VelocityMinTriggerSound = 20f;
    public float TimeBetweenSounds = 2f;

    float timer = 0f;

    SoundModule sound = null;

    void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, TimeBetweenSounds);
        if (timer == TimeBetweenSounds)
        {
            if (LastPieceOfRopeRB.velocity.magnitude > VelocityMinTriggerSound)
            {
                //sound.PlayOneShot("RopeFast");
            }
        }
    }
}
