using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float TimeWait = 0f;
    public float Speed = 0f;

    float timer = 0f;
    SliderJoint2D sliderJoint = null;

    enum eState
    {
        IS_FIRST,
        MOVING_FIRST,
        IS_END,
        MOVING_END,
    }

    eState currentState = eState.IS_FIRST;

    void Awake()
    {
        sliderJoint = GetComponent<SliderJoint2D>();
    }

	void Update()
    {
		switch (currentState)
        {
            case eState.IS_FIRST:
                timer = Mathf.Min(timer + Time.deltaTime, TimeWait);
                if (timer == TimeWait)
                {
                    currentState = eState.MOVING_END;
                    JointMotor2D motor = new JointMotor2D();
                    motor.motorSpeed = -Speed;
                    motor.maxMotorTorque = sliderJoint.motor.maxMotorTorque;
                    sliderJoint.motor = motor;
                    timer = 0f;
                }
                break;
            case eState.MOVING_FIRST:
                if (sliderJoint.limitState == JointLimitState2D.UpperLimit)
                {
                    currentState = eState.IS_FIRST;
                }
                break;
            case eState.IS_END:
                timer = Mathf.Min(timer + Time.deltaTime, TimeWait);
                if (timer == TimeWait)
                {
                    currentState = eState.MOVING_FIRST;
                    JointMotor2D motor = new JointMotor2D();
                    motor.motorSpeed = Speed;
                    motor.maxMotorTorque = sliderJoint.motor.maxMotorTorque;
                    sliderJoint.motor = motor;
                    timer = 0f;
                }
                break;
            case eState.MOVING_END:
                if (sliderJoint.limitState == JointLimitState2D.LowerLimit)
                {
                    currentState = eState.IS_END;
                }
                break;
        }
	}
}
