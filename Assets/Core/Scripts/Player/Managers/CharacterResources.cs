using UnityEngine;

public class CharacterResources : MonoBehaviour
{
    public CharacterAssets characterAssets;

    public Sprite[] HairSprites
    {
        get { return characterAssets.HairSprites; }
    }

    public Sprite[] GlassesSprites
    {
        get { return characterAssets.GlassesSprites; }
    }

    public Sprite[] FacialFeaturesSprites
    {
        get { return characterAssets.FacialFeaturesSprites; }
    }

    public Sprite[] ArmSprites
    {
        get { return characterAssets.ArmSprites; }
    }

    public Sprite[] ForearmSprites
    {
        get { return characterAssets.ForearmSprites; }
    }

    public Sprite[] HandSprites
    {
        get { return characterAssets.HandSprites; }
    }

    public Sprite[] HandClosedSprites
    {
        get { return characterAssets.HandClosedSprites; }
    }

    public Sprite[] HandPointSprites
    {
        get { return characterAssets.HandPointSprites; }
    }

    public Sprite[] BackSprites
    {
        get { return characterAssets.BackSprites; }
    }

    public RuntimeAnimatorController[] PersonalityAnims
    {
        get { return characterAssets.PersonalityAnims; }
    }

    public RuntimeAnimatorController[] PersonalityUIAnims
    {
        get { return characterAssets.PersonalityUIAnims; }
    }

    private void OnAssetLoaded(string path, object obj)
    {
        characterAssets = obj as CharacterAssets;
    }

    public Sprite GetHair(string spriteName)
    {
        return GetSprite(characterAssets.HairSprites, spriteName);
    }

    public Sprite GetGlasses(string spriteName)
    {
        return GetSprite(characterAssets.GlassesSprites, spriteName);
    }

    public Sprite GetFacialFeatures(string spriteName)
    {
        return GetSprite(characterAssets.FacialFeaturesSprites, spriteName);
    }

    public Sprite GetArm(string spriteName)
    {
        return GetSprite(characterAssets.ArmSprites, spriteName);
    }

    public Sprite GetForearm(string spriteName)
    {
        return GetSprite(characterAssets.ForearmSprites, spriteName);
    }

    public Sprite GetHandOpen(string spriteName)
    {
        return GetSprite(characterAssets.HandSprites, spriteName);
    }

    public Sprite GetHandClosed(string spriteName)
    {
        return GetSprite(characterAssets.HandClosedSprites, spriteName);
    }

    public Sprite GetHandPoint(string spriteName)
    {
        return GetSprite(characterAssets.HandPointSprites, spriteName);
    }

    public Sprite GetBack(string spriteName)
    {
        return GetSprite(characterAssets.BackSprites, spriteName);
    }

    public RuntimeAnimatorController GetPersonality(string personalityName)
    {
        foreach (var animatorController in characterAssets.PersonalityAnims)
        {
            if (animatorController.name == personalityName)
            {
                return animatorController;
            }
        }
        return null;
    }

    public RuntimeAnimatorController GetPersonalityUI(string personalityName)
    {
        foreach (var animatorController in characterAssets.PersonalityUIAnims)
        {
            if (animatorController.name == personalityName)
            {
                return animatorController;
            }
        }
        return null;
    }

    public Sprite GetStandard(string spriteName)
    {
        return GetSprite(characterAssets.StandardSprites, spriteName);
    }

    Sprite GetSprite(Sprite[] sprites, string spriteName)
    {
        foreach (var sprite in sprites)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }
        return null;
    }

    public bool IsLoaded()
    {
        return characterAssets != null;
    }
}