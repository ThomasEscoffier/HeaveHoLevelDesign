using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour {

    public bool IsLeft = false;
    public float ForceBreakArm = 50000;
    public GameObject Forearm = null;
    public Hand CorrespondingHand = null;

    bool isUnderTension = false;
    Character parentCharacter;
    HingeJoint2D joint;

    DistanceJoint2D distanceJoint;

    void Awake()
    {
        joint = GetComponent<HingeJoint2D>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        
        parentCharacter = transform.parent.GetComponent<Character>();
    }
    
    public HingeJoint2D GetCharacterJoint()
    {
        return joint;
    }

    public Character GetParentCharacter()
    {
        return parentCharacter;
    }

    public Hand GetHand()
    {
        return CorrespondingHand;
    }

    public Arm GetOtherArm()
    {
        if (parentCharacter == null)
            return null;
        return IsLeft ? parentCharacter.RightArm : parentCharacter.LeftArm;
    }

    public List<Arm> GetIsArmHeldByOtherCharacter()
    {
        List<Arm> arms = new List<Arm>();
        List<FixedJoint2D> joints = new List<FixedJoint2D>();
        joints.AddRange(GetHand().GetComponents<FixedJoint2D>()); //Hand
        joints.AddRange(Forearm.GetComponents<FixedJoint2D>()); //Forearm
        joints.AddRange(GetComponents<FixedJoint2D>()); //Arm

        foreach (FixedJoint2D joint in joints)
        {
            if (joint.connectedBody && !parentCharacter.IsGameObjectFromCharacter(joint.connectedBody.gameObject))
            {
                Hand hand = joint.connectedBody.GetComponent<Hand>();
                if (hand != null && hand.GetArm() != null)
                {
                    arms.Add(hand.GetArm());
                }
            }
        }

        return arms;
    }

    public static Arm GetArmFromGameObject(GameObject obj)
    {
        if (obj.CompareTag("Character"))
        {
            Arm arm = obj.GetComponent<Arm>();
            Hand hand = obj.GetComponent<Hand>();

            if (arm != null)
            {
                return arm;
            }
            else if (hand != null)
            {
                return hand.GetArm();
            }
            else
            {
                Character character = Character.GetCharacterFromGameObject(obj);
                if (character != null)
                {
                    if (character.LeftArm != null && character.LeftArm.Forearm == obj)
                    {
                        return character.LeftArm;
                    }
                    else if (character.RightArm != null && character.RightArm.Forearm == obj)
                    {
                        return character.RightArm;
                    }
                }
                else
                {
                    BodyPart bodyPart = obj.GetComponent<BodyPart>();
                    if (bodyPart != null)
                    {
                        return bodyPart.arm;
                    }
                    else
                    {
                        Debug.LogError("Character object without BodyPart script : " + obj.name);
                    }
                }
            }
        }
        return null;
    }

    public void SetIsUnderTension(bool state)
    {
        isUnderTension = state;
    }

    public bool GetIsUnderTention()
    {
        return isUnderTension;
    }

    void OnJointBreak2D(Joint2D brokenJoint)
    {
        if (brokenJoint == joint)
        {
            BreakJoint();
        }
    }

    public void BreakJoint()
    {
        transform.parent = null;
        
        if (IsLeft)
        {
            parentCharacter.RemoveLeftArm();
        }
        else
        {
            parentCharacter.RemoveRightArm();
        }

        if (distanceJoint != null)
        {
            Destroy(distanceJoint);
        }
        Destroy(joint);
    }
    

    public void Break()
    {
        Destroy(joint);
        if (IsLeft)
        {
            if (parentCharacter.RightArm != null)
            {
                parentCharacter.RemoveLeftArm();
            }
            else
            {
                parentCharacter.Die();
            }
        }
        else
        {
            if (parentCharacter.LeftArm != null)
            {
                parentCharacter.RemoveRightArm();
            }
            else
            {
                parentCharacter.Die();
            }
        }
        parentCharacter = null;
    }
}
