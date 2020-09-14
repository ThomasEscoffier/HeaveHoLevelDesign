using UnityEngine;
using Rewired;

public class CreditsMainMenu : MonoBehaviour
{
    public GameObject[] Pages;
    public Animator ArrowLeft = null;
    public Animator ArrowRight = null;
    public float RepeatDelay = 0.2f;
    public float DeadZone = 0.5f;

    float timer = 0f;
    int index = 0;

    SoundModule soundModule = null;

    void Awake()
    {
        soundModule = GetComponent<SoundModule>();
        UpdateShowArrows();
    }

    void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, RepeatDelay);

        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if (index > 0 && timer == RepeatDelay && playerInput.GetAxis("NavHorizontal") < -DeadZone)
            {
                Pages[index].SetActive(false);

                index--;
                UpdateShowArrows();
                ArrowLeft.SetTrigger("Activate");
                soundModule.PlayOneShot("Arrow");

                Pages[index].SetActive(true);
                timer = 0f;
            }
            else if (index < (Pages.Length - 1) && timer == RepeatDelay && playerInput.GetAxis("NavHorizontal") > DeadZone)
            {
                Pages[index].SetActive(false);

                index++;
                UpdateShowArrows();
                ArrowRight.SetTrigger("Activate");
                soundModule.PlayOneShot("Arrow");

                Pages[index].SetActive(true);
                timer = 0f;
            }
        }
    }

    void UpdateShowArrows()
    {
        ArrowLeft.gameObject.SetActive(index > 0);
        ArrowRight.gameObject.SetActive(index < Pages.Length - 1);
    }
}
