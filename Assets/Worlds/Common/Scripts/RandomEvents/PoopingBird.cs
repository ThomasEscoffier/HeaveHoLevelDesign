using UnityEngine;

public class PoopingBird : MonoBehaviour
{
    SoundModule sound = null;

    void Awake()
    {
        sound = GetComponent<SoundModule>();
    }

    public void PlayFlapSound()
    {
        sound.PlayOneShot("Bird", transform.position);
    }
}