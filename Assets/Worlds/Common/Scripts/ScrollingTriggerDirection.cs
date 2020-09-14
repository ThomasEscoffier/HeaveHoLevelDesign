using UnityEngine;

public class ScrollingTriggerDirection : MonoBehaviour {

    public ScrollingCameraTriggers.eDirection Direction;
    public int Order = 0;

    Collider2D triggerDirection = null;
    Camera cam = null;
    ScrollingCameraTriggers cameraTriggers = null;

    void Awake()
    {
        triggerDirection = GetComponent<Collider2D>();
        cam = Camera.main;
        cameraTriggers = FindObjectOfType<ScrollingCameraTriggers>();
    }
    
    void Update()
    {
        if (triggerDirection.bounds.Contains(new Vector3(cam.transform.position.x, cam.transform.position.y, 0f)))
        {
            if (cameraTriggers != null)
            {
                cameraTriggers.ActivateTrigger(this);
            }
        }
    }
}