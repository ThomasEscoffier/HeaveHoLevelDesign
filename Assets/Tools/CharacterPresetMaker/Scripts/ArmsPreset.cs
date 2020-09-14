using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CharacterPresets/ArmsPreset")]
public class ArmsPreset : ScriptableObject
{
    public string Name;

    public string LeftArmName;
    public string LeftForearmName;
    public string LeftHandName;
    public string LeftHandClosedName;
    public string LeftHandPointName;

    public string RightArmName;
    public string RightForearmName;
    public string RightHandName;
    public string RightHandClosedName;
    public string RightHandPointName;

    public string BackName;

    public bool IsHideHands;

    public void Init(string Name_, string LeftArmName_, string LeftForearmName_, string LeftHandName_, string LeftHandClosedName_, string LeftHandPointName_,
                    string RightArmName_, string RightForearmName_, string RightHandName_, string RightHandClosedName_, string RightHandPointName_, string BackName_, bool IsHideHands_)
    {
        Name = Name_;

        LeftArmName = LeftArmName_;
        LeftForearmName = LeftForearmName_;
        LeftHandName = LeftHandName_;
        LeftHandClosedName = LeftHandClosedName_;
        LeftHandPointName = LeftHandPointName_;

        RightArmName = RightArmName_;
        RightForearmName = RightForearmName_;
        RightHandName = RightHandName_;
        RightHandClosedName = RightHandClosedName_;
        RightHandPointName = RightHandPointName_;

        BackName = BackName_;

        IsHideHands = IsHideHands_;
    }

    public bool IsSame(ArmsPreset otherArm)
    {
        return LeftArmName == otherArm.LeftArmName && LeftForearmName == otherArm.LeftForearmName
            && LeftHandName == otherArm.LeftHandName && LeftHandClosedName == otherArm.LeftHandClosedName && LeftHandPointName == otherArm.LeftHandPointName
            && RightArmName == otherArm.RightArmName && RightForearmName == otherArm.RightForearmName
            && RightHandName == otherArm.RightHandName && RightHandClosedName == otherArm.RightHandClosedName && RightHandPointName == otherArm.RightHandPointName
            && BackName == otherArm.BackName && IsHideHands == otherArm.IsHideHands;
    }

    public bool IsSame(string otherLeftArmName, string otherLeftForearmName, string otherLeftHandName, string otherLeftHandClosedName, string otherLeftHandPointName,
                        string otherRightArmName, string otherRightForearmName, string otherRightHandName, string otherRightHandClosedName, string otherRightHandPointName,
                        string otherBackName, bool otherIsHideHands)
    {
        return LeftArmName == otherLeftArmName && LeftForearmName == otherLeftForearmName
            && LeftHandName == otherLeftHandName && LeftHandClosedName == otherLeftHandClosedName && LeftHandPointName == otherLeftHandPointName
            && RightArmName == otherRightArmName && RightForearmName == otherRightForearmName
            && RightHandName == otherRightHandName && RightHandClosedName == otherRightHandClosedName && RightHandPointName == otherRightHandPointName
            && BackName == otherBackName && IsHideHands == otherIsHideHands;
    }
}
