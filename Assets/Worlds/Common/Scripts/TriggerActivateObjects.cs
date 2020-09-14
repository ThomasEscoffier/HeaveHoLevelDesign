using System.Collections.Generic;
using UnityEngine;

public class TriggerActivateObjects : MonoBehaviour {

    public bool onlyDetectPlayerBody = false;
    public bool CheckIfIsCamera = false;
    public List<GameObject> ObjectsToActivate = new List<GameObject>();
    bool isActived = false;

    void Start()
    {
        SetAllObjects(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActived && collision.CompareTag("Character"))
        {
            if (onlyDetectPlayerBody)
            {
                Character character = collision.GetComponent<Character>();
                if (character != null && (!CheckIfIsCamera || CheckIfIsCamera && LevelManager.IsObjectInsideCamera(character.gameObject)))
                {
                    SetAllObjects(true);
                }
            }
            else
            {
                if (!CheckIfIsCamera || CheckIfIsCamera && LevelManager.IsObjectInsideCamera(collision.gameObject))
                {
                    SetAllObjects(true);
                }
            }
        }
    }

    private void SetAllObjects(bool state)
    {
        foreach (GameObject obj in ObjectsToActivate)
        {
            obj.SetActive(state);
        }
        isActived = state;
    }
}
