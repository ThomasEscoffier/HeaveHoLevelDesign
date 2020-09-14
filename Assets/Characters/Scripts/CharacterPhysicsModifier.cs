using System.Linq;
using UnityEngine;

public class CharacterPhysicsModifier : MonoBehaviour
{
    public WorldProperties[] worldsToApplyIn;

    public float scale = 0.2f;
    public Rigidbody2D[] rigidbodies;
    
    
    void Start()
    {
        foreach (var rb in rigidbodies)
        {
            rb.mass *= scale;
            rb.drag *= scale;
            rb.angularDrag *= scale;
        }
            
        var character = GetComponent<Character>();
        if (character == null)
        {
            Debug.LogError("Character not found");
            return;
        }
        character.ForceMovement *= scale;
        character.ForcePull *= scale;
        character.BumpSmall.ForcePushBack *= scale;
        character.BumpMedium.ForcePushBack *= scale;
        character.BumpHard.ForcePushBack *= scale;
        character.BumpHudge.ForcePushBack *= scale;
        
        character.ReleaseImpulseForceOnItselfMax *= scale;
        character.ReleaseImpulseForceOnOthersMax *= scale;        
        character.ReleaseImpulseForceOnItselfMin *= scale;
        character.ReleaseImpulseForceOnOthersMin *= scale;

    }
}
