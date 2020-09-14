using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class KeyboardMapper : MonoBehaviour
{
    public float InputTimeOut = 5f;
    public int MaxNbInputMappers = 2;

    KeyboardMapperRow[] rows;

    int currentIndex = 0;
    InputMapper[] inputMappers = new InputMapper[2];
    Rewired.Player keyboardPlayer = null;
    Controller controller = null;

    bool isListening = false;

    const string currentCategory = "Character";
    const string currentLayout = "AssistedControlLeft";

    void Awake()
    {
        rows = GetComponentsInChildren<KeyboardMapperRow>();
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (player.controllers.hasKeyboard)
            {
                keyboardPlayer = player;
            }
        }
        controller = keyboardPlayer.controllers.GetController(ControllerType.Keyboard, keyboardPlayer.controllers.Keyboard.id);

        for (int i = 0; i < inputMappers.Length; ++i)
        {
            inputMappers[i] = new InputMapper();
            inputMappers[i].options.allowButtonsOnFullAxisAssignment = false;

            inputMappers[i].options.timeout = InputTimeOut;
            inputMappers[i].options.ignoreMouseXAxis = true;
            inputMappers[i].options.ignoreMouseYAxis = true;

            inputMappers[i].InputMappedEvent += OnInputMapped;
            inputMappers[i].StoppedEvent += OnStopped;
            inputMappers[i].ConflictFoundEvent += OnConflictFound;
            inputMappers[i].CanceledEvent += OnCancelled;
        }

        LoadKeyboardMap();
    }

    void Update()
    {
        if (isListening)
            return;

        foreach (Rewired.Player playerInput in ReInput.players.AllPlayers)
        {
            if (playerInput.GetButtonUp(RewiredConsts.Action.Menu_Special))
            {
                ResetToDefault();
            }
        }
    }

    public void SetCurrentRowIndex(KeyboardMapperRow row)
    {
        for (int i = 0; i < rows.Length; ++i)
        {
            if (row == rows[i])
            {
                currentIndex = i;
            }
        }
    }

    public void StartListeningInput()
    {
        isListening = true;
        StartCoroutine(StartListening(rows[currentIndex]));
    }

    IEnumerator StartListening(KeyboardMapperRow row)
    {
        //Wait to avoid binding with Confirm button
        yield return new WaitForSeconds(0.1f);

        ControllerMap currentControllerMap = keyboardPlayer.controllers.maps.GetMap(controller.type, controller.id, currentCategory, currentLayout);

        for (int i = 0; i < rows[currentIndex].RewiredActionNames.Length; ++i)
        {
            inputMappers[i].Start(
                new InputMapper.Context()
                {
                    actionName = row.RewiredActionNames[i],
                    controllerMap = currentControllerMap,
                    actionRange = row.AxisRange,
                    actionElementMapToReplace = currentControllerMap.GetElementMap(GetElementIdFromAction(rows[currentIndex].RewiredActionNames[i]))
                }
            );
        }
        keyboardPlayer.controllers.maps.SetMapsEnabled(false, "Menu");
        row.SetTextKeyName(row.GetLocalization().LocalizationKey, true);
    }

    IEnumerator WaitBeforeListeningDefault()
    {
        //Wait to avoid reseting with Reset default button
        yield return new WaitForSeconds(0.5f);

        isListening = false;
    }

    public string GetElementNameFromAction(string actionName)
    {
        ControllerMap currentControllerMap = keyboardPlayer.controllers.maps.GetMap(controller.type, controller.id, currentCategory, currentLayout);
        foreach (var actionElementMap in currentControllerMap.ElementMapsWithAction(actionName))
        {
            if (actionElementMap.axisContribution.Equals(rows[currentIndex].ActionContribution))
            {
                return actionElementMap.elementIdentifierName;
            }
        }
        return "";
    }

    public string GetElementNameFromAction(KeyboardMapperRow row, string actionName)
    {
        ControllerMap currentControllerMap = keyboardPlayer.controllers.maps.GetMap(controller.type, controller.id, currentCategory, currentLayout);
        foreach (var actionElementMap in currentControllerMap.ElementMapsWithAction(actionName))
        {
            if (actionElementMap.axisContribution.Equals(row.ActionContribution))
            {
                return actionElementMap.elementIdentifierName;
            }
        }
        return "";
    }

    int GetElementIdFromAction(string actionName)
    {
        ControllerMap currentControllerMap = keyboardPlayer.controllers.maps.GetMap(controller.type, controller.id, currentCategory, currentLayout);
        foreach (var actionElementMap in currentControllerMap.ElementMapsWithAction(actionName))
        {
            if (actionElementMap.axisContribution.Equals(rows[currentIndex].ActionContribution))
            {
                return actionElementMap.id;
            }
        }
        return -1;
    }

    void OnInputMapped(InputMapper.InputMappedEventData data)
    {      
        rows[currentIndex].SetTextKeyName(GetElementNameFromAction(data.inputMapper.mappingContext.actionName), false);
        SaveKeyboardMap();
    }

    void OnStopped(InputMapper.StoppedEventData data)
    {
        keyboardPlayer.controllers.maps.SetMapsEnabled(true, "Menu");
        StartCoroutine(WaitBeforeListeningDefault());
    }

    void OnCancelled(InputMapper.CanceledEventData data)
    {
        rows[currentIndex].SetTextKeyName(GetElementNameFromAction(data.inputMapper.mappingContext.actionName), false);
        keyboardPlayer.controllers.maps.SetMapsEnabled(true, "Menu");
        StartCoroutine(WaitBeforeListeningDefault());
    }

    void OnConflictFound(InputMapper.ConflictFoundEventData data)
    {
        bool hasOtherConflict = false;
        foreach (var conflit in data.conflicts)
        {
            //Check if not same layout and if conflict with same input on the other arm
            if (data.inputMapper.mappingContext.controllerMap.layoutId.Equals(conflit.controllerMap.layoutId) && data.inputMapper.mappingContext.controllerMap.sourceMapId.Equals(conflit.controllerMap.sourceMapId)
                && !(data.inputMapper.mappingContext.actionElementMapToReplace.axisContribution.Equals(conflit.elementMap.axisContribution)
                && (data.inputMapper.mappingContext.actionName.Equals("LeftArmMovementX") && conflit.action.name.Equals("RightArmMovementX")
                || data.inputMapper.mappingContext.actionName.Equals("RightArmMovementX") && conflit.action.name.Equals("LeftArmMovementX")
                || data.inputMapper.mappingContext.actionName.Equals("LeftArmMovementY") && conflit.action.name.Equals("RightArmMovementY")
                || data.inputMapper.mappingContext.actionName.Equals("RightArmMovementY") && conflit.action.name.Equals("LeftArmMovementY")
                || data.inputMapper.mappingContext.actionName.Equals("PointLeft") && conflit.action.name.Equals("PointRight")
                || data.inputMapper.mappingContext.actionName.Equals("PointRight") && conflit.action.name.Equals("PointLeft"))))
            {
                hasOtherConflict = true;
            }
        }

        if (hasOtherConflict)
        {
            data.responseCallback(InputMapper.ConflictResponse.Cancel);
        }
        else
        {
            data.responseCallback(InputMapper.ConflictResponse.Add);
            SaveKeyboardMap();
        }
    }

    void SaveKeyboardMap()
    {
        List<string> xml = new List<string>();

        foreach (ControllerMapSaveData saveData in keyboardPlayer.GetSaveData(false).AllControllerMapSaveData)
        {
            xml.Add(saveData.map.ToXmlString());
        }
        GameManager.Instance.GetSaveManager().GameSaveData.KeyboardMappingXML = xml;
        GameManager.Instance.GetSaveManager().Save();
    }

    void LoadKeyboardMap()
    {
        if (GameManager.Instance.GetSaveManager().GameSaveData.KeyboardMappingXML.Count > 0)
        {
            keyboardPlayer.controllers.maps.AddMapsFromXml(ControllerType.Keyboard, controller.id, GameManager.Instance.GetSaveManager().GameSaveData.KeyboardMappingXML);
        }
    }

    void ResetToDefault()
    {
        GameManager.Instance.GetSaveManager().GameSaveData.KeyboardMappingXML = GameManager.Instance.GetSaveManager().GameSaveData.DefaultKeyboardMappingXML;

        LoadKeyboardMap();
        for (int i = 0; i < rows.Length; ++i)
        {
            rows[i].RefreshKeyName();
        }

        GameManager.Instance.GetSaveManager().Save();
    }

    //Used to get defaults when creating a new save
    public static List<string> GetKeyboardMapXML()
    {
        Rewired.Player keyboardPlayer = null;
        foreach (Rewired.Player player in ReInput.players.GetPlayers())
        {
            if (player.controllers.hasKeyboard)
            {
                keyboardPlayer = player;
            }
        }

        List<string> xml = new List<string>();

        foreach (ControllerMapSaveData saveData in keyboardPlayer.GetSaveData(false).AllControllerMapSaveData)
        {
            xml.Add(saveData.map.ToXmlString());
        }

        return xml;
    }
}
