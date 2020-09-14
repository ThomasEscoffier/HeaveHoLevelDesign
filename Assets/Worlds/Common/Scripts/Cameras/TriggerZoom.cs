using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoom : MonoBehaviour {

    public bool IsUp = false;
    public float CameraSize = 10f;
    public Vector3 CameraPosition = Vector2.zero;
    public float TimeBeforeActivating = 1f;

    float timerActivating = 0f;
    bool isWaitingToDeactivate = false;
    bool isZoomActive = false;
    CameraZoom cameraZoom;
    List<Character> charactersInTrigger = new List<Character>();

    void Awake()
    {
        cameraZoom = FindObjectOfType<CameraZoom>();
    }

    void Update()
    {
        if (isWaitingToDeactivate)
        {
            timerActivating = Mathf.Min(timerActivating + Time.deltaTime, TimeBeforeActivating);
            if (timerActivating >= TimeBeforeActivating)
            {
                isZoomActive = false;
                isWaitingToDeactivate = false;
                cameraZoom.StartZoomIn(IsUp);
                timerActivating = 0f;
            }
        }

        if (!isZoomActive && charactersInTrigger.Count > 0 && IsAtLeastOneCharacterGrabbing())
        {
            cameraZoom.StartZoomOut(IsUp);
            isWaitingToDeactivate = false;
            isZoomActive = true;
        }
        else if (isZoomActive && !isWaitingToDeactivate && charactersInTrigger.Count <= 0 && !IsAtLeastOneCharacterGrabbing())
        {
            isWaitingToDeactivate = true;
            timerActivating = 0f;
        }
    }

    public bool GetIsZoomActive()
    {
        return isZoomActive;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            Character character = Character.GetCharacterFromGameObject(collision.gameObject);
            if (character != null)
            {
                charactersInTrigger.Add(character);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            Character character = Character.GetCharacterFromGameObject(collision.gameObject);
            if (character != null)
            {
                charactersInTrigger.Remove(character);
            }
        }
    }

    public bool IsAtLeastOneCharacterGrabbing()
    {
        foreach (Character character in charactersInTrigger)
        {
            if (character.LeftArm != null && GameManager.Instance.GetChainsManager().IsEnchoredVisible(character.LeftArm)
                || character.RightArm != null && GameManager.Instance.GetChainsManager().IsEnchoredVisible(character.RightArm))
            {
                return true;
            }
        }
        return false;
    }
}
