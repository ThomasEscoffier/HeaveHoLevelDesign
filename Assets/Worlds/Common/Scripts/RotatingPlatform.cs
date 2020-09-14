using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour {

    public float SpeedMin = 1f;
    public float SpeedMax = 1f;

    public float TimeAtMinSpeed = 0f;
    public float TimeAtMaxSpeed = 0f;

    public float TimeAccelerate = 0f;
    public float TimeDecelerate = 0f;

    public bool OnlyAccelerateOnce = false;
    public bool OnlyDecelerateOnce = false;

    public bool StartAtMaxSpeed = false;

    HingeJoint2D joint;
    bool isAtMaxSpeed = false;
    bool isWaiting = false;

    float currentSpeed = 0f;
    float timer = 0f;

    bool isFinish = false;

	void Awake()
    {
        joint = GetComponent<HingeJoint2D>();
        joint.useMotor = true;
        if (StartAtMaxSpeed)
        {
            currentSpeed = SpeedMax;
            isAtMaxSpeed = true;
        }
        else
        {
            currentSpeed = SpeedMin;
        }
        SetSpeed(currentSpeed);
    }
	
	void Update ()
    {
        if (isFinish)
            return;

		if (isAtMaxSpeed)
        {
            if (isWaiting)
            {
                timer = Mathf.Min(timer + Time.deltaTime, TimeAtMaxSpeed);
                if (timer >= TimeAtMaxSpeed)
                {
                    timer = 0f;
                    isWaiting = false;
                }
            }
            else
            {
                timer = Mathf.Min(timer + Time.deltaTime, TimeDecelerate);
                currentSpeed = Mathf.Lerp(SpeedMax, SpeedMin, timer / TimeDecelerate);
                if (timer >= TimeDecelerate)
                {
                    currentSpeed = SpeedMin;
                    timer = 0f;
                    isAtMaxSpeed = false;
                    isWaiting = true;
                    if (OnlyDecelerateOnce)
                    {
                        isFinish = true;
                    }
                }
            }
        }
        else
        {
            if (isWaiting)
            {
                timer = Mathf.Min(timer + Time.deltaTime, TimeAtMinSpeed);
                if (timer >= TimeAtMinSpeed)
                {
                    timer = 0f;
                    isWaiting = false;
                }
            }
            else
            {
                timer = Mathf.Min(timer + Time.deltaTime, TimeAccelerate);
                currentSpeed = Mathf.Lerp(SpeedMin, SpeedMax, timer/TimeAccelerate);
                if (timer >= TimeAccelerate)
                {
                    currentSpeed = SpeedMax;
                    timer = 0f;
                    isAtMaxSpeed = true;
                    isWaiting = true;
                    if (OnlyAccelerateOnce)
                    {
                        isFinish = true;
                    }
                }
            }
        }
        SetSpeed(currentSpeed);
	}

    void SetSpeed(float speed)
    {
        JointMotor2D motor = joint.motor;
        motor.motorSpeed = speed;
        joint.motor = motor;
    }
}
