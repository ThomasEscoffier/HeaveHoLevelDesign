using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class SoundModule : MonoBehaviour {

    [System.Serializable]
    public class EventInfo
    {
        public string EventName = "";
        public string EventPath = "";
        public bool IsInitAtStart = false;
        public bool IsRunningAtStart = false;
        List<ParamInfo> EventParameters = new List<ParamInfo>();
        public FMOD.Studio.EventInstance eventInstance { get; private set;}

        public EventInfo(string eventName, string eventPath, List<ParamInfo> parameters)
        {
            EventName = eventName;
            EventPath = eventPath;
            EventParameters.AddRange(parameters);
        }

        public void InitSoundEvent()
        {
            // TODO work out why this catch is necessary.
            try
            {
                eventInstance = FMODUnity.RuntimeManager.CreateInstance(EventPath);
                if (EventParameters != null)
                {
                    for (int i = 0; i < EventParameters.Count; ++i)
                    {
                        EventParameters[i].InitParam(eventInstance);
                    }
                }

                if (IsRunningAtStart)
                {
                    eventInstance.start();
                }
            }
            catch (EventNotFoundException)
            {
                Debug.LogError("Unexpected FMOD error : " + EventName);
            }
        }

        public void InitSoundEvent(Transform t, Rigidbody2D rb)
        {
            eventInstance = FMODUnity.RuntimeManager.CreateInstance(EventPath);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInstance, t, rb);
            //eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(t.gameObject, rb));
            if (EventParameters != null)
            {
                for (int i = 0; i < EventParameters.Count; ++i)
                {
                    EventParameters[i].InitParam(eventInstance);
                }
            }
            if (IsRunningAtStart)
            {
                eventInstance.start();
            }
        }

        public void AttachInstanceTo(Transform t, Rigidbody2D rb)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInstance, t, rb);
        }

        public void Set3DAttributes(Transform t, Rigidbody2D rb)
        {
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(t, rb));
        }

        public void TerminateSoundEvent()
        {
            if (eventInstance.isValid())
            {
                FMODUnity.RuntimeManager.DetachInstanceFromGameObject(eventInstance);
                eventInstance.release();
            }
        }

        public void AddParameter(string name, float defaultValue)
        {
            if (EventParameters == null)
            {
                EventParameters = new List<ParamInfo>();
            }
            ParamInfo newParam = new ParamInfo(name, defaultValue);
            newParam.InitParam(eventInstance);
            EventParameters.Add(newParam);
        }

        public void SetParameterValue(string name, float value)
        {
            if (EventParameters == null)
            {
                Debug.LogError("EventParameters is null!");
                return;
            }
            for (int i = 0; i < EventParameters.Count; ++i)
            {
                if (EventParameters[i].ParamName == name)
                {
                    EventParameters[i].SetParamValue(value);
                    return;
                }
            }
        }

        public FMOD.Studio.EventInstance PlayEvent()
        {
            if (!eventInstance.isValid())
            {
                Debug.LogError("Error : The event : " + EventName + " is not instanciated");
                return new FMOD.Studio.EventInstance();
            }

            FMOD.Studio.PLAYBACK_STATE playState;
            bool isPaused = false;

            eventInstance.getPlaybackState(out playState);
            eventInstance.getPaused(out isPaused);

            if (isPaused)
            {
                eventInstance.setPaused(false);
            }
            else
            {
                FMOD.RESULT result = eventInstance.start();
                if (result != FMOD.RESULT.OK)
                {
                    Debug.Log(result);
                }
            }
            return eventInstance;
        }

        public void PlayOneShot()
        {
            FMODUnity.RuntimeManager.PlayOneShot(EventPath);
        }

        public void PlayOneShot(Vector3 pos)
        {
            FMODUnity.RuntimeManager.PlayOneShot(EventPath, pos);
        }

        public void PlayOneShot(GameObject obj)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(EventPath, obj);
        }

        public void StopEvent()
        {
            FMOD.Studio.PLAYBACK_STATE playState;
            eventInstance.getPlaybackState(out playState);

            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PauseEvent()
        {
            FMOD.Studio.PLAYBACK_STATE playState;
            bool isPaused = false;

            eventInstance.getPlaybackState(out playState);
            eventInstance.getPaused(out isPaused);

            eventInstance.getPaused(out isPaused);
            if (!isPaused && playState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                eventInstance.setPaused(true);
            }
        }

        public bool IsPlaying()
        {
            FMOD.Studio.PLAYBACK_STATE playState;
            bool isPaused = false;

            eventInstance.getPlaybackState(out playState);
            eventInstance.getPaused(out isPaused);

            return !isPaused && playState == FMOD.Studio.PLAYBACK_STATE.PLAYING;
        }

        public bool IsPaused()
        {
            bool isPaused = false;
            eventInstance.getPaused(out isPaused);
            return isPaused;
        }

        public int GetLength()
        {
            int length = 0;

            FMOD.Studio.EventDescription des = new FMOD.Studio.EventDescription();
            eventInstance.getDescription(out des);
            des.getLength(out length);

            return length;
        }

        public void ChangePitch(float newPitch)
        {
            eventInstance.setPitch(newPitch);
        }
    }

    [System.Serializable]
    public class ParamInfo
    {
        public string ParamName = "";
        public float ParamValue = 0f;
        FMOD.Studio.ParameterInstance parameterInstance;

        public ParamInfo(string paramName, float paramValue = 0f)
        {
            ParamName = paramName;
            ParamValue = paramValue;
        }

        public void InitParam(FMOD.Studio.EventInstance eventInstance)
        {
            eventInstance.getParameter(ParamName, out parameterInstance);
            if (ParamValue != 0f)
            {
                SetParamValue(ParamValue);
            }
        }

        public void SetParamValue(float value)
        {
            if (parameterInstance.isValid())
            {
                parameterInstance.setValue(value);
                ParamValue = value;
            }
            else
            {
                Debug.LogError("Error : sound parameter : " + ParamName + " doesn't exist");
            }
        }
    }

    public List<EventInfo> EventList;

    protected Rigidbody2D rb;
    protected List<FMOD.Studio.EventInstance> playingInstances = new List<FMOD.Studio.EventInstance>();

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        for (int i = 0; i < EventList.Count; ++i)
        {
            if (EventList[i].IsInitAtStart)
            {
                EventList[i].InitSoundEvent(transform, rb);
            }
        }
    }

    public virtual void Update()
    {
        if (playingInstances.Count <= 0)
            return;

        for (var index = playingInstances.Count - 1; index >= 0; index--)
        {
            var instance = playingInstances[index];
            FMOD.Studio.PLAYBACK_STATE playState;
            instance.getPlaybackState(out  playState);

            if (playState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                playingInstances.RemoveAt(index);    
            }
        }
    }

    public void InitEvent(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e != null && !e.eventInstance.isValid())
        {
            e.InitSoundEvent(transform, rb);
        }
    }

    public void TerminateEventInstance(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e != null)
        {
            e.TerminateSoundEvent();
        }
    }

    public void TerminateEventInstance(EventInfo e)
    {
        if (e != null)
        {
            e.TerminateSoundEvent();
        }
    }

    public void StopAllPlayingEvents()
    {
        if (playingInstances.Count <= 0)
            return;

        foreach (FMOD.Studio.EventInstance instance in playingInstances)
        {
            FMOD.Studio.PLAYBACK_STATE playState;
            bool isPaused = false;

            instance.getPlaybackState(out playState);
            instance.getPaused(out isPaused);

            if (playState != FMOD.Studio.PLAYBACK_STATE.STOPPED || isPaused)
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    public void SetParameterValue(string eventName, string eventParameterName, float value)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e != null)
        {
            e.SetParameterValue(eventParameterName, value);
        }
    }

    public void AddParameter(string eventName, string eventParameterName, float defaultValue)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e != null)
        {
            e.AddParameter(eventParameterName, defaultValue);
        }
    }

    protected EventInfo GetEvent(List<EventInfo> eventList, string eventName)
    {
        for (int i = 0; i < eventList.Count; ++i)
        {
            if (eventList[i].EventName == eventName)
            {
                return eventList[i];
            }
        }
        return null;
    }

    public bool DoesEventExist(string eventName)
    {
        foreach (EventInfo e in EventList)
        {
            if (e.EventName == eventName)
            {
                return true;
            }
        }
        return false;
    }

    public void PlayEvent(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            Debug.LogError("Error : Couldn't find event : " + eventName);
            return;
        }

        if (playingInstances.Contains(e.eventInstance))
        {
            e.PlayEvent();
        }
        else
        {
            playingInstances.Add(e.PlayEvent());
        }
    }

    public void PlayEvent(EventInfo e)
    {
        if (playingInstances.Contains(e.eventInstance))
        {
            e.PlayEvent();
        }
        else
        {
            playingInstances.Add(e.PlayEvent());
        }
    }

    public void PlayOneShot(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            Debug.LogError("Error : Couldn't find event : " + eventName);
            return;
        }

        e.PlayOneShot();
    }

    public void PlayOneShot(EventInfo e)
    {
        e.PlayOneShot();
    }

    public void PlayOneShot(string eventName, Vector3 pos)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            Debug.LogError("Error : Couldn't find event : " + eventName);
            return;
        }

        e.PlayOneShot(pos);
    }

    public void PlayOneShot(string eventName, GameObject obj)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            Debug.LogError("Error : Couldn't find event : " + eventName);
            return;
        }

        e.PlayOneShot(obj);
    }

    public void PlayOneShotOnObjFromAnimation(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            Debug.LogError("Error : Couldn't find event : " + eventName);
            return;
        }

        e.PlayOneShot(gameObject);
    }

    public void StopEvent(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            Debug.LogError("Error : Couldn't find event : " + eventName);
            return;
        }

        e.StopEvent();
    }

    public void StopEvent(EventInfo e)
    {
        e?.StopEvent();
    }

    public void PauseEvent(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            Debug.LogError("Error : Couldn't find event : " + eventName);
            return;
        }

        e.PauseEvent();
    }

    public void PauseEvent(EventInfo e)
    {
        e.PauseEvent();
    }

    public bool IsPlaying(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            return false;
        }
        return e.IsPlaying();
    }

    public bool IsPlaying(EventInfo e)
    {
        return e.IsPlaying();
    }

    public bool IsPaused(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            return false;
        }
        return e.IsPaused();
    }

    public bool IsPaused(EventInfo e)
    {
        return e.IsPaused();
    }

    public void AttachInstanceTo(string eventName, Transform t, Rigidbody2D rb)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            return;
        }

        e.AttachInstanceTo(t, rb);
    }

    public void Set3DAttributes(string eventName, Transform t, Rigidbody2D rb)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            return;
        }

        e.Set3DAttributes(t, rb);
    }

    public int GetLength(string eventName)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            return 0;
        }
        return e.GetLength();
    }

    public void ChangePitch(string eventName, float pitch)
    {
        EventInfo e = GetEvent(EventList, eventName);
        if (e == null)
        {
            return;
        }
        e.ChangePitch(pitch);
    }

    void OnDestroy()
    {
        for (int i = 0; i < EventList.Count; ++i)
        {
            if (EventList[i].IsInitAtStart)
            {
                EventList[i].StopEvent();
                EventList[i].TerminateSoundEvent();
            }
        }
    }
}
