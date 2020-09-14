using System.Collections.Generic;
using UnityEngine;

public class RandomLevelEventsManager : MonoBehaviour {

    public List<RandomLevelEvent> RandomLevelEvents = new List<RandomLevelEvent>();
    public bool isPlayingRandomly = true;
    public float TimeMax = 2f;
    public float TimeMin = 1f;
    float timeBeforePlaying = 0f;
    float timer = 0f;

    int currentIndex = -1;
    int oldIndex = -1;
    bool isStopped = false;

    void Start ()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.OnLevelFinished.AddListener(StopAllEvents);
        ResetRandom();
    }

	void Update ()
    {
        if (RandomLevelEvents.Count == 0 || isStopped)
            return;

        if (oldIndex == -1 || !RandomLevelEvents[oldIndex].GetIsPlaying())
        {
            if (oldIndex != -1 && RandomLevelEvents[oldIndex].HappensOnlyOne)
            {
                RandomLevelEvents.Remove(RandomLevelEvents[oldIndex]);
                if (oldIndex < currentIndex)
                {
                    currentIndex--;
                }
                else if (oldIndex == currentIndex)
                {
                    ResetRandom();
                }
                oldIndex = -1;
            }
            timer = Mathf.Min(timer + Time.deltaTime, timeBeforePlaying);
            if (timer >= timeBeforePlaying)
            {
                PlayNextEvent();
            }
        }
    }

    void ResetRandom()
    {
        timeBeforePlaying = Random.Range(TimeMin, TimeMax);
        timer = 0f;
        oldIndex = currentIndex;
        if (isPlayingRandomly)
        {
            currentIndex = Random.Range(0, RandomLevelEvents.Count);
        }
        else
        {
            currentIndex++;
            if (currentIndex > RandomLevelEvents.Count)
            {
                currentIndex = 0;
            }
        }
    }

    public void StopAllEvents()
    {
        isStopped = true;
        if (RandomLevelEvents.Count == 0)
            return;

        foreach (var levelEvent in RandomLevelEvents)
        {
            if (levelEvent != null)
            {
                levelEvent.Finish();
            }
        }
    }

    public void PlayNextEvent()
    {
        bool isValidEvent = true;
        var levelEvent = RandomLevelEvents[currentIndex];
        if (levelEvent != null)
        {
            levelEvent.Init();
            levelEvent.Play();
        }
        else
        {
            isValidEvent = false;
        }
        ResetRandom();
        if (!isValidEvent)
        {
            oldIndex = -1;
        }
    }
}
