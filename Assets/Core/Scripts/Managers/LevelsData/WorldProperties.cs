using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Level/WorldProperties", order = 1)]
public class WorldProperties : ScriptableObject
{
    public string WorldName;
    public string LocaKey;
    public string TypeName;
    public int OrderInMenu;
    public Color ColorInMenu = Color.black;
    public string BackgroundAnimatorName;
    public RunProperties[] runs;
    public string[] requiredAssetbundles;
}

[System.Serializable]
public class RunProperties
{
    public int NbPlayerMin = 1;
    public int NbPlayerMax = 2;
    public List<LevelProperties> properties;
}

[System.Serializable]
public class LevelProperties
{
    public enum eLevelType
    {
        NORMAL,
        SCROLLING,
        MINIGAME,
        BOSS,
        OTHER,
    }

    public string LevelName = "";
    [Tooltip("Levels such as Tutorials, Intros...")]
    public string LevelToLoadBefore = "";
    [Tooltip("Levels such as Score recaps, Outros...")]
    public string LevelToLoadAfter = "";
    [Tooltip("The higher the number the more difficult")]
    public int Difficulty = 0;
    public eLevelType LevelType = eLevelType.NORMAL;
    public bool IsUsingInteractiveMusic = false;
    public string Music = "MusicDefault";
    public string Ambiance = "";
    [Tooltip("Sound effect applied. Ex : Echo")]
    public string Snapshot = "";
    public string Stinger = "";
    public string StingerOnesShot = "";
    public Color TintCharacter = Color.white;
    public Color TintCharacterBody = Color.clear;
    public Color ColorScorePanel = Color.green;
    public bool IsAvailableForFusion = true;
    public string TransitionName = "";
    public bool HasToken = true;
}