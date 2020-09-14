using UnityEngine.SceneManagement;

public class MusicManager : SoundModule
{
    public string DefaultMusic = "MusicDefault";
    public string DemoEndMusic = "MusicDemoEnd";
    public string TitleMusic = "Title";
    public string LanguageMusic = "";
    EventInfo currentMusic;
    EventInfo currentStingerEvent;

    const string TitleSceneName = "Title";
    const string SplashScreenSceneName = "Splashscreen";

    const string LevelParam = "Level";

    const string StartStinger = "StingerStart";
    const string DeathStinger = "StingerDeath";
    const string RespawnStinger = "StingerRespawn";
    const string HeadStinger = "StingerTete";
    const string ArmStinger = "StingerBras";
    const string VictoryStinger = "StingerArrivee";

    const string HoldCollectible = "MusicHoldToken";
    const string EndMusicParameter = "Deroulement";
    const string AreaParameter = "Area";

    string currentStinger = "";
    string currentStingerOneShot = "";

    bool isCurrentlyDynamic = false;

    void Start()
    {
        /*LevelProperties currentLevel = GameManager.Instance.GetLevelSelector().GetCurrentLevel();
        if (currentLevel != null)
        {
            if (currentLevel.IsUsingInteractiveMusic)
            {
                RunProperties currentRun = GameManager.Instance.GetLevelSelector().GetRunPropertiesFromLevel(currentLevel);
                int index = GameManager.Instance.GetLevelSelector().GetLevelIndexInRun(currentRun, currentLevel);
                PlayWorldMusic(currentLevel.Music, currentLevel.Stinger, index > -1 ? index : 0);
            }
            else
            {
                GetEvent(EventList, currentLevel.Music).InitSoundEvent();
                PlayMusic(currentLevel.Music);
            }
        }*/
        if (SceneManager.GetActiveScene().name.Contains(TitleSceneName))
        {
            GetEvent(EventList, TitleMusic).InitSoundEvent();
            PlayMusic(TitleMusic);
        }
        else if (SceneManager.GetActiveScene().name == SplashScreenSceneName)
        {
            /*var eventInfo = GetEvent(EventList, LanguageMusic);
            if (eventInfo != null)
            {
                eventInfo.InitSoundEvent();
                PlayMusic(LanguageMusic);
            }*/
        }
        /*else
        {
            GetEvent(EventList, DefaultMusic).InitSoundEvent();
            PlayMusic(DefaultMusic);
        }*/
    }

    public override void Update()
    {
        base.Update();
    }

    void ResetStinger()
    {
        currentStingerEvent.SetParameterValue(currentStinger, 0f);
        currentStinger = "";
    }

    public void StopMusic()
    {
        if (currentMusic != null)
        {
            StopEvent(currentMusic);
            TerminateEventInstance(currentMusic);
            currentMusic = null;
        }
        if (currentStinger != null)
        {
            TerminateEventInstance(currentStingerEvent);
            currentStingerEvent = null;
        }
    }

    public void PlayMusic(string musicName)
    {
        if (IsPlaying(musicName))
            return;

        if (currentMusic != null && !currentMusic.EventName.Equals(musicName))
        {
            StopMusic();
        }

        if (!musicName.Equals(""))
        {
            currentMusic = GetEvent(EventList, musicName);
            currentMusic.InitSoundEvent();
            PlayEvent(currentMusic);
            isCurrentlyDynamic = false;
        }
    }

    public void PlayMusic(EventInfo e)
    {
        if (e.IsPlaying())
            return;

        if (currentMusic != null && !currentMusic.EventName.Equals(e.EventName))
        {
            StopEvent(currentMusic);
            TerminateEventInstance(currentMusic);
        }

        if (!e.EventName.Equals(""))
        {
            currentMusic = e;
            currentMusic.InitSoundEvent();
            PlayEvent(currentMusic);
            isCurrentlyDynamic = false;
        }
    }

    public void PlayWorldMusic(string musicPath, string stingerOneShotName, string stingerPath, int index)
    {
        if (currentMusic != null)
        {
            StopEvent(currentMusic);
            TerminateEventInstance(currentMusic);
        }

        currentMusic = new EventInfo("currentMusic", musicPath, new System.Collections.Generic.List<ParamInfo>());
        currentMusic.InitSoundEvent();
        PlayMusic(currentMusic);

        if (currentMusic != null)
        {
            currentMusic.AddParameter(HoldCollectible, 0f);
            currentMusic.AddParameter(EndMusicParameter, 0f);
            currentMusic.AddParameter(AreaParameter, 0f);
        }

        currentStingerEvent = new EventInfo("currentStinger", stingerPath, new System.Collections.Generic.List<ParamInfo>() { new ParamInfo(LevelParam, index + 1), new ParamInfo(StartStinger, 0f),
                                                                                                                                new ParamInfo(RespawnStinger, 0f), new ParamInfo(RespawnStinger, 0f),
                                                                                                                                new ParamInfo(DeathStinger, 0f), new ParamInfo(HeadStinger, 0f),
                                                                                                                                new ParamInfo(ArmStinger, 0f), new ParamInfo(VictoryStinger, 0f) });
        currentStingerOneShot = stingerOneShotName;
        currentStingerEvent.InitSoundEvent();
        isCurrentlyDynamic = true;
    }

    public void PauseCurrentMusic()
    {
        if (currentMusic == null || !IsPlaying(currentMusic))
            return;

        PauseEvent(currentMusic);
    }

    public void UnPauseCurrentMusic()
    {
        if (currentMusic == null || IsPlaying(currentMusic))
            return;

        PlayEvent(currentMusic);
    }

    public void AddParameter(string paramName, float paramValue = 0)
    {
        currentMusic?.AddParameter(paramName, paramValue);
    }

    public void SetMusicParameter(string paramName, float paramValue)
    {
        currentMusic?.SetParameterValue(paramName, paramValue);
    }

    public EventInfo GetCurrentMusicEvent()
    {
        return currentMusic;
    }

    public void LaunchStartMusicEvent()
    {
        if (currentMusic == null || currentStingerEvent == null)
            return;
        PlayOneShot(currentStingerOneShot + "start");
    }

    public void LaunchDeathMusicEvent()
    {
        if (currentMusic == null || currentStingerEvent == null)
            return;
        PlayOneShot(currentStingerOneShot + "death");
    }

    public void LaunchRespawnMusicEvent()
    {
        if (currentMusic == null || currentStingerEvent == null)
            return;
        PlayOneShot(currentStingerOneShot + "respawn");
    }

    public void LaunchEndMusicEvent()
    {
        if (currentMusic == null)
            return;
        currentMusic.SetParameterValue(EndMusicParameter, 1f);
    }

    public void LaunchBumpCharacterMusicEvent(int intensity)
    {
        if (currentMusic == null || currentStingerEvent == null)
            return;
        float value = 0f;
        if (intensity == 1)
        {
            value = 1f;
        }
        else if (intensity == 2)
        {
            value = 2f;
        }
        else if (intensity == 3)
        {
            value = 3f;
        }
        else if (intensity == 4)
        {
            value = 4f;
        }
        ResetStinger();
        currentStingerEvent.SetParameterValue(HeadStinger, value);
        currentStinger = HeadStinger;
        currentStingerEvent.PlayEvent();
    }

    public void LaunchBumpCharacterBodyMusicEvent(int intensity)
    {
        if (currentMusic == null || currentStingerEvent == null)
            return;
        float value = 0f;
        if (intensity == 1)
        {
            value = 1f;
        }
        else if (intensity == 2)
        {
            value = 2f;
        }
        else if (intensity == 3)
        {
            value = 3f;
        }
        else if (intensity == 4)
        {
            value = 4f;
        }
        ResetStinger();
        currentStingerEvent.SetParameterValue(ArmStinger, value);
        currentStinger = ArmStinger;
        currentStingerEvent.PlayEvent();
    }

    public void LaunchPlayerInVictoryEvent(int nbCharacters)
    {
        if (currentMusic == null || currentStingerEvent == null)
            return;
        float value = 0f;
        if (nbCharacters == 1)
        {
            value = 1f;
        }
        else if (nbCharacters == 2)
        {
            value = 2f;
        }
        else if (nbCharacters == 3)
        {
            value = 3f;
        }
        else if (nbCharacters == 4)
        {
            value = 4f;
        }
        ResetStinger();
        currentStingerEvent.SetParameterValue(VictoryStinger, value);
        currentStinger = VictoryStinger;
        currentStingerEvent.PlayEvent();
    }

    public void SetGrabCollectibleEvent(bool state)
    {
        if (currentMusic == null)
            return;
        currentMusic.SetParameterValue(HoldCollectible, state ? 1f : 0f);
    }

    public void SetAreaEvent(float value)
    {
        if (currentMusic == null || currentStingerEvent == null)
            return;
        currentMusic.SetParameterValue(AreaParameter, value);
    }

    public bool GetIsDynamic()
    {
        return isCurrentlyDynamic;
    }

    public void SetIsDynamic(bool state)
    {
        isCurrentlyDynamic = state;
    }

    public void ChangeMusicPitch(float newPitch)
    {
        if (currentMusic != null)
        {
            currentMusic.ChangePitch(newPitch);
        }
    }
}
