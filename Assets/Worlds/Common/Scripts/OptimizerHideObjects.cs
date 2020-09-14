using System.Collections.Generic;
using UnityEngine;

public class OptimizerHideObjects : MonoBehaviour 
{
    public float Offset = 3.0f;
    public GameObject Start, End;
    public int TimeSlice = 2;
    public bool UpdatLineRenderer = true;
    public float AngularDrag = 10f;
    public float LinearDrag = 10f;
    
    private GameObject childrenContainer;
    private bool childrenActive = true;
    private int currentFrame;
    private LineRenderer line;
    private Vector3[] positions;
    private bool isHeld = false;

    private Rigidbody2D StartRb = null;
    private Rigidbody2D EndRb = null;
    private List<Rigidbody2D> nodes = new List<Rigidbody2D>();

    private bool isPaused = false;

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
            StartRb = transform.GetChild(0).GetComponent<Rigidbody2D>();
            Debug.LogWarning("OptimizerDisableObjects missing a start gameobject");
        }
        else
        {
            StartRb = Start.GetComponent<Rigidbody2D>();
        }
        if (End == null)
        {
            End = transform.GetChild(transform.childCount - 1).gameObject;
            EndRb = transform.GetChild(transform.childCount - 1).GetComponent<Rigidbody2D>();
            Debug.LogWarning("OptimizerDisableObjects missing an end gameobject");
        }
        else
        {
            EndRb = End.GetComponent<Rigidbody2D>();
        }
        
        childrenContainer = new GameObject("parent");
        childrenContainer.transform.SetParent(transform, false);
        // Set as first sibling and stop loop at 1 not not consider childrenContainer.
        childrenContainer.transform.SetAsFirstSibling();
        for (int i = transform.childCount - 1; i >= 1; i--)
        {
            transform.GetChild(i).SetParent(childrenContainer.transform);
        }

        for (int i = 0; i < childrenContainer.transform.childCount; ++i)
        {
            Rigidbody2D rb = childrenContainer.transform.GetChild(i).GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                nodes.Add(rb);
            }
        }
        
        CheckParts();
        // Add a range to stagger instances.
        currentFrame = Random.Range(0, TimeSlice + 1);

        if (UpdatLineRenderer)
        {
            
            line = GetComponent<LineRenderer>();
            if (line != null)
            {
                positions = new Vector3[childrenContainer.transform.childCount];
                line.positionCount = positions.Length;
                var spriteRenderer =  End.GetComponent<SpriteRenderer>();
                line.sharedMaterial = new Material(line.sharedMaterial)
                {
                    color = spriteRenderer.color
                };
            }
        }
    }

    private void Update ()
    {
        if (!isPaused)
        {
            if (currentFrame >= TimeSlice)
            {
                CheckParts();
                if (childrenActive)
                {
                    if (!isHeld)
                    {
                        if (IsHeldByCharacter())
                        {
                            ChangeDrag(LinearDrag, AngularDrag);
                            isHeld = true;
                        }
                    }
                    else
                    {
                        if (!IsHeldByCharacter())
                        {
                            ChangeDrag(0f, 0f);
                            isHeld = false;
                        }
                    }
                }
                currentFrame = 0;
            }
            else
            {
                currentFrame++;
            }
        }

        if (childrenActive && line != null)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                var pos = childrenContainer.transform.GetChild(i).localPosition;
                pos.z = -1;
                positions[i] = pos;
            }
            line.SetPositions(positions);
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

    public Rigidbody2D GetStartRigidbody()
    {
        return StartRb;
    }

    public Rigidbody2D GetEndRigidbody()
    {
        return EndRb;
    }

    public void SetIsPaused(bool state)
    {
        isPaused = state;
    }

    private bool IsHeldByCharacter()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (nodes[i].bodyType == RigidbodyType2D.Static)
                continue;

            FixedJoint2D[] joints = nodes[i].GetComponents<FixedJoint2D>();
            for (int j = 0; j < joints.Length; ++j)
            {
                if (joints[j].connectedBody.CompareTag("Character"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ChangeDrag(float linearDrag, float angularDrag)
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            nodes[i].drag = linearDrag;
            nodes[i].angularDrag = angularDrag;
        }
    }
}
