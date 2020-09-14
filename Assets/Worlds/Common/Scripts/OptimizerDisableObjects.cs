using UnityEngine;

public class OptimizerDisableObjects : MonoBehaviour {

    public float Offset = 3.0f;
    public GameObject Start, End;
    public int TimeSlice = 2;
    
    private GameObject childrenContainer;
    private bool childrenActive = true;
    private int currentFrame;

    private void Awake()
    {
        if (transform.childCount < 1)
        {
            Debug.LogError("OptimizerDisableObjects does not have enough children");
            enabled = false;
            return;
        }
        if (Start == null)
        {
            Start = transform.GetChild(0).gameObject;
            Debug.LogWarning("OptimizerDisableObjects missing a start gameobject");
        }
        if (End == null)
        {
            End = transform.GetChild(transform.childCount - 1).gameObject;
            Debug.LogWarning("OptimizerDisableObjects missing an end gameobject");
        }
        
        childrenContainer = new GameObject("parent");
        childrenContainer.transform.SetParent(transform, false);
        // Set as first sibling and stop loop at 1 not not consider childrenContainer.
        childrenContainer.transform.SetAsFirstSibling();
        for (int i = transform.childCount - 1; i >= 1; i--)
        {
            transform.GetChild(i).SetParent(childrenContainer.transform);
        }
        
        CheckParts();
        // Add a range to stagger instances.
        currentFrame = Random.Range(0, TimeSlice + 1);
    }

    private void Update ()
    {
        if (currentFrame >= TimeSlice)
        {
            CheckParts();
            currentFrame = 0;
        }
        else
        {
            currentFrame++;
        }
        
    }

    private void Toggle(bool setActive)
    { 
        if (childrenActive == setActive)
        {
            return;
        }
        childrenContainer.SetActive(setActive);
        childrenActive = setActive;
    }
    
    private void CheckParts()
    {
        Toggle(LevelManager.IsObjectInsideCamera(Start, Offset) || 
               LevelManager.IsObjectInsideCamera(End, Offset));
    }
}
