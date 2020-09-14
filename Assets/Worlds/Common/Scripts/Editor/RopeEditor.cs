using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OptimizerHideObjects))]
public class RopeEditor : Editor
{
    private OptimizerHideObjects rope;
    
    private int numJoints = 10;
    private float colliderScale = 1.1f;
    private GameObject prefab;
    private bool useDistanceJoints;

    private void OnEnable()
    {
        rope = (OptimizerHideObjects) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("EDITOR");
        numJoints = EditorGUILayout.IntField("Number of joints", numJoints);
        colliderScale = EditorGUILayout.FloatField("Joint collider scale", colliderScale);
        prefab = EditorGUILayout.ObjectField( "Prefab", prefab, typeof(GameObject)) as GameObject;
        useDistanceJoints = EditorGUILayout.Toggle("Use Distance Joints", useDistanceJoints);

        if (GUILayout.Button("Generate rope nodes"))
        {
            for (int i = rope.transform.childCount - 1; i >= 0; i--)
            {
                var child = rope.transform.GetChild(i);
                if (child.gameObject != rope.End && child.gameObject != rope.Start)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            var startPos = rope.Start.transform.localPosition;
            var endPos = rope.End.transform.localPosition;
            var distance = Vector3.Distance(startPos, endPos);
            var inc = distance / (numJoints + 1);
            var direction = (endPos - startPos).normalized;
            
            for (int i = 1; i < numJoints + 1; i++)
            {
                // calculate position.
                var gO = Instantiate(prefab, rope.transform);
                gO.name = $"Node{i}";
                gO.transform.localPosition = startPos + direction * (inc * i);
                var collider = gO.GetComponent<CapsuleCollider2D>();
                var size = collider.size;
                size.y = inc * colliderScale;
                collider.size = size;

                // update joints (hinge maybe distance).
            }
            
            rope.End.transform.SetAsLastSibling();

            for (int i = 0; i < rope.transform.childCount - 1; i++)
            {
                var child = rope.transform.GetChild(i);
                var hinge = child.GetComponent<HingeJoint2D>();
                hinge.connectedBody = rope.transform.GetChild(i + 1).GetComponent<Rigidbody2D>();
                var distanceJ = child.GetComponent<DistanceJoint2D>();
                if (useDistanceJoints)
                {
                    distanceJ = distanceJ ? distanceJ : child.gameObject.AddComponent<DistanceJoint2D>();
                    if (distanceJ != null)
                    {
                       distanceJ.connectedBody = rope.transform.GetChild(i + 1).GetComponent<Rigidbody2D>();
                    }
                }
                else
                {
                    if (distanceJ != null)
                    {
                        DestroyImmediate(distanceJ);
                    }
                }
                
            }
            
            var endHinge = rope.End.GetComponent<HingeJoint2D>();
            endHinge.connectedBody = rope.transform.GetChild(rope.transform.childCount - 2).GetComponent<Rigidbody2D>();
        }
    }
}
