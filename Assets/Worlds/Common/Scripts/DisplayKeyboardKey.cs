using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class DisplayKeyboardKey : MonoBehaviour
{
    public string RewiredActionName;
    public string CategoryName = "Character";
    public Pole ActionContribution = Pole.Positive;
    public bool AdaptKeySize = true;

    public GameObject Normal;
    public GameObject Tab;
    public GameObject Backspace;
    public GameObject Return;
    public GameObject Spacebar;

    Text keyText = null;
    Animator keyAnim = null;
    Rewired.Player keyboardPlayer = null;
    ControllerMap currentControllerMap = null;

    const string currentLayout = "AssistedControlLeft";

    void Awake()
    {
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (player.controllers.hasKeyboard)
            {
                keyboardPlayer = player;
            }
        }
        currentControllerMap = keyboardPlayer.controllers.maps.GetMap(ControllerType.Keyboard, keyboardPlayer.controllers.Keyboard.id, CategoryName, currentLayout);
        
        string keyName = GetElementNameFromAction(RewiredActionName);

        if (AdaptKeySize)
        {
            ActivateCorrespondingKeyShape(keyName);
        }
        else
        {
            SetTextFromActivatedKey();
        }
        
        keyText.text = keyName;
    }

    public string GetElementNameFromAction(string actionName)
    {
        foreach (var actionElementMap in currentControllerMap.ElementMapsWithAction(actionName))
        {
            if (actionElementMap.axisContribution.Equals(ActionContribution))
            {
                return actionElementMap.elementIdentifierName;
            }
        }
        return "";
    }

    public void SetAnimBool(bool state)
    {
        keyAnim.SetBool("IsHighlight", state);
    }

    void ActivateCorrespondingKeyShape(string keyName)
    {
        Normal.SetActive(false);
        switch (keyName)
        {
            case "Tab":
                {
                    Tab.SetActive(true);
                    keyAnim = Tab.GetComponent<Animator>();
                    keyText = Tab.transform.GetChild(0).GetComponent<Text>();
                    break;
                }
            case "Backspace":
                {
                    Backspace.SetActive(true);
                    keyAnim = Backspace.GetComponent<Animator>();
                    keyText = Backspace.transform.GetChild(1).GetComponent<Text>();
                    break;
                }
            case "Return":
                {
                    Return.SetActive(true);
                    keyAnim = Return.GetComponent<Animator>();
                    keyText = Return.transform.GetChild(0).GetComponent<Text>();
                    break;
                }
            case "Space":
                {
                    Spacebar.SetActive(true);
                    keyAnim = Spacebar.GetComponent<Animator>();
                    keyText = Spacebar.transform.GetChild(0).GetComponent<Text>();
                    break;
                }
            default:
                {
                    Normal.SetActive(true);
                    keyAnim = Normal.GetComponent<Animator>();
                    keyText = Normal.transform.GetChild(0).GetComponent<Text>();
                    break;
                }
        }
    }

    void SetTextFromActivatedKey()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                keyAnim = transform.GetChild(i).GetComponent<Animator>();
                if (i == 2)
                {
                    keyText = transform.GetChild(i).GetChild(1).GetComponent<Text>();
                }
                else
                {
                    keyText = transform.GetChild(i).GetChild(0).GetComponent<Text>();
                }
            }
        }
    }
}
