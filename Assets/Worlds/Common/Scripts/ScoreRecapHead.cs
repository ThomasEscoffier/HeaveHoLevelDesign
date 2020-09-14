using UnityEngine;
using UnityEngine.UI;
using Managers;

public class ScoreRecapHead : MonoBehaviour {

    public Image HeadBase;
    public Image Hair;
    public Image FacialFeatures;
    public Image Glasses;
    public Image Face;

    public Character.ePersonality Personnality;

    Animator characterFace = null;

    void Awake()
    {
        characterFace = GetComponentInChildren<Animator>();
    }

    public void SetOutfit(Character.Outfit characterOutfit)
    {
        HeadBase.color = characterOutfit.BodyColor;
        if (characterOutfit.HairSprite == null)
        {
            Hair.enabled = false;
        }
        else
        {
            Hair.enabled = true;
            Hair.sprite = characterOutfit.HairSprite;
        }

        if (characterOutfit.IsHideFace)
        {
            HeadBase.enabled = false;
            Glasses.enabled = false;
            FacialFeatures.enabled = false;
            Face.enabled = false;
            characterFace.enabled = false;
        }
        else
        {
            if (characterOutfit.GlassesSprite == null)
            {
                Glasses.enabled = false;
            }
            else
            {
                Glasses.enabled = true;
                Glasses.sprite = characterOutfit.GlassesSprite;
            }

            if (characterOutfit.FacialFeaturesSprite == null)
            {
                FacialFeatures.enabled = false;
            }
            else
            {
                FacialFeatures.enabled = true;
                FacialFeatures.sprite = characterOutfit.FacialFeaturesSprite;
            }
        }

        characterFace.runtimeAnimatorController = GameManager.Instance.CharacterAssets.GetPersonalityUI(Character.GetPersonalityAnimatorUIName(characterOutfit.Personality));
    }
}
