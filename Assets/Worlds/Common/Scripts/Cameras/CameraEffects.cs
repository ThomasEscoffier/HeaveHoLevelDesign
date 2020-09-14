using UnityEngine;
using UnityEngine.Events;

public class CameraEffects : MonoBehaviour {

    public UnityEvent EndLerpEvent;

    Camera cam = null;
    Animator anim = null;

    bool isLerping = false;
    Vector3 startPos;
    Vector3 endPos;
    float startSizeCam;
    float endSizeCam;
    float timeZoom = 0.5f;
    float timerZoom = 0f;

    [Tooltip("Lerp progression value : time between 0 and 1")]
    public AnimationCurve ZoomCurve;

    Vector3 initPos;
    float initSize;

    //SHAKE : To set with function
    float timeShakeFrequence = 0.1f;
    float timerShaking = 0f;
    AnimationCurve amplitudeShaking;
    //

    float timeShake = 0f;

    bool useTimeScale = true;

    bool isShaking = false;
    bool isShakingVertical = false;
    bool isShakingHorizontal = false;
    bool isSmoothShaking = false;
    float timerShakeFrequence = 0f;
    bool isShakeLeft = false;
    Vector3 previousPos = Vector3.zero;
    Vector3 previousShake = Vector3.zero;

    void Awake()
    {
        cam = GetComponent<Camera>();
        anim = GetComponent<Animator>();
        initPos = transform.position;
        initSize = cam.orthographicSize;
    }

	void Update ()
    {
        //SHAKE
        if (isShaking)
        {
            timerShakeFrequence = Mathf.Min(timerShakeFrequence + Time.deltaTime, timeShakeFrequence);
            if (isSmoothShaking)
            {
                float x = isShakingHorizontal ? Mathf.Lerp(previousShake.x, isShakeLeft ? previousPos.x - amplitudeShaking.Evaluate(timerShaking) : previousPos.x + amplitudeShaking.Evaluate(timerShaking), timerShakeFrequence / timeShakeFrequence) : transform.localPosition.x;
                float y = isShakingVertical ? Mathf.Lerp(previousShake.y, isShakeLeft ? previousPos.y - amplitudeShaking.Evaluate(timerShaking) : previousPos.y + amplitudeShaking.Evaluate(timerShaking), timerShakeFrequence / timeShakeFrequence) : transform.localPosition.y;
                transform.localPosition = new Vector3(x, y, transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(isShakeLeft ? previousPos.x - amplitudeShaking.Evaluate(timerShaking) : previousPos.x + amplitudeShaking.Evaluate(timerShaking), previousPos.y, previousPos.z);
            }

            //Next shake
            if (timerShakeFrequence == timeShakeFrequence)
            {
                isShakeLeft = !isShakeLeft;
                timerShakeFrequence = 0f;
            }

            //End Shake
            timerShaking = Mathf.Min(timerShaking + Time.deltaTime, timeShake);
            if (timerShaking == timeShake)
            {
                isShaking = false;
                transform.localPosition = previousPos;
            }
        }

        //LERP
        if (isLerping && timeZoom > 0f)
        {
            timerZoom = Mathf.Min(useTimeScale ? timerZoom + Time.deltaTime : timerZoom + Time.unscaledDeltaTime, timeZoom);
            transform.position = Vector3.Lerp(startPos, endPos, ZoomCurve.Evaluate(timerZoom));
            cam.orthographicSize = Mathf.Lerp(startSizeCam, endSizeCam, ZoomCurve.Evaluate(timerZoom));
            if (timerZoom == timeZoom)
            {
                timerZoom = 0f;
                isLerping = false;
                previousPos = transform.localPosition;
                EndLerpEvent.Invoke();
            }
        }
	}

    //Uses World pos
    public void StartLerp(Vector2 nextPos, float newSizeCam, float zoomTime, bool isTimeScaled = true)
    {
        if (anim != null)
        {
            anim.enabled = false;
        }

        useTimeScale = isTimeScaled;

        timeZoom = zoomTime;

        startPos = transform.position;
        endPos = new Vector3(nextPos.x, nextPos.y, transform.position.z);

        startSizeCam = cam.orthographicSize;
        endSizeCam = newSizeCam;

        isLerping = true;
    }

    //Uses World pos
    public void InstantZoom(Vector2 nextPos, float newSizeCam)
    {
        transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
        cam.orthographicSize = newSizeCam;
    }

    public void Reset()
    {
        StartLerp(initPos, initSize, timeZoom);
    }

    //Uses local pos
    public void Shake(float shakeTime, float shakeFrenquency, AnimationCurve shakeAmplitude, bool isSmooth = false, bool isHorizontal = true, bool isVertical = false)
    {
        if (isShaking)
            return;

        previousPos = transform.localPosition;
        previousShake = previousPos;
        timeShake = shakeTime;
        timerShaking = 0f;
        timeShakeFrequence = shakeFrenquency;

        amplitudeShaking = shakeAmplitude;

        isSmoothShaking = isSmooth;
        isShakingHorizontal = isHorizontal;
        isShakingVertical = isVertical;

        float randomValue = Random.value;
        if (randomValue < 0.5f)
        {
            isShakeLeft = true;
        }
        else
        {
            isShakeLeft = false;
        }

        isShaking = true;
    }
}
