using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class UITutorialLevels : MonoBehaviour
{
    public Animator MicrosoftControllerMove;
    public Animator SonyControllerMove;
    public Animator NintendoControllerMove;
    public Animator KeyboardControllerMove;

    public Animator MicrosoftControllerGrab;
    public Animator SonyControllerGrab;
    public Animator NintendoControllerGrab;
    public Animator KeyboardControllerGrab;

    public Text MoveText;
    public Text GrabText;

    void Awake()
    {
        SetAnimController();
    }

    void SetAnimController()
    {
        ControllerTypesManager.eControllerType type = ControllerTypesManager.eControllerType.KEYBOARD;
        Rewired.Player player = ReInput.players.GetPlayer(GameManager.Instance.GetPlayers()[0].GetControls().GetPlayerId());
        if (player.controllers.joystickCount > 0)
        {
            type = GameManager.Instance.GetControllerTypesManager().GetControllerTypeFromJoystick(player.controllers.Joysticks[0] as Joystick);
        }
        switch (type)
        {
            case ControllerTypesManager.eControllerType.KEYBOARD:
                {
                    MoveText.enabled = false;
                    GrabText.enabled = false;

                    MicrosoftControllerMove.gameObject.SetActive(false);
                    SonyControllerMove.gameObject.SetActive(false);
                    NintendoControllerMove.gameObject.SetActive(false);
                    KeyboardControllerMove.gameObject.SetActive(true);
                    KeyboardControllerMove.SetTrigger("Joystick");

                    MicrosoftControllerGrab.gameObject.SetActive(false);
                    SonyControllerGrab.gameObject.SetActive(false);
                    NintendoControllerGrab.gameObject.SetActive(false);
                    KeyboardControllerGrab.gameObject.SetActive(true);
                    KeyboardControllerGrab.SetTrigger("Grab");
                    break;
                }
            case ControllerTypesManager.eControllerType.XBOX360:
            case ControllerTypesManager.eControllerType.XBOXONE:
                {
                    MicrosoftControllerMove.gameObject.SetActive(true);
                    MicrosoftControllerMove.SetTrigger("Joystick");
                    SonyControllerMove.gameObject.SetActive(false);
                    NintendoControllerMove.gameObject.SetActive(false);
                    KeyboardControllerMove.gameObject.SetActive(false);

                    MicrosoftControllerGrab.gameObject.SetActive(true);
                    MicrosoftControllerGrab.SetTrigger("Grab");
                    SonyControllerGrab.gameObject.SetActive(false);
                    NintendoControllerGrab.gameObject.SetActive(false);
                    KeyboardControllerGrab.gameObject.SetActive(false);
                    break;
                }
            case ControllerTypesManager.eControllerType.JOYCON_DUAL:
            case ControllerTypesManager.eControllerType.JOYCON_LEFT:
            case ControllerTypesManager.eControllerType.JOYCON_RIGHT:
            case ControllerTypesManager.eControllerType.JOYCON_PRO:
            case ControllerTypesManager.eControllerType.JOYCON_HANDHELD:
                {
                    MicrosoftControllerMove.gameObject.SetActive(false);
                    SonyControllerMove.gameObject.SetActive(false);
                    NintendoControllerMove.gameObject.SetActive(true);
                    NintendoControllerMove.SetTrigger("Joystick");
                    KeyboardControllerMove.gameObject.SetActive(false);

                    MicrosoftControllerGrab.gameObject.SetActive(false);
                    SonyControllerGrab.gameObject.SetActive(false);
                    NintendoControllerGrab.gameObject.SetActive(true);
                    NintendoControllerGrab.SetTrigger("Grab");
                    KeyboardControllerGrab.gameObject.SetActive(false);
                    break;
                }
            case ControllerTypesManager.eControllerType.BITPRO8:
            case ControllerTypesManager.eControllerType.PS4:
            default:
                {
                    MicrosoftControllerMove.gameObject.SetActive(false);
                    SonyControllerMove.gameObject.SetActive(true);
                    SonyControllerMove.SetTrigger("Joystick");
                    NintendoControllerMove.gameObject.SetActive(false);
                    KeyboardControllerMove.gameObject.SetActive(false);

                    MicrosoftControllerGrab.gameObject.SetActive(false);
                    SonyControllerGrab.gameObject.SetActive(true);
                    SonyControllerGrab.SetTrigger("Grab");
                    NintendoControllerGrab.gameObject.SetActive(false);
                    KeyboardControllerGrab.gameObject.SetActive(false);
                    break;
                }
        }
    }
}
