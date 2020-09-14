using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CharacterPresets/CharacterPreset")]
public class CharacterPreset : ScriptableObject
{
    public string Name;

    public ColorPreset BodyColor;
    public string HairName;
    public string GlassesName;
    public string FacialFeaturesName;
    public ArmsPreset Arms;
    public Character.ePersonality Personality;
    public bool IsRobot;
    public bool IsMask;
    public bool IsHideFace;

    public bool IsAvailableInSelection;

    public void Init(string Name_, ColorPreset ColorPreset_, string HairName_, string GlassesName_, string FacialFeaturesName_, ArmsPreset Arms_, Character.ePersonality Personality_, bool IsRobot_, bool IsMask_, bool IsHideFace_, bool IsAvailableInSelection_)
    {
        Name = Name_;
        BodyColor = ColorPreset_;
        HairName = HairName_;
        GlassesName = GlassesName_;
        FacialFeaturesName = FacialFeaturesName_;
        Arms = Arms_;
        Personality = Personality_;
        IsRobot = IsRobot_;
        IsMask = IsMask_;
        IsHideFace = IsHideFace_;
        IsAvailableInSelection = IsAvailableInSelection_;
    }

    public bool IsSame(CharacterPreset otherCharacter)
    {
        return HairName == otherCharacter.HairName && GlassesName == otherCharacter.GlassesName && FacialFeaturesName == otherCharacter.FacialFeaturesName
            && Arms.IsSame(otherCharacter.Arms) && Personality == otherCharacter.Personality;
    }

    public bool IsSame(string otherHairName, string otherGlassesName, string otherFacialFeaturesName, ArmsPreset otherArms, Character.ePersonality otherPersonality)
    {
        return HairName == otherHairName && GlassesName == otherGlassesName && FacialFeaturesName == otherFacialFeaturesName
            && Arms.IsSame(otherArms) && Personality == otherPersonality;
    }
}
