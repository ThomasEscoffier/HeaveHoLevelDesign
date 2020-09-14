using UnityEngine;

public class HUD : MonoBehaviour
{
    Canvas canvas = null;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = "Foreground";
        canvas.sortingOrder = 998;

        if (transform.childCount > 1)
        {
            transform.GetChild(1).SetAsFirstSibling();
        }
    }
}
