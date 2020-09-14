using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlsButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public PlayerMenuSelection Selection;
    public PlayerControls.eLayout Layout;

    public float AlphaNormal = 0.5f;
    public float AlphaHighlighted = 1f;

    Image img = null;
    SoundModule sound = null;

    void Awake()
    {
        img = GetComponent<Image>();
        sound = GetComponent<SoundModule>();
        ChangeOpacity(AlphaNormal);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ChangeOpacity(AlphaHighlighted);
        sound.PlayOneShot("Arrow");
    }

    public void OnDeselect(BaseEventData data)
    {
        ChangeOpacity(AlphaNormal);
    }

    void ChangeOpacity(float alpha)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }

	/*public void SelectCurrentControlsType()
    {
        Selection.ConfirmControls(Layout);
    }*/
}
