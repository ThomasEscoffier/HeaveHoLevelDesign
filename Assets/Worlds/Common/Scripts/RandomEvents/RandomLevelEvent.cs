using UnityEngine;

public class RandomLevelEvent : MonoBehaviour
{
    public bool HappensOnlyOne = true;

    protected bool isPlaying = false;

    public virtual void Start()
    {
        Init();
    }

    public virtual void Init() { }

    public virtual void Play()
    {
        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying)
        {
            if (GetIsFinished())
            {
                Finish();
            }
            else
            {
                UpdatePlaying();
            }
        }
    }

    public virtual void UpdatePlaying() { }

    public virtual bool GetIsFinished() { return true; }

    public virtual void Pause()
    {
        isPlaying = false;
    }

    public virtual void UnPause()
    {
        isPlaying = true;
    }

    public virtual void Finish()
    {
        isPlaying = false;
    }

    public bool GetIsPlaying()
    {
        return isPlaying;
    }
}