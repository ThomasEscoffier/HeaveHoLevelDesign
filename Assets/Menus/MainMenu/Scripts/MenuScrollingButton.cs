using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScrollingButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject BackgroundPanel;

    MainButtonsPanel panel;
    protected Button button = null;
    protected SoundModule soundModule = null;

    public bool IsStillSelected = false;

    public virtual void Awake()
    {
        button = GetComponent<Button>();
        soundModule = GetComponent<SoundModule>();
        panel = FindObjectOfType<MainButtonsPanel>();
    }

    public virtual void OnDeselect(BaseEventData data)
    {
        if (!IsStillSelected)
        {
            BackgroundPanel.gameObject.SetActive(false);
        }
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        soundModule.PlayOneShot("ArrowVertical");
        DisplayPanel();
    }

    public void DisplayPanel()
    {
        BackgroundPanel.gameObject.SetActive(true);
        panel.ReOrderButtons();
    }

    public Button GetButton()
    {
        return button;
    }
}
