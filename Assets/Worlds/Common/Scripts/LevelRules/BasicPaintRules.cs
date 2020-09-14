using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Rules/BasicPaint")]
public class BasicPaintRules : BasicRules {

    public AAccessory PaintGunPrefab = null;

    public override void InitCharacter(Character character)
    {
        base.InitCharacter(character);
        if (PaintGunPrefab != null)
        {
            character.LeftHand.AddNewAccessory(Instantiate(PaintGunPrefab, character.LeftHand.transform));
            character.RightHand.AddNewAccessory(Instantiate(PaintGunPrefab, character.RightHand.transform));
        }
    }
}
