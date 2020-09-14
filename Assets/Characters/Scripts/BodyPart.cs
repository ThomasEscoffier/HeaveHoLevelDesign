using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour {

    public Arm arm = null;

    Character parentCharacter = null;
    Rigidbody2D rb = null;
    List<Collider2D> grabColliders = new List<Collider2D>();
    List<Collider2D> characterColliders = new List<Collider2D>();

    public Arm GetArm()
    {
        return arm;
    }

    public Character GetParentCharacter()
    {
        return parentCharacter;
    }

    public bool GetIsTouchingGrabSurface()
    {
        grabColliders.RemoveAll(i => i == null);
        return grabColliders.Count > 0;
    }

    public bool GetIsTouchingOtherCharacter()
    {
        characterColliders.RemoveAll(i => i == null);
        return characterColliders.Count > 0;
    }

    public List<Character> GetTouchingCharacters()
    {
        characterColliders.RemoveAll(i => i == null);

        List<Character> characters = new List<Character>();
        foreach (Collider2D col in characterColliders)
        {
            Character character = Character.GetCharacterFromGameObject(col.gameObject);
            if (!characters.Contains(character))
            {
                characters.Add(character);
            }
        }

        return characters;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        parentCharacter = Character.GetCharacterFromGameObject(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Grab") && !grabColliders.Contains(col.collider))
        {
            grabColliders.Add(col.collider);
            Character.BumpInfo bumpInfo = parentCharacter.GetBumpIntensity(col.relativeVelocity.magnitude);
            if (bumpInfo.VelocityToLaunch > 0)
            {
                Platform platform = col.gameObject.GetComponent<Platform>();
                parentCharacter.BumpPlatform(col.GetContact(0), platform != null ? platform.Type : Platform.ePlatformType.NONE, bumpInfo);
            }
        }
        else if (col.collider.CompareTag("Character") && !characterColliders.Contains(col.collider) && !parentCharacter.IsGameObjectFromCharacter(col.gameObject))
        {
            characterColliders.Add(col.collider);
            Character.BumpInfo bumpInfo = parentCharacter.GetBumpIntensity(col.relativeVelocity.magnitude);
            if (bumpInfo.VelocityToLaunch > 0)
            {
                Platform platform = col.gameObject.GetComponent<Platform>();
                parentCharacter.BumpCharacter(col.GetContact(0), platform != null ? platform.Type : Platform.ePlatformType.NONE, bumpInfo);
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Grab"))
        {
            grabColliders.Remove(col.collider);
        }
        else if (col.collider.CompareTag("Character"))
        {
            characterColliders.Remove(col.collider);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Grab") && !grabColliders.Contains(col))
        {
            grabColliders.Add(col);
        }
        else if (col.CompareTag("Character") && !characterColliders.Contains(col) && !parentCharacter.IsGameObjectFromCharacter(col.gameObject))
        {
            characterColliders.Add(col);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Grab"))
        {
            grabColliders.Remove(col);
        }
        else if (col.CompareTag("Character"))
        {
            characterColliders.Remove(col);
        }
    }
    
    void OnJointBreak2D(Joint2D brokenJoint)
    {
        Destroy(GetComponent<DistanceJoint2D>());
        arm.BreakJoint();
    }
}
