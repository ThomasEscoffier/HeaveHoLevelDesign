using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
using System.Globalization;

public class LocalizationManager {

    public enum eLanguage
    {
        EN = 2,
        FR,
        DE,
        ES,
        PTBR,
        IT,
        RU,
        JP,
        KO,
        TCH,
        SCH,
        NOTSET
    }

    public enum eFontType
    {
        BIG, // Neat chalk
        NORMAL, // Crayon hand
        DIGITAL, // Digital dot
    }

    eLanguage currentLanguage = eLanguage.EN;
    Dictionary<string, string> texts = new Dictionary<string, string>();
    Font fontsBig = null;
    Font fontsDigital = null;
    Font fontsNormal = null;

    float ratioBigFont = 0;
    float ratioDigitalFont = 0;
    float ratioNormalFont = 0;

    public UnityEvent OnLanguageChangedEvent = new UnityEvent();

    const string englishLangLocaKey = "Language_English";

    FontAssets fontAssets;

    public void Init()
    {
        fontAssets = Resources.Load<FontAssets>("Fonts/fontassets");
        currentLanguage = eLanguage.EN;
        SetLanguage(currentLanguage);
    }

    public bool IsLoaded()
    {
        return fontAssets != null;
    }

    public bool SetLanguage(eLanguage language, bool saveImmediate = false)
    {
        if (fontAssets == null)
        {
            return false;
        }
        texts.Clear();
        currentLanguage = language;
        TextAsset textData = Resources.Load<TextAsset>("Localization/InGameText");

        string[] data = textData.text.Split(new char[] { '\n' });

        int index = (int)currentLanguage;
        for (int i = 4; i < data.Length; ++i)
        {
            string[] row = data[i].Split(new char[] { ',' });

            if (!row[index].Equals(""))
            {
                texts.Add(row[0], !row[index].Contains("</") ? row[index].Replace('/', ',') : row[index]); // Don't replace if markups
            }
        }

        LoadFontRatio(currentLanguage);

        fontsBig = GetFont(language, eFontType.BIG);
        fontsDigital = GetFont(language, eFontType.DIGITAL);
        fontsNormal = GetFont(language, eFontType.NORMAL);

        if (OnLanguageChangedEvent != null)
        {
            OnLanguageChangedEvent.Invoke();
        }
        return true;
    }

    Font GetFont(eLanguage language, eFontType type)
    {
        switch (language)
        {
            case eLanguage.EN:
            case eLanguage.FR:
            case eLanguage.DE:
            case eLanguage.ES:
            case eLanguage.PTBR:
            case eLanguage.IT:
            default:
                switch (type)
                {
                    case eFontType.BIG:
                        return fontAssets.Roman.Big;
                    case eFontType.DIGITAL:
                        return fontAssets.Roman.Digital;
                    case eFontType.NORMAL:
                    default:
                        return fontAssets.Roman.Small;
                }
            case eLanguage.RU:
                switch (type)
                {
                    case eFontType.BIG:
                        return fontAssets.Russian.Big;
                    case eFontType.DIGITAL:
                        return fontAssets.Russian.Digital;
                    case eFontType.NORMAL:
                    default:
                        return fontAssets.Russian.Small;
                }
            case eLanguage.JP:
                switch (type)
                {
                    case eFontType.BIG:
                        return fontAssets.Japanese.Big;
                    case eFontType.DIGITAL:
                        return fontAssets.Japanese.Digital;
                    case eFontType.NORMAL:
                    default:
                        return fontAssets.Japanese.Small;
                }
            case eLanguage.KO:
                switch (type)
                {
                    case eFontType.BIG:
                        return fontAssets.Korean.Big;
                    case eFontType.DIGITAL:
                        return fontAssets.Korean.Digital;
                    case eFontType.NORMAL:
                    default:
                        return fontAssets.Korean.Small;
                }
            case eLanguage.TCH:
                switch (type)
                {
                    case eFontType.BIG:
                        return fontAssets.TraditionalChinese.Big;
                    case eFontType.DIGITAL:
                        return fontAssets.TraditionalChinese.Digital;
                    case eFontType.NORMAL:
                    default:
                        return fontAssets.TraditionalChinese.Small;
                }
            case eLanguage.SCH:
                switch (type)
                {
                    case eFontType.BIG:
                        return fontAssets.SimplifiedChinese.Big;
                    case eFontType.DIGITAL:
                        return fontAssets.SimplifiedChinese.Digital;
                    case eFontType.NORMAL:
                    default:
                        return fontAssets.SimplifiedChinese.Small;
                }
        }
    }

    void LoadFontRatio(eLanguage language)
    {
        switch (language)
        {
            case eLanguage.EN:
            case eLanguage.FR:
            case eLanguage.DE:
            case eLanguage.ES:
            case eLanguage.PTBR:
            case eLanguage.IT:
            default:
                ratioBigFont = fontAssets.Roman.BigRatio;
                ratioNormalFont = fontAssets.Roman.SmallRatio;
                ratioDigitalFont = fontAssets.Roman.DigitalRatio;
                break;
            case eLanguage.RU:
                ratioBigFont = fontAssets.Russian.BigRatio;
                ratioNormalFont = fontAssets.Russian.SmallRatio;
                ratioDigitalFont = fontAssets.Russian.DigitalRatio;
                break;
            case eLanguage.JP:
                ratioBigFont = fontAssets.Japanese.BigRatio;
                ratioNormalFont = fontAssets.Japanese.SmallRatio;
                ratioDigitalFont = fontAssets.Japanese.DigitalRatio;
                break;
            case eLanguage.KO:
                ratioBigFont = fontAssets.Korean.BigRatio;
                ratioNormalFont = fontAssets.Korean.SmallRatio;
                ratioDigitalFont = fontAssets.Korean.DigitalRatio;
                break;
            case eLanguage.TCH:
                ratioBigFont = fontAssets.TraditionalChinese.BigRatio;
                ratioNormalFont = fontAssets.TraditionalChinese.SmallRatio;
                ratioDigitalFont = fontAssets.TraditionalChinese.DigitalRatio;
                break;
            case eLanguage.SCH:
                ratioBigFont = fontAssets.SimplifiedChinese.BigRatio;
                ratioNormalFont = fontAssets.SimplifiedChinese.SmallRatio;
                ratioDigitalFont = fontAssets.SimplifiedChinese.DigitalRatio;
                break;
        }
    }

    public string GetTextFromLoca(string key)
    {
        if (texts.ContainsKey(key))
            return texts[key];
        return "";
    }

    public eLanguage GetCurrentLanguage()
    {
        return currentLanguage;
    }

    public Font GetCurrentLanguageFont(eFontType fontType)
    {
        switch(fontType)
        {
            case eFontType.BIG:
                return fontsBig;
            case eFontType.DIGITAL:
                return fontsDigital;
            case eFontType.NORMAL:
            default:
                return fontsNormal;
        }
    }

    public float GetFontRatio(eFontType fontType)
    {
        switch (fontType)
        {
            case eFontType.BIG:
                return ratioBigFont;
            case eFontType.DIGITAL:
                return ratioDigitalFont;
            case eFontType.NORMAL:
            default:
                return ratioNormalFont;
        }
    }

    public static string RemoveDiacritics(string text)
    {
        if (text.Equals(""))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
