using UnityEngine;

public abstract class AAccessory : MonoBehaviour {

    protected SpriteRenderer sprite;
    protected Hand parentHand = null;
    protected HandTryOn uiParentHand = null;
    protected SoundModule soundModule = null;

    public virtual void Awake()
    {
        soundModule = GetComponent<SoundModule>();
    }

    public virtual void Update() { }
    public virtual void LateUpdate() { }

    public abstract void OnStart(Hand hand);
    public abstract void OnStart(HandTryOn hand); // To show in UI
    public abstract void OnUse();
    public abstract void OnStopUse();
    public abstract void OnStop();
}
