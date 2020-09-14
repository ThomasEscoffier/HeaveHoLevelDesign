using System;
using UnityEngine;

[CreateAssetMenu(fileName ="fontassets", menuName = "Fonts/Create FontAssets")]
public class FontAssets : ScriptableObject 
{
    [Serializable]
    public class FontDefinition
    {
        [SerializeField] 
        protected Font big,
            small,
            digital;

        [SerializeField]
        protected float bigSize,
            smallSize,
            digitalSize;

        public Font Small
        {
            get { return small; }
        }

        public Font Big
        {
            get { return big; }
        }

        public Font Digital
        {
            get { return digital; }
        }

        public float BigRatio
        {
            get { return bigSize; }
        }

        public float SmallRatio
        {
            get { return smallSize; }
        }

        public float DigitalRatio
        {
            get { return digitalSize; }
        }
    }

    [SerializeField] 
    protected FontDefinition romanAlphabet, russian, japanese, korean, traditionalChinese, simplifiedChinese;

    [SerializeField] 
    protected TextAsset fontsInfos;

    public FontDefinition TraditionalChinese
    {
        get { return traditionalChinese; }
    }

    public FontDefinition SimplifiedChinese
    {
        get { return simplifiedChinese; }
    }

    public FontDefinition Roman
    {
        get { return romanAlphabet; }
    }

    public FontDefinition Russian
    {
        get { return russian; }
    }

    public FontDefinition Japanese
    {
        get { return japanese; }
    }

    public FontDefinition Korean
    {
        get { return korean; }
    }

    public TextAsset FontsInfos
    {
        get { return fontsInfos; }
    }
}
