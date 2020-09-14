using UnityEngine;
using Rewired;
using Rewired.Data.Mapping;

public class ControllerTypesManager : MonoBehaviour
{
    public enum eControllerType
    {
        NONE,
        XBOX360,
        XBOXONE,
        PS4,
        BITPRO8,
        JOYCON_LEFT,
        JOYCON_RIGHT,
        JOYCON_DUAL,
        JOYCON_PRO,
        JOYCON_HANDHELD,
        KEYBOARD,
    }

    [System.Serializable]
    public struct ControllerType
    {
        public eControllerType Type;
        public HardwareJoystickMap JoystickMap;
    }

    public ControllerType[] ControllerTypes;

    public eControllerType GetControllerTypeFromJoystick(Joystick joystick)
    {
        for (int i = 0; i < ControllerTypes.Length; ++i)
        {
            if (ControllerTypes[i].JoystickMap.Guid == joystick.hardwareTypeGuid)
            {
                return ControllerTypes[i].Type;
            }
        }
        return eControllerType.NONE;
    }

    public bool GetIsControllerNintendoFromJoystick(Joystick joystick)
    {
        for (int i = 0; i < ControllerTypes.Length; ++i)
        {
            if ((ControllerTypes[i].Type == eControllerType.JOYCON_DUAL
                || ControllerTypes[i].Type == eControllerType.JOYCON_HANDHELD
                || ControllerTypes[i].Type == eControllerType.JOYCON_LEFT
                || ControllerTypes[i].Type == eControllerType.JOYCON_PRO
                || ControllerTypes[i].Type == eControllerType.JOYCON_RIGHT)
                && ControllerTypes[i].JoystickMap.Guid == joystick.hardwareTypeGuid)
            {
                return true;
            }
        }
        return false;
    }
}
