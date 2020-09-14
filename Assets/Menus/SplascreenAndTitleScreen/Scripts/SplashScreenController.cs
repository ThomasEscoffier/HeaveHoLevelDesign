using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour {

    public float TimeFade = 1f;
    public float TimeBeforeChangeScene = 3f;
    float timer = 0f;

    bool isFadingIn = true;
    bool isFadingOut = false;

    public Image ControllerImage = null;
    public Text ControllerText = null;

    Color startColor;
    Color endColor;

    [System.Serializable]
    public struct TitleRessources
    {
        public string TitleName;
        public string BundleName;
        public string WorldName;
    }
    public TitleRessources[] Titles;

    bool isTitleLaunched = false;
    int indexTitleToLaunch;
    List<TitleRessources> availableTitles = new List<TitleRessources>();

    void Awake ()
    {
        startColor = ControllerImage.color;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        availableTitles.Add(Titles[0]);
        for (int i = 1; i < Titles.Length; ++i)
        {
            availableTitles.Add(Titles[i]);
        }
    }

    void Update()
    {
        if (isFadingIn)
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeFade);
            ControllerImage.color = Color.Lerp(endColor, startColor, timer / TimeFade);
            ControllerText.color = Color.Lerp(endColor, startColor, timer / TimeFade);
            if (timer == TimeFade)
            {
                timer = 0f;
                isFadingIn = false;
            }
        }
        else if (isFadingOut)
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeFade);
            ControllerImage.color = Color.Lerp(startColor, endColor, timer / TimeFade);
            ControllerText.color = Color.Lerp(startColor, endColor, timer / TimeFade);
            if (!isTitleLaunched && timer == TimeFade)
            {
                LoadTitleScreen();
            }
        }
        else
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeChangeScene);
            if (timer == TimeBeforeChangeScene)
            {
                timer = 0f;
                isFadingOut = true;
            }
        }
    }

    public void LoadTitleScreen()
    {
        indexTitleToLaunch = Random.Range(0, availableTitles.Count);
        SceneManager.LoadScene(availableTitles[indexTitleToLaunch].TitleName, LoadSceneMode.Single);
        isTitleLaunched = true;
    }
}
