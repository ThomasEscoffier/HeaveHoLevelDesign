using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Managers;
using Rewired;

public class MainMenu : MonoBehaviour
{
    public GameManager GameManagerPrefab = null;

    [System.Serializable]
    public struct TitleRessources
    {
        public string TitleName;
        public string BundleName;
        public string WorldName;
    }
    public int TitleNormalIndex = 0;
    public TitleRessources[] Titles;

    bool isTitleLaunched = false;
    int indexTitleToLaunch;
    List<TitleRessources> availableTitles = new List<TitleRessources>();

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instantiate(GameManagerPrefab);
        }

        GameManager.Instance.GetMusicManager().PlayMusic(GameManager.Instance.GetMusicManager().DefaultMusic);
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            player.controllers.maps.SetAllMapsEnabled(false);
            player.controllers.maps.SetMapsEnabled(true, "Menu", "AssistedControlLeft");
        }

        availableTitles.Add(Titles[0]);
        for (int i = 1; i < Titles.Length; ++i)
        {
            availableTitles.Add(Titles[i]);
        }
    }

    public void LoadTitleScreen()
    {
        if (isTitleLaunched)
            return;

        indexTitleToLaunch = Random.Range(0, availableTitles.Count);
        SceneManager.LoadScene(availableTitles[indexTitleToLaunch].TitleName, LoadSceneMode.Single);
        isTitleLaunched = true;
    }
}
