using UnityEngine;

[CreateAssetMenu(fileName = "characterassets", menuName = "Character/Create CharacterAssets")]
public class CharacterAssets : ScriptableObject
{
    [SerializeField]
    protected Sprite[] hairSprites;
    [SerializeField]
    protected Sprite[] glassesSprites;
    [SerializeField]
    protected Sprite[] facialFeaturesSprites;
    [SerializeField]
    protected Sprite[] armSprites;
    [SerializeField]
    protected Sprite[] forearmSprites;
    [SerializeField]
    protected Sprite[] handSprites;
    [SerializeField]
    protected Sprite[] handClosedSprites;
    [SerializeField]
    protected Sprite[] handPointSprites;
    [SerializeField]
    protected Sprite[] backSprites;
    [SerializeField]
    protected Sprite[] standardSprites;
    [SerializeField]
    protected RuntimeAnimatorController[] personalityAnims;
    [SerializeField]
    protected RuntimeAnimatorController[] personalityUIAnims;

    public Sprite[] HairSprites
    {
        get { return hairSprites; }
#if UNITY_EDITOR
			set { hairSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] GlassesSprites
    {
        get { return glassesSprites; }
#if UNITY_EDITOR
			set { glassesSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] FacialFeaturesSprites
    {
        get { return facialFeaturesSprites; }
#if UNITY_EDITOR
			set { facialFeaturesSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] ArmSprites
    {
        get { return armSprites; }
#if UNITY_EDITOR
			set { armSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] ForearmSprites
    {
        get { return forearmSprites; }
#if UNITY_EDITOR
			set { forearmSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] HandSprites
    {
        get { return handSprites; }
#if UNITY_EDITOR
			set { handSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] HandClosedSprites
    {
        get { return handClosedSprites; }
#if UNITY_EDITOR
			set { handClosedSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] HandPointSprites
    {
        get { return handPointSprites; }
#if UNITY_EDITOR
			set { handPointSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] BackSprites
    {
        get { return backSprites; }
#if UNITY_EDITOR
			set { backSprites = value; } // Set by CharacterAssetsEditor
#endif
    }

    public Sprite[] StandardSprites
    {
        get { return standardSprites; }
    }

    public RuntimeAnimatorController[] PersonalityAnims
    {
        get { return personalityAnims; }
#if UNITY_EDITOR
			set { personalityAnims = value; } // Set by CharacterAssetsEditor
#endif
    }

    public RuntimeAnimatorController[] PersonalityUIAnims
    {
        get { return personalityUIAnims; }
#if UNITY_EDITOR
			set { personalityUIAnims = value; } // Set by CharacterAssetsEditor
#endif
    }
}
