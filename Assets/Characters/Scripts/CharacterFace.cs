using UnityEngine;

public class CharacterFace : MonoBehaviour
{
    CharacterSoundModule soundModule = null;
    //Character parentCharacter = null;

    void Awake()
    {
        //parentCharacter = transform.parent.parent.GetComponent<Character>();
        //soundModule = transform.parent.parent.GetComponent<CharacterSoundModule>();
    }

    public void PlayChargeSound()
    {
        /*if (!soundModule.IsChargePlaying())
        {
            soundModule.PlayCharge();
        }*/
    }
}
