using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    public TriggerZoom TriggerUp;
    public TriggerZoom TriggerDown;
    Victory victoryTrigger;

    public Camera LevelCamera;
    public float TimeTransition = 2f;

    bool isZooming = false;

    float timerZoom = 0f;
    float oldSize = 0f;
    Vector3 oldPos = Vector3.zero;
    float newSize = 0f;
    Vector3 newPos = Vector3.zero;

    float minSize = 0f;
    Vector3 minPos = Vector3.zero;

    void Awake()
    {
        minSize = LevelCamera.orthographicSize;
        minPos = LevelCamera.transform.position;
        victoryTrigger = FindObjectOfType<Victory>();
    }

    void Update()
    {
        if (victoryTrigger != null && victoryTrigger.IsValidated())
            return;

        if (isZooming)
        {
            if (timerZoom >= TimeTransition)
            {
                isZooming = false;
            }

            LevelCamera.orthographicSize = Mathf.Lerp(oldSize, newSize, timerZoom / TimeTransition);
            LevelCamera.transform.position = Vector3.Lerp(oldPos, newPos, timerZoom / TimeTransition);
            timerZoom = Mathf.Min(timerZoom + Time.deltaTime, TimeTransition);
        }
    }

    public void StartZoomOut(bool isUp)
    {
        oldSize = LevelCamera.orthographicSize;
        oldPos = LevelCamera.transform.position;
        timerZoom = 0f;
        isZooming = true;

        if (isUp)
        {
            if (TriggerDown.GetIsZoomActive())
            {
                newSize = minSize + (TriggerDown.CameraSize - minSize) + (TriggerUp.CameraSize - minSize);
                newPos = TriggerDown.CameraPosition + (TriggerUp.CameraPosition - TriggerDown.CameraPosition) / 2;
            }
            else
            {
                newSize = TriggerUp.CameraSize;
                newPos = TriggerUp.CameraPosition;
            }
        }
        else
        {
            if (TriggerUp.GetIsZoomActive())
            {
                newSize = minSize + (TriggerDown.CameraSize - minSize) + (TriggerUp.CameraSize - minSize);
                newPos = TriggerDown.CameraPosition + (TriggerUp.CameraPosition - TriggerDown.CameraPosition) / 2;
            }
            else
            {
                newSize = TriggerDown.CameraSize;
                newPos = TriggerDown.CameraPosition;
            }
        }
    }

    public void StartZoomIn(bool isUp)
    {
        oldSize = LevelCamera.orthographicSize;
        oldPos = LevelCamera.transform.position;
        timerZoom = 0f;
        isZooming = true;

        if (isUp)
        {
            if (TriggerDown.GetIsZoomActive())
            {
                newSize = TriggerDown.CameraSize;
                newPos = TriggerDown.CameraPosition;
            }
            else
            {
                newSize = minSize;
                newPos = minPos;
            }
        }
        else
        {
            if (TriggerUp.GetIsZoomActive())
            {
                newSize = TriggerUp.CameraSize;
                newPos = TriggerUp.CameraPosition;
            }
            else
            {
                newSize = minSize;
                newPos = minPos;
            }
        }
    }
}
