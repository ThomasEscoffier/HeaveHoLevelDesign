using UnityEngine;

public class TutorialKeyboardKey : MonoBehaviour
{
    public DisplayKeyboardKey AnimKeyUp = null;
    public DisplayKeyboardKey AnimKeyLeft = null;
    public DisplayKeyboardKey AnimKeyRight = null;
    public DisplayKeyboardKey AnimKeyDown = null;

    public DisplayKeyboardKey AnimKeyUpLeft = null;
    public DisplayKeyboardKey AnimKeyUpRight = null;

    public void SetIsHighlightUp()
    {
        AnimKeyUp.gameObject.SetActive(true);
        AnimKeyLeft.gameObject.SetActive(true);
        AnimKeyRight.gameObject.SetActive(true);
        AnimKeyDown.gameObject.SetActive(true);

        AnimKeyUpLeft.gameObject.SetActive(false);
        AnimKeyUpRight.gameObject.SetActive(false);

        AnimKeyUp.SetAnimBool(true);
        AnimKeyLeft.SetAnimBool(false);
        AnimKeyRight.SetAnimBool(false);
        AnimKeyDown.SetAnimBool(false);
    }

    public void SetIsHighlightLeft()
    {
        AnimKeyUp.gameObject.SetActive(true);
        AnimKeyLeft.gameObject.SetActive(true);
        AnimKeyRight.gameObject.SetActive(true);
        AnimKeyDown.gameObject.SetActive(true);

        AnimKeyUpLeft.gameObject.SetActive(false);
        AnimKeyUpRight.gameObject.SetActive(false);

        AnimKeyUp.SetAnimBool(false);
        AnimKeyLeft.SetAnimBool(true);
        AnimKeyRight.SetAnimBool(false);
        AnimKeyDown.SetAnimBool(false);
    }

    public void SetIsHighlightRight()
    {
        AnimKeyUp.gameObject.SetActive(true);
        AnimKeyLeft.gameObject.SetActive(true);
        AnimKeyRight.gameObject.SetActive(true);
        AnimKeyDown.gameObject.SetActive(true);

        AnimKeyUpLeft.gameObject.SetActive(false);
        AnimKeyUpRight.gameObject.SetActive(false);

        AnimKeyUp.SetAnimBool(false);
        AnimKeyLeft.SetAnimBool(false);
        AnimKeyRight.SetAnimBool(true);
        AnimKeyDown.SetAnimBool(false);
    }

    public void SetIsHighlightDown()
    {
        AnimKeyUp.gameObject.SetActive(true);
        AnimKeyLeft.gameObject.SetActive(true);
        AnimKeyRight.gameObject.SetActive(true);
        AnimKeyDown.gameObject.SetActive(true);

        AnimKeyUpLeft.gameObject.SetActive(false);
        AnimKeyUpRight.gameObject.SetActive(false);

        AnimKeyUp.SetAnimBool(false);
        AnimKeyLeft.SetAnimBool(false);
        AnimKeyRight.SetAnimBool(false);
        AnimKeyDown.SetAnimBool(true);
    }

    public void SetIsHighlightUpLeft()
    {
        AnimKeyUp.gameObject.SetActive(false);
        AnimKeyLeft.gameObject.SetActive(false);
        AnimKeyRight.gameObject.SetActive(false);
        AnimKeyDown.gameObject.SetActive(false);

        AnimKeyUpLeft.gameObject.SetActive(true);
        AnimKeyUpRight.gameObject.SetActive(true);

        AnimKeyUpLeft.SetAnimBool(true);
        AnimKeyUpRight.SetAnimBool(false);
    }

    public void SetIsHighlightUpRight()
    {
        AnimKeyUp.gameObject.SetActive(false);
        AnimKeyLeft.gameObject.SetActive(false);
        AnimKeyRight.gameObject.SetActive(false);
        AnimKeyDown.gameObject.SetActive(false);

        AnimKeyUpLeft.gameObject.SetActive(true);
        AnimKeyUpRight.gameObject.SetActive(true);

        AnimKeyUpLeft.SetAnimBool(false);
        AnimKeyUpRight.SetAnimBool(true);
    }
}
