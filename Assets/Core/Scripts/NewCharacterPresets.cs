using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CharacterPresets/Presets")]
public class NewCharacterPresets : ScriptableObject
{
    public List<ColorPreset> ColorsPresets = new List<ColorPreset>();
    public List<CharacterPreset> CharactersPresets = new List<CharacterPreset>();

    public List<string> HeadsHideFace = new List<string>();
    public List<string> RobotHeads = new List<string>();
    public List<string> MaskHeads = new List<string>(); 

    public List<string> HairDefault = new List<string>();
    public List<string> GlassesDefault = new List<string>();
    public List<string> FacialFeaturesDefault = new List<string>();

    public ColorPreset GetColorAlreadyExists(ColorPreset colorPreset)
    {
        foreach (ColorPreset color in ColorsPresets)
        {
            if (color.IsSame(colorPreset))
            {
                return color;
            }
        }
        return null;
    }

    public ColorPreset GetColorAlreadyExists(Color color)
    {
        foreach (ColorPreset colorPreset in ColorsPresets)
        {
            if (colorPreset.IsSame(color))
            {
                return colorPreset;
            }
        }
        return null;
    }

    public bool DoesColorAlreadyExists(ColorPreset colorPreset)
    {
        foreach (ColorPreset color in ColorsPresets)
        {
            if (color.IsSame(colorPreset))
            {
                return true;
            }
        }
        return false;
    }

    public CharacterPreset GetCharacterAlreadyExists(CharacterPreset characterPreset)
    {
        foreach (CharacterPreset character in CharactersPresets)
        {
            if (character.IsSame(characterPreset))
            {
                return character;
            }
        }
        return null;
    }

    public CharacterPreset GetCharacterAlreadyExists(string HairName_, string GlassesName_, string FacialFeaturesName_, ArmsPreset Arms_, Character.ePersonality Personality_)
    {
        foreach (CharacterPreset character in CharactersPresets)
        {
            if (character.IsSame(HairName_, GlassesName_, FacialFeaturesName_, Arms_, Personality_))
            {
                return character;
            }
        }
        return null;
    }

    public bool DoesCharacterAlreadyExists(CharacterPreset characterPreset)
    {
        foreach (CharacterPreset character in CharactersPresets)
        {
            if (character.IsSame(characterPreset))
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveColor(ColorPreset preset)
    {
        ColorsPresets.Remove(preset);
    }

    public void RemoveCharacter(CharacterPreset preset)
    {
        CharactersPresets.Remove(preset);
    }

    public void AddColor(ColorPreset preset)
    {
        ColorsPresets.Add(preset);
    }

    public void AddCharacter(CharacterPreset preset)
    {
        CharactersPresets.Add(preset);
    }

    public List<ColorPreset> GetColors()
    {
        return ColorsPresets;
    }

    public List<CharacterPreset> GetCharacters()
    {
        return CharactersPresets;
    }

    public ColorPreset GetColorPreset(string colorName)
    {
        foreach (ColorPreset color in ColorsPresets)
        {
            if (color.Name == colorName)
            {
                return color;
            }
        }

        return null;
    }

    public ArmsPreset GetArmPreset(string armName, bool onlyInDefault = false) // Search in characters and default
    {
        if (!onlyInDefault)
        {
            foreach (CharacterPreset character in CharactersPresets)
            {
                if (character.Arms.Name == armName)
                {
                    return character.Arms;
                }
            }
        }

        return null;
    }

    public CharacterPreset GetCharacterPreset(string characterName)
    {
        foreach (CharacterPreset character in CharactersPresets)
        {
            if (character.Name == characterName)
            {
                return character;
            }
        }

        return null;
    }

    public bool DoesColorNameAlreadyExists(string colorName)
    {
        foreach (ColorPreset color in ColorsPresets)
        {
            if (color.Name == colorName)
            {
                return true;
            }
        }
        return false;
    }

    public bool DoesCharacterNameAlreadyExists(string characterName)
    {
        foreach (CharacterPreset character in CharactersPresets)
        {
            if (character.Name == characterName)
            {
                return true;
            }
        }
        return false;
    }

    public void AddHeadToRobotList(string headName)
    {
        if (!RobotHeads.Contains(headName))
        {
            RobotHeads.Add(headName);
        }
    }

    public void RemoveHeadFromRobotList(string headName)
    {
        RobotHeads.Remove(headName);
    }

    public void AddHeadToMaskList(string headName)
    {
        if (!MaskHeads.Contains(headName))
        {
            MaskHeads.Add(headName);
        }
    }

    public void RemoveHeadFromMaskList(string headName)
    {
        MaskHeads.Remove(headName);
    }

    public void AddHeadToHideFaceList(string headName)
    {
        if (HeadsHideFace == null)
        {
            HeadsHideFace = new List<string>();
        }
        if (!HeadsHideFace.Contains(headName))
        {
            HeadsHideFace.Add(headName);
        }
    }

    public void RemoveHeadFromHideFaceList(string headName)
    {
        if (HeadsHideFace == null)
        {
            HeadsHideFace = new List<string>();
        }
        HeadsHideFace.Remove(headName);
    }

    public void AddHairToDefaultList(string headName)
    {
        if (!HairDefault.Contains(headName))
        {
            HairDefault.Add(headName);
        }
    }

    public void RemoveHairFromDefaultList(string headName)
    {
        HairDefault.Remove(headName);
    }

    public void AddGlassesToDefaultList(string headName)
    {
        if (!GlassesDefault.Contains(headName))
        {
            GlassesDefault.Add(headName);
        }
    }

    public void RemoveGlassesFromDefaultList(string headName)
    {
        GlassesDefault.Remove(headName);
    }

    public void AddFacialFeaturesToDefaultList(string headName)
    {
        if (!FacialFeaturesDefault.Contains(headName))
        {
            FacialFeaturesDefault.Add(headName);
        }
    }

    public void RemoveFacialFeaturesFromDefaultList(string headName)
    {
        FacialFeaturesDefault.Remove(headName);
    }

    public bool IsHeadRobot(string headName)
    {
        return RobotHeads.Contains(headName);
    }

    public bool IsHeadMask(string headName)
    {
        return MaskHeads.Contains(headName);
    }

    public bool IsHeadHideFace(string headName)
    {
        return HeadsHideFace != null && HeadsHideFace.Contains(headName);
    }

    public CharacterPreset GetRandomOutfit(List<string> availableOutfits)
    {
        List<ColorPreset> availableColors = new List<ColorPreset>();
        foreach (ColorPreset color in ColorsPresets)
        {
            if (!string.IsNullOrEmpty(color.Name))
            {
                availableColors.Add(color);
            }
        }
        ColorPreset colorPreset = availableColors[Random.Range(0, availableColors.Count)];
        List<CharacterPreset> availablePresets = CharactersPresets.FindAll(i => availableOutfits.Contains(i.Name));

        List<string> hairNames = new List<string>();
        hairNames.AddRange(HairDefault);
        List<string> glassesNames = new List<string>();
        glassesNames.AddRange(GlassesDefault);
        List<string> facialFeaturesNames = new List<string>();
        facialFeaturesNames.AddRange(FacialFeaturesDefault);
        foreach (CharacterPreset preset in availablePresets)
        {
            if (!hairNames.Contains(preset.HairName))
            {
                hairNames.Add(preset.HairName);
            }
            if (!glassesNames.Contains(preset.GlassesName))
            {
                glassesNames.Add(preset.GlassesName);
            }
            if (!facialFeaturesNames.Contains(preset.FacialFeaturesName))
            {
                facialFeaturesNames.Add(preset.FacialFeaturesName);
            }
        }

        string hair = hairNames[Random.Range(0, hairNames.Count)];
        string glass = glassesNames[Random.Range(0, glassesNames.Count)];
        string facialFeature = facialFeaturesNames[Random.Range(0, facialFeaturesNames.Count)];

        CharacterPreset character = CreateInstance<CharacterPreset>();
        character.Init("Random", colorPreset, hair, glass, facialFeature, availablePresets[Random.Range(0, availablePresets.Count)].Arms, Character.GetRandomPersonality(), IsHeadRobot(hair), IsHeadMask(hair), IsHeadHideFace(hair), false);
        return character;
    }
}
