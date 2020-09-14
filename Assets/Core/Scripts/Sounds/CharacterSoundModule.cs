using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class CharacterSoundModule : SoundModule {

    public string HappyEventPath;
    public string GrumpyEventPath;
    public string ScaredEventPath;
    public string WeirdoEventPath;

    public string HappyElectricEventPath;
    public string GrumpyElectricEventPath;
    public string ScaredElectricEventPath;
    public string WeirdoElectricEventPath;

    public int HappyNbRandomElectricSounds = 4;
    public int GrumpyNbRandomElectricSounds = 4;
    public int ScaredNbRandomElectricSounds = 4;
    public int WeirdoNbRandomElectricSounds = 4;

    public string HappyChargeEventPath;
    public string GrumpyChargeEventPath;
    public string ScaredChargeEventPath;
    public string WeirdoChargeEventPath;

    public int HappyNbRandomChargeSounds = 4;
    public int GrumpyNbRandomChargeSounds = 4;
    public int ScaredNbRandomChargeSounds = 4;
    public int WeirdoNbRandomChargeSounds = 4;

    const string personalityParameterName = "State";
    const string maskParameterName = "Masque";
    const string robotParameterName = "Robot";
    EventInfo currentPersonalityEvent = null;
    EventInfo currentElectricEvent = null;
    EventInfo currentChargeEvent = null;
    List<EventInfo> electricEvents = new List<EventInfo>();
    List<EventInfo> chargeEvents = new List<EventInfo>();

    public enum eState
    {
        NONE = 0,
        WAIT,
        LOSE_ARM,
        HIT,
        FALL,
        EFFORT1,
        EFFORT2,
        EFFORT3,
        LEVELEND,
        LOSER,
        POINT,
        SELECTED,
        VICTORIOUS,
        RELEASED_CHARGE,
        ANGRY,
        JOY,
    }

    string GetPersonalityEventPath(Character.ePersonality personality)
    {
        string path = "";
        switch (personality)
        {
            case Character.ePersonality.HAPPY:
                path = HappyEventPath;
                break;
            case Character.ePersonality.GRUMPY:
                path = GrumpyEventPath;
                break;
            case Character.ePersonality.SCARED:
                path = ScaredEventPath;
                break;
            case Character.ePersonality.WEIRDO:
                path = WeirdoEventPath;
                break;
            default:
                break;
        }
        return path;
    }

    string GetPersonalityElectricEventPath(Character.ePersonality personality)
    {
        string path = "";
        switch (personality)
        {
            case Character.ePersonality.HAPPY:
                path = HappyElectricEventPath;
                break;
            case Character.ePersonality.GRUMPY:
                path = GrumpyElectricEventPath;
                break;
            case Character.ePersonality.SCARED:
                path = ScaredElectricEventPath;
                break;
            case Character.ePersonality.WEIRDO:
                path = WeirdoElectricEventPath;
                break;
            default:
                break;
        }
        return path;
    }

    string GetPersonalityChargeEventPath(Character.ePersonality personality)
    {
        string path = "";
        switch (personality)
        {
            case Character.ePersonality.HAPPY:
                path = HappyChargeEventPath;
                break;
            case Character.ePersonality.GRUMPY:
                path = GrumpyChargeEventPath;
                break;
            case Character.ePersonality.SCARED:
                path = ScaredChargeEventPath;
                break;
            case Character.ePersonality.WEIRDO:
                path = WeirdoChargeEventPath;
                break;
            default:
                break;
        }
        return path;
    }

    int GetCharacterParameterValue(eState value)
    {
        switch(value)
        {
            case eState.WAIT:
                return 1;
            case eState.LOSE_ARM:
                return 2;
            case eState.HIT:
                return 3;
            case eState.FALL:
                return 4;
            case eState.EFFORT1:
                return 5;
            case eState.EFFORT2:
                return 6;
            case eState.EFFORT3:
                return 7;
            case eState.LEVELEND:
                return 8;
            case eState.LOSER:
                return 9;
            case eState.POINT:
                return 10;
            case eState.SELECTED:
                return 11;
            case eState.VICTORIOUS:
                return 12;
            case eState.RELEASED_CHARGE:
                return 14;
            case eState.JOY:
                return 15;
            case eState.ANGRY:
                return 16;
        }
        return 0;
    }

    public override void Update()
    {
        base.Update();
        if (currentPersonalityEvent != null)
        {
           // Replaced the below so that the array [attachedInstances] in RuntimeManager.cs does not grow every update()
           // currentPersonalityEvent.AttachInstanceTo(transform, rb);
           
            currentPersonalityEvent.eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform,rb));
        }

        if (currentChargeEvent != null)
        {
            currentChargeEvent.eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform, rb));
        }

        if (currentElectricEvent != null)
        {
            currentElectricEvent.eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform, rb));
        }
    }

    public void InitPersonality(Character.ePersonality personality, bool isRobot, bool isMask, bool attachedTransform = true, eState state = eState.NONE)
    {
        List<ParamInfo> parameters = new List<ParamInfo>();

        parameters.Add(new ParamInfo(personalityParameterName, GetCharacterParameterValue(state)));
        parameters.Add(new ParamInfo(maskParameterName, isMask ? 1f : 0f));
        parameters.Add(new ParamInfo(robotParameterName, isRobot ? 1f : 0f));

        if (currentPersonalityEvent != null)
        {
            currentPersonalityEvent.TerminateSoundEvent();
            EventList.Remove(currentPersonalityEvent);
            currentPersonalityEvent = null;
        }

        currentPersonalityEvent = new EventInfo(Character.GetPersonalityName(personality), GetPersonalityEventPath(personality), parameters);
        currentPersonalityEvent.IsRunningAtStart = true;
        currentPersonalityEvent.IsInitAtStart = true;

        if (attachedTransform)
        {
            currentPersonalityEvent.InitSoundEvent(transform, rb);
        }
        else
        {
            currentPersonalityEvent.InitSoundEvent();
        }
        EventList.Add(currentPersonalityEvent);

        //InitElectricEvents(personality, isRobot, isMask);
        //InitCharge(personality, isRobot, isMask);
    }

    public void SetParameterValue(eState value)
    {
        currentPersonalityEvent.SetParameterValue(personalityParameterName, (float)value);
    }

    public void PlayCharacterSound(eState state)
    {
        //StopElectric();
        //StopCharge();
        currentPersonalityEvent.SetParameterValue(personalityParameterName, GetCharacterParameterValue(state));
        PlayEvent(currentPersonalityEvent); // Use the general function so the event is added to the playing instances
    }

    public void StopCharacterEvent()
    {
        if (currentPersonalityEvent != null)
        {
            currentPersonalityEvent.StopEvent();
        }
    }

    public void InitElectricEvents(Character.ePersonality personality, bool isRobot, bool isMask)
    {
        int NbRandomElectricSounds = 0;
        switch (personality)
        {
            case Character.ePersonality.HAPPY:
                NbRandomElectricSounds = HappyNbRandomElectricSounds;
                break;
            case Character.ePersonality.GRUMPY:
                NbRandomElectricSounds = GrumpyNbRandomElectricSounds;
                break;
            case Character.ePersonality.SCARED:
                NbRandomElectricSounds = ScaredNbRandomElectricSounds;
                break;
            case Character.ePersonality.WEIRDO:
                NbRandomElectricSounds = WeirdoNbRandomElectricSounds;
                break;
            default:
                break;
        }

        if (electricEvents.Count > 0)
        {
            foreach (EventInfo info in electricEvents)
            {
                info.TerminateSoundEvent();
            }
            electricEvents.Clear();
        }

        for (int i = 0; i < NbRandomElectricSounds; ++i)
        {
            List<ParamInfo> parameters = new List<ParamInfo>();
            parameters.Add(new ParamInfo(maskParameterName, isMask ? 1f : 0f));
            parameters.Add(new ParamInfo(robotParameterName, isRobot ? 1f : 0f));
            EventInfo newElecSound = new EventInfo("Electric_" + Character.GetPersonalityName(personality), GetPersonalityElectricEventPath(personality) + i.ToString(), parameters);
            newElecSound.InitSoundEvent();
            electricEvents.Add(newElecSound);
        }
    }

    public void PlayElectric()
    {
        if (currentElectricEvent != null)
            return;
        int i = Random.Range(0, electricEvents.Count);
        currentElectricEvent = electricEvents[i];
        currentElectricEvent.PlayEvent();
        Debug.Log("Play electric "+ i + " on " + currentElectricEvent.EventName);
    }

    public void StopElectric()
    {
        if (currentElectricEvent != null)
        {
            //Debug.Log("Stop electric " + currentElectricEvent.EventName);
            currentElectricEvent.StopEvent();
            currentElectricEvent = null;
        }
    }

    public void InitCharge(Character.ePersonality personality, bool isRobot, bool isMask)
    {
        int NbRandomChargeSounds = 0;

        switch (personality)
        {
            case Character.ePersonality.HAPPY:
                NbRandomChargeSounds = HappyNbRandomChargeSounds;
                break;
            case Character.ePersonality.GRUMPY:
                NbRandomChargeSounds = GrumpyNbRandomChargeSounds;
                break;
            case Character.ePersonality.SCARED:
                NbRandomChargeSounds = ScaredNbRandomChargeSounds;
                break;
            case Character.ePersonality.WEIRDO:
                NbRandomChargeSounds = WeirdoNbRandomChargeSounds;
                break;
            default:
                break;
        }

        if (chargeEvents.Count > 0)
        {
            foreach (EventInfo info in chargeEvents)
            {
                info.TerminateSoundEvent();
            }
            chargeEvents.Clear();
        }

        for (int i = 0; i < NbRandomChargeSounds; ++i)
        {
            List<ParamInfo> parameters = new List<ParamInfo>();
            parameters.Add(new ParamInfo(maskParameterName, isMask ? 1f : 0f));
            parameters.Add(new ParamInfo(robotParameterName, isRobot ? 1f : 0f));
            EventInfo newCharge = new EventInfo("Electric", GetPersonalityChargeEventPath(personality) + i.ToString(), parameters);
            newCharge.InitSoundEvent();
            chargeEvents.Add(newCharge);
        }
    }

    public void PlayCharge()
    {
        if (currentChargeEvent != null)
            return;
        currentChargeEvent = chargeEvents[Random.Range(0, chargeEvents.Count)];
        currentChargeEvent.PlayEvent();
    }

    public bool IsChargePlaying()
    {
        return currentChargeEvent != null && currentChargeEvent.IsPlaying();
    }

    public void StopCharge()
    {
        if (currentChargeEvent != null)
        {
            currentChargeEvent.StopEvent();
            currentChargeEvent = null;
        }
    }

    public void PauseSounds()
    {
        currentPersonalityEvent.PauseEvent();

        if (currentElectricEvent != null)
        {
            currentElectricEvent.PauseEvent();
        }

        if (currentChargeEvent != null)
        {
            currentChargeEvent.PauseEvent();
        }
    }

    public void UnPauseSounds()
    {
        currentPersonalityEvent.PlayEvent();

        if (currentElectricEvent != null)
        {
            currentElectricEvent.PlayEvent();
        }

        if (currentChargeEvent != null)
        {
            currentChargeEvent.PlayEvent();
        }
    }
}
