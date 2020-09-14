using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour {

    public string LocalizationKey = "";
    public string SwitchOverrideKey;
    public LocalizationManager.eFontType FontType = LocalizationManager.eFontType.BIG;
    Text displayedText = null;
    int size = 0;

	void Start()
    {
        displayedText = GetComponent<Text>();
        if (displayedText.resizeTextForBestFit)
        {
            size = displayedText.resizeTextMaxSize;
        }
        else
        {
            size = displayedText.fontSize;
        }
        if (GameManager.Instance.GetLocalizationManager().IsLoaded())
        {
            OnLanguageChanged();
        }
        GameManager.Instance.GetLocalizationManager().OnLanguageChangedEvent.AddListener(OnLanguageChanged);
	}

    public void SetText(string key)
    {
        LocalizationKey = key;
        if (displayedText == null || GameManager.Instance.GetLocalizationManager() == null || string.IsNullOrEmpty(LocalizationKey))
            return;

        displayedText.text = GameManager.Instance.GetLocalizationManager().GetTextFromLoca(key);
        if (FontType == LocalizationManager.eFontType.DIGITAL)
        {
            displayedText.text = displayedText.text.ToUpper();
        }
    }

    void SetFont()
    {
        displayedText.font = GameManager.Instance.GetLocalizationManager().GetCurrentLanguageFont(FontType);
    }

    void SetSize()
    {
        float newSize = (float)size * GameManager.Instance.GetLocalizationManager().GetFontRatio(FontType);
        if (displayedText.resizeTextForBestFit)
        {
            displayedText.resizeTextMaxSize = (int)newSize;
        }
        else
        {
            displayedText.fontSize = (int)newSize;
        }
    }

    public void OnLanguageChanged()
    {
        SetFont();
        SetSize();
        SetText(LocalizationKey);
    }
}
