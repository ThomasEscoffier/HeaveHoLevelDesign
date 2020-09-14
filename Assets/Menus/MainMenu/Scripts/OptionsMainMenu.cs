using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;

public class OptionsMainMenu : MonoBehaviour
{
    public GameObject[] FirstSlider;
    public GameObject PreviousButtonMenu = null;
    public MenuScrollingButton ButtonOption = null;
    public MainButtonsPanel MainMenu = null;
    public Image Background = null;
    public GameObject ConfirmRoot = null;
    public GameObject ResetRoot = null;
    public GameObject Foreground = null;
    public Animator ArrowLeft = null;
    public Animator ArrowRight = null;
    public float RepeatDelay = 2f;
    public float DeadZone = 0.5f;
    public GameObject[] Pages;

    EventSystem eventSystem = null;
    SoundModule soundModule = null;

    bool isInOptions = false;

    int index = 0;
    float timer = 0f;

    void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        soundModule = GetComponent<SoundModule>();
    }

    void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, RepeatDelay);

        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if (!isInOptions)
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

            if (!isInOptions && playerInput.GetButtonDown(RewiredConsts.Action.Menu_Confirm))
            {
                isInOptions = true;
                Foreground.SetActive(false);
                ConfirmRoot.SetActive(false);
                ResetRoot.SetActive(true);
                Background.gameObject.SetActive(true);
                MainMenu.SetIsActivated(false);
                ButtonOption.IsStillSelected = true;
                eventSystem.SetSelectedGameObject(FirstSlider[index]);
            }
            else if (isInOptions && playerInput.GetButtonDown(RewiredConsts.Action.Menu_Cancel))
            {
                isInOptions = false;
                Foreground.SetActive(true);
                ConfirmRoot.SetActive(true);
                ResetRoot.SetActive(false);
                Background.gameObject.SetActive(false);
                MainMenu.SetIsActivated(true);
                ButtonOption.IsStillSelected = false;
                eventSystem.SetSelectedGameObject(PreviousButtonMenu);
            }
        }
    }

    void UpdateShowArrows()
    {
        ArrowLeft.gameObject.SetActive(index > 0);
        ArrowRight.gameObject.SetActive(index < Pages.Length - 1);
    }

    public bool GetIsInOption()
    {
        return isInOptions;
    }
}
