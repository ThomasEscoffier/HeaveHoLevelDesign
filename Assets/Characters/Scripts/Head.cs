using UnityEngine;

public class Head : MonoBehaviour
{
    float startAmp;
    float endAmp;

    float startFreq;
    float endFreq;

    float timer = 0f;
    float timeBetweenValues = 0f;

    bool isShaking = false;
    float timerShakeFrequence = 0f;
    bool isShakeLeft = false;
    Vector3 previousPos = Vector3.zero;
    Vector3 previousShake = Vector3.zero;

    void Update()
    {
        if (isShaking)
        {
            timer = Mathf.Min(timer + Time.deltaTime, timeBetweenValues);
            float currentAmp = Mathf.Lerp(startAmp, endAmp, timer / timeBetweenValues);
            float currentFreq = Mathf.Lerp(startFreq, endFreq, timer / timeBetweenValues);

            timerShakeFrequence = Mathf.Min(timerShakeFrequence + Time.deltaTime, currentFreq);
            transform.localPosition = new Vector3(isShakeLeft ? previousPos.x - currentAmp : previousPos.x + currentAmp, previousPos.y, previousPos.z);

            //Next shake
            if (timerShakeFrequence == currentFreq)
            {
                isShakeLeft = !isShakeLeft;
                timerShakeFrequence = 0f;
            }
        }
    }

    public void StartShake(float startShakeFrenquency, float maxShakeFrenquency, float startShakeAmplitude, float maxShakeAmplitude, float timeTransition)
    {
        if (isShaking)
            return;

        previousPos = transform.localPosition;
        previousShake = previousPos;
        startFreq = startShakeFrenquency;
        endFreq = maxShakeFrenquency;

        startAmp = startShakeAmplitude;
        endAmp = maxShakeAmplitude;

        timer = 0f;
        timeBetweenValues = timeTransition;

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

    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = previousPos;
    }
}
