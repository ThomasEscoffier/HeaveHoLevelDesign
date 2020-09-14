using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using Rewired.Integration.UnityUI;

public class ArrowSelection : Selectable {

    [System.Serializable]
    public class ArrowSelectionEvent : UnityEvent<ArrowSelection> {}

    public ArrowSelectionEvent UpdateOptionEvent;

    EventSystem eventSystem = null;
    RewiredStandaloneInputModule inputModule = null;
    SoundModule soundModule = null;
    Localization localization = null;
    Text text = null;

    public Text OptionText;

    public Color TextColorSelected;
    public Color TextColorNormal;
    public Color TextColorDisabled;

    public Image Background = null;
    public bool IsChangeColorBackground = false;
    public Color BackgroundNormal;
    public Color BackgroundSelected;
    public Color BackgroundDisabled;

    bool isSelected = false;
    bool isDisabled = false;

    Animator anim = null;

    int playerId = 0;
    Rewired.Player PlayerInput = null;

    public bool DisplayCurrentOption = false;
    public bool IsLocalized = true;

    public UnityEvent OnSelected = new UnityEvent();

    [Range(0, 1)]
    public float Sensitivity = 0.5f;

    bool isLeft = false;
    int index = 0;
    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }
    public List<string> Options = new List<string>();

    float timer = 0f;

    protected override void Awake()
    {
        base.Awake();

        soundModule = GetComponentInChildren<SoundModule>();
        anim = Background.GetComponent<Animator>();
        if (IsChangeColorBackground)
        {
            Background.enabled = true;
            anim.enabled = false;
            Background.color = BackgroundNormal;
        }
        else
        {
            Background.enabled = false;
        }
        OptionText.color = TextColorNormal;
        localization = OptionText.GetComponent<Localization>();
        text = OptionText.GetComponent<Text>();
        
        if (!IsLocalized)
        {
            localization.enabled = false;
        }
    }

    void Update()
    {
        if (isDisabled)
            return;

        if (eventSystem && eventSystem.currentSelectedGameObject == gameObject)
        {
            if (!isSelected)
            {
                if (IsChangeColorBackground)
                {
                    Background.color = BackgroundSelected;
                    anim.enabled = true;
                }
                else
                {
                    Background.enabled = true;
                }
                OptionText.color = TextColorSelected;
                isSelected = true;
                soundModule.PlayOneShot("ArrowVertical");
                OnSelected.Invoke();
            }

            if (PlayerInput == null)
            {
                foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
                {
                    UpdateInput(playerInput);
                }
            }
            else
            {
                UpdateInput(PlayerInput);
            }
        }
        else
        {
            if (isSelected)
            {
                if (IsChangeColorBackground)
                {
                    Background.color = BackgroundNormal;
                    anim.enabled = false;
                }
                else
                {
                    Background.enabled = false;
                }
                OptionText.color = TextColorNormal;
                isSelected = false;
            }
        }

        if (inputModule)
        {
            timer = Mathf.Min(timer + Time.unscaledDeltaTime, inputModule.repeatDelay);
        }
    }

    void UpdateInput(Rewired.Player playerInput)
    {
        if (timer >= inputModule.repeatDelay && playerInput.GetAxis(inputModule.horizontalAxis) > Sensitivity)
        {
            isLeft = true;
            index++;
            if (index >= Options.Count)
            {
                index = 0;
            }
            RefreshLocalization();
            UpdateOptionEvent.Invoke(this);
            if (soundModule.DoesEventExist("Arrow"))
            {
                soundModule.PlayOneShot("Arrow");
            }
            timer = 0f;
        }
        if (timer >= inputModule.repeatDelay && playerInput.GetAxis(inputModule.horizontalAxis) < -Sensitivity)
        {
            isLeft = false;
            index--;
            if (index < 0)
            {
                index = Options.Count - 1;
            }
            RefreshLocalization();
            UpdateOptionEvent.Invoke(this);
            if (soundModule.DoesEventExist("Arrow"))
            {
                soundModule.PlayOneShot("Arrow");
            }
            timer = 0f;
        }
    }

    public void RefreshLocalization()
    {
        if (DisplayCurrentOption)
        {
            if (IsLocalized)
            {
                localization.SetText(Options[index]);
            }
            else
            {
                text.text = Options[index];
            }
        }
    }

    public void SetCurrentEventSystem(EventSystem evtSystem, RewiredStandaloneInputModule input)
    {
        eventSystem = evtSystem;
        inputModule = input;
        timer = inputModule.repeatDelay;
    }

    public virtual void ListenPlayerId(int id)
    {
        playerId = id;
        PlayerInput = ReInput.players.GetPlayer(playerId);
    }

    public virtual int GetPlayerId()
    {
        return playerId;
    }

    public void AddOption(string option)
    {
        Options.Add(option);
        RefreshLocalization();
    }

    public void AddOptions(List<string> options)
    {
        Options.AddRange(options);
        RefreshLocalization();
    }

    public void ClearOptions()
    {
        Options.Clear();
    }

    public bool GetIsLastCommandDirectionLeft()
    {
        return isLeft;
    }

    public void SetIsDisabled(bool state)
    {
        isDisabled = state;
        interactable = !state;
        if (isDisabled)
        {
            OptionText.color = TextColorDisabled;
            if (IsChangeColorBackground)
            {
                Background.color = BackgroundDisabled;
                anim.enabled = false;
            }
        }
        else
        {
            OptionText.color = TextColorNormal;
            if (IsChangeColorBackground)
            {
                Background.color = BackgroundNormal;
                anim.enabled = true;
            }
        }
    }
}
