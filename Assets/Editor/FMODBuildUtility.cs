using BitCode.Editor.Build;
using FMODUnity;

public static class FMODBuildUtility 
{
    [BuildConfiguration("CopyFMODBanks")]
    public static void CopyFMODBanks()
    {
        EventManager.UpdateCache();
        EventManager.CopyToStreamingAssets();
    }
}
