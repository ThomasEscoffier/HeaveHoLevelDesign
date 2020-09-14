using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;

public class KeyboardMapperRow : MonoBehaviour, ISelectHandler
{
    public KeyboardMapper Mapper = null;
    public string[] RewiredActionNames;
    public Pole ActionContribution = Pole.Positive;
    public AxisRange AxisRange = AxisRange.Positive;

    Button bindButton = null;
    Text actionNameText = null;
    Text actionKeyNameText = null;
    Localization localization = null;

    void Awake()
    {
        bindButton = GetComponent<Button>();
        actionNameText = bindButton.GetComponentInChildren<Text>();
        actionKeyNameText = transform.GetChild(2).GetComponent<Text>();
        localization = actionKeyNameText.GetComponent<Localization>();
        localization.enabled = false;
    }
    
    void Start()
    {
        RefreshKeyName();
    }

    public void RefreshKeyName()
    {
        actionKeyNameText.text = Mapper.GetElementNameFromAction(this, RewiredActionNames[0]);
    }

    public void SetTextKeyName(string actionKeyName, bool isLocalized = true)
    {
        if (isLocalized)
        {
            localization.enabled = true;
            localization.SetText(actionKeyName);
        }
        else
        {
            localization.enabled = false;
            actionKeyNameText.text = actionKeyName;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (Mapper != null)
        {
            Mapper.SetCurrentRowIndex(this);
        }
    }

    public Localization GetLocalization()
    {
        return localization;
    }
}
