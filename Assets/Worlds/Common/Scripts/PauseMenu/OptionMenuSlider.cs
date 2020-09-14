using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionMenuSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Color ColorWhenSelected;
    public Color ColorWhenNotSelected;

    public Image SliderImage = null;

    void Awake()
    {
        SliderImage.color = ColorWhenNotSelected;
    }

    public void OnSelect(BaseEventData eventData)
    {
        SliderImage.color = ColorWhenSelected;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SliderImage.color = ColorWhenNotSelected;
    }
}
