using System.Collections.Generic;
using UnityEngine;

public class RotatingWheel : MonoBehaviour {

    [System.Serializable]
    public struct WheelForceTorque
    {
        public uint NumberPlayers;
        public float Speed;
        public float MaxForceTorque;

        public WheelForceTorque(uint numberPlayers, float minForceTorque, float maxForceTorque)
        {
            NumberPlayers = numberPlayers;
            Speed = minForceTorque;
            MaxForceTorque = maxForceTorque;
        }
    }

    public WheelForceTorque[] WheelForcesByPlayers;

    public float MinSpeedSoundRolling = 5f;
    public float MaxSpeedSoundRolling = 40f;

    int currentNumberPlayers = 0;
    HingeJoint2D wheelJoint = null;
    Rigidbody2D rb = null;
    SoundModule soundModule = null;
    bool isRollingSoundPlaying = false;

    const float rollingMinValue = 2f;
    const float rollingMaxValue = 8f;

    void Awake()
    {
        wheelJoint = GetComponent<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        soundModule = GetComponent<SoundModule>();

        //soundModule.InitEvent("Roll");
        //soundModule.AddParameter("Roll", "Speed", 0f);
    }

    void Update()
    {
        int newNumberPlayers = GetHoldingCharacters().Count;
        if (newNumberPlayers != currentNumberPlayers)
        {
            currentNumberPlayers = newNumberPlayers;
            for (int i = 0; i < WheelForcesByPlayers.Length; ++i)
            {
                if (WheelForcesByPlayers[i].NumberPlayers == currentNumberPlayers)
                {
                    JointMotor2D newMotor = new JointMotor2D();
                    newMotor.motorSpeed = WheelForcesByPlayers[i].Speed;
                    newMotor.maxMotorTorque = WheelForcesByPlayers[i].MaxForceTorque;
                    wheelJoint.motor = newMotor;
                }
            }
        }

        //UpdateRolling();
    }

    public List<Character> GetHoldingCharacters()
    {
        List<Character> characters = new List<Character>();
        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();

        foreach (FixedJoint2D joint in joints)
        {
            Hand hand = joint.connectedBody.GetComponent<Hand>();
            if (hand != null && !characters.Contains(hand.GetParentCharacter()))
            {
                characters.Add(hand.GetParentCharacter());
                List<Character> charactersAttached = new List<Character>();
                GameManager.Instance.GetChainsManager().GetCharactersInChain(hand.gameObject, new List<ChainsManager.Node>(), ref charactersAttached);

                if (charactersAttached.Count > 0)
                {
                    for(int i = 0; i < charactersAttached.Count; ++i)
                    {
                        if (!characters.Contains(charactersAttached[i]))
                        {
                            characters.Add(charactersAttached[i]);
                        }
                    }
                }
            }
        }

        return characters;
    }

    void UpdateRolling()
    {
        if (Mathf.Abs(rb.angularVelocity) > MinSpeedSoundRolling)
        {
            soundModule.SetParameterValue("Roll", "Speed", Mathf.Clamp(((rb.angularVelocity - MinSpeedSoundRolling) / MaxSpeedSoundRolling) * rollingMaxValue, rollingMinValue, rollingMaxValue));
            soundModule.AttachInstanceTo("Roll", transform, rb);
            if (!isRollingSoundPlaying)
            {
                soundModule.PlayEvent("Roll");
                isRollingSoundPlaying = true;
            }
        }
        else
        {
            soundModule.StopEvent("Roll");
            isRollingSoundPlaying = false;
        }
    }

    void OnDestroy()
    {
        //soundModule.TerminateEventInstance("Roll");
    }
}
