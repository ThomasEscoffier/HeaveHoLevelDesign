using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatformWithLimits : MonoBehaviour {

    public float LowerAngle = 0f;
    public float UpperAngle = 359f;

    public float Speed = 0f;
    public float TimeWait = 0f;

    HingeJoint2D joint;
    bool isWaiting = false;
    float timer = 0f;

	void Awake()
    {
        joint = GetComponent<HingeJoint2D>();
        joint.useMotor = true;
        joint.useLimits = true;
        JointAngleLimits2D limits = joint.limits;
        limits.min = LowerAngle;
        limits.max = UpperAngle;
        joint.limits = limits;
        SetSpeed(Speed);
    }
	
	void Update ()
    {
        if (isWaiting)
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeWait);
            if (timer >= TimeWait)
            {
                timer = 0f;
                isWaiting = false;
                if (joint.jointAngle <= LowerAngle)
                {
                    SetSpeed(Speed);
                }
                else if (joint.jointAngle >= UpperAngle)
                {
                    SetSpeed(Speed * -1);
                }
            }
        }
        else
        {
            if (joint.jointAngle <= LowerAngle || joint.jointAngle >= UpperAngle)
            {
                isWaiting = true;
            }
        }
	}

    void SetSpeed(float speed)
    {
        JointMotor2D motor = joint.motor;
        motor.motorSpeed = speed;
        joint.motor = motor;
    }
}
