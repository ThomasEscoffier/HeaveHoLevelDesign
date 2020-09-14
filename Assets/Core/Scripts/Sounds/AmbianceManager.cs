using System.Collections.Generic;

public class AmbianceManager : SoundModule
{
    public List<EventInfo> SnapshotList = new List<EventInfo>();

    EventInfo currentAmbiance = null;
    EventInfo currentSnapshot = null;

    public override void Awake()
    {
        for (int i = 0; i < EventList.Count; ++i)
        {
            EventList[i].InitSoundEvent();
        }
    }

    void Start()
    {
        LevelProperties currentLevel = GameManager.Instance.GetLevelSelector().GetCurrentLevel();
        if (currentLevel != null && !currentLevel.Ambiance.Equals(""))
        {
            if (currentLevel.IsUsingInteractiveMusic)
            {
                StartWorldAmbiance(GameManager.Instance.GetLevelSelector().GetCurrentLevel().Ambiance);
                StartWorldSnapshot(GameManager.Instance.GetLevelSelector().GetCurrentLevel().Snapshot);
            }
            else
            {
                StartAmbiance(currentLevel.Ambiance);
                StartSnapshot(currentLevel.Snapshot);
            }
        }
        else
        {
            StartSnapshot("Menu");
        }
    }

    public void StartAmbiance(string ambianceName)
    {
        if ((currentAmbiance == null || !currentAmbiance.EventName.Equals(ambianceName)) && !string.IsNullOrEmpty(ambianceName))
        {
            currentAmbiance = GetEvent(EventList, ambianceName);
            currentAmbiance.InitSoundEvent();
            PlayEvent(currentAmbiance);
        }
    }

    public void StartAmbiance(EventInfo e)
    {
        if (currentAmbiance == null || !currentAmbiance.Equals(e))
        {
            currentAmbiance = e;
            currentAmbiance.InitSoundEvent();
            PlayEvent(currentAmbiance);
        }
    }

    public void StartWorldAmbiance(string ambiancePath)
    {
        StartAmbiance(new EventInfo("currentAmbiance", ambiancePath, new List<ParamInfo>()));
    }

    public void StopCurrentAmbiance()
    {
        if (currentAmbiance != null)
        {
            StopEvent(currentAmbiance);
            TerminateEventInstance(currentAmbiance);
            currentAmbiance = null;
        }
    }

    public void StartSnapshot(string snapshotName)
    {
        if ((currentSnapshot == null || !currentSnapshot.EventName.Equals(snapshotName)) && !string.IsNullOrEmpty(snapshotName))
        {
            currentSnapshot = GetEvent(SnapshotList, snapshotName);
            currentSnapshot.InitSoundEvent();
            PlayEvent(currentSnapshot);
        }
    }

    public void StartSnapshot(EventInfo e)
    {
        if (currentSnapshot == null || !currentSnapshot.Equals(e))
        {
            currentSnapshot = e;
            currentSnapshot.InitSoundEvent();
            PlayEvent(currentSnapshot);
        }
    }

    public void StartWorldSnapshot(string snapshotPath)
    {
        if (!string.IsNullOrEmpty(snapshotPath))
        {
            StartSnapshot(new EventInfo("currentAmbiance", snapshotPath, new List<ParamInfo>()));
        }
    }

    public void StopCurrentSnapshot()
    {
        if (currentSnapshot != null)
        {
            StopEvent(currentSnapshot);
            TerminateEventInstance(currentSnapshot);
            currentSnapshot = null;
        }
    }

    public void AddAmbianceParameter(string paramName, float paramValue = 0)
    {
        currentAmbiance?.AddParameter(paramName, paramValue);
    }

    public void SetAmbianceParameter(string paramName, float paramValue)
    {
        currentAmbiance?.SetParameterValue(paramName, paramValue);
    }
}
