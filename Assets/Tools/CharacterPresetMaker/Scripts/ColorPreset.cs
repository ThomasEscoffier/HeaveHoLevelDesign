using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CharacterPresets/ColorPreset")]
public class ColorPreset : ScriptableObject
{
    public string Name;

    public float r;
    public float g;
    public float b;
    public float a;

    public void Init(string Name_, float r_, float g_, float b_, float a_)
    {
        Name = Name_;
        r = r_;
        g = g_;
        b = b_;
        a = a_;
    }

    public bool IsSame(ColorPreset otherColor)
    {
        return r == otherColor.r && g == otherColor.g && b == otherColor.b && a == otherColor.a;
    }

    public bool IsSame(Color otherColor)
    {
        return r == otherColor.r && g == otherColor.g && b == otherColor.b && a == otherColor.a;
    }
}
