using Rewired;

public class PlayerControls {

    public enum eLayout
    {
        DEFAULT,
        ASSISTED,
        CUSTOM,
    }

    public enum eControlsMode
    {
        CONTROLLER,
        KEYBOARD,
    }

    eLayout currentLayout = eLayout.DEFAULT;
    eControlsMode currentMode = eControlsMode.CONTROLLER;

    public int playerId = -1;
    Rewired.Player PlayerInput = null;
    Rewired.PlayerMouse MouseInput = null;

    public int GetPlayerId()
    {
        return playerId;
    }

    public eLayout GetCurrentLayout()
    {
        return currentLayout;
    }

    public void SetCurrentLayout(eLayout newLayout)
    {
        currentLayout = newLayout;
    }

    public eControlsMode GetControlsMode()
    {
        return currentMode;
    }

    public void SetControlsMode(eControlsMode mode)
    {
        currentMode = mode;
    }

    public void SetMapEnabled(bool state, string mapName)
    {
        var maps = ReInput.players.GetPlayer(playerId).controllers.maps;
        switch (currentLayout)
        {
            default:
            case eLayout.DEFAULT:
                maps.SetMapsEnabled(!state, mapName, "AssistedControlLeft");
                maps.SetMapsEnabled(state, mapName, "DefaultControl");
                break;
            case eLayout.ASSISTED:
                maps.SetMapsEnabled(!state, mapName, "DefaultControl");
                maps.SetMapsEnabled(state, mapName, "AssistedControlLeft");
                break;
            case eLayout.CUSTOM:
                break;
        }
    }

    public void ListenPlayerInput(int playerId)
    {
        this.playerId = playerId;
        //if (currentMode == eControlsMode.MOUSE)
        //{
            MouseInput = PlayerMouse.Factory.Create();
        MouseInput.playerId = playerId;
        //}
        //else
        //{
            PlayerInput = ReInput.players.GetPlayer(playerId);
        //}
    }

    public Rewired.Player GetPlayerInput()
    {
        return PlayerInput;
    }

    public int GetNbJoysticks()
    {
        return PlayerInput.controllers.joystickCount;
    }

    public Rewired.PlayerMouse GetPlayerMouseInput()
    {
        return MouseInput;
    }
}
