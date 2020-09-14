using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCharactersTrigger : MonoBehaviour {

    List<Character> charactersInTrigger = new List<Character>();

    void Update()
    {
        charactersInTrigger.RemoveAll(i => i == null);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null && !charactersInTrigger.Contains(character))
            {
                charactersInTrigger.Add(character);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null)
            {
                charactersInTrigger.Remove(character);
            }
        }
    }

    public int GetNumberCharacters()
    {
        return charactersInTrigger.Count;
    }

    public List<Character> GetCharacters()
    {
        return charactersInTrigger;
    }
}
