using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Rewired;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GardenPlanet
{
    public class SettingsCategoryInput: SettingsCategory
    {
        [Header("Messages")]
        public string noJoystickMessage = "No attached devices detected.";
        public string joystickErrorMessage = "Error switching joystick.";

        [Multiline]
        public string rebindingPopupMessage = "Press the desired input for\n{0}";
        public string rebindingPopupTimerMessage = "Cancels in {0} seconds...";

        [Header("Keybinds holder top position")]
        public float keybindsHolderTop = 113f;
        public float keybindsHolderTopJoystick = 183f;

        [Header("Arrays of actions that are rebindable")]
        public string[] keyboardMouseActions;
        public string[] joystickActions;
        public string[] gameActions;
        public string[] menusActions;

        [Header("Rewired category and layout")]
        public string category = "Default";
        public string layoutGame = "Game";
        public string layoutMenus = "Menus";

        [Header("Input type buttons")]
        public GlobalSelectableButton keyboardTypeButton;
        public GlobalSelectableButton mouseTypeButton;
        public GlobalSelectableButton joystickTypeButton;

        [Header("Object references")]
        public GameObject keybindsHolder;

        [Header("Joystick selector references")]
        public GameObject joystickDeviceSelector;
        public GameObject joystickSwitcherButtons;
        public GlobalButton nextJoystickButton;
        public GlobalButton prevJoystickButton;
        public TextMeshProUGUI joystickNameText;

        [Header("References to keybinding scroll list")]
        public GameObject keybindSpacerTemplate;
        public SettingsInputKeybind settingsInputKeybindTemplate;
        public GameObject keybindList;
        public Scrollbar scrollbar;
        public RectTransform scrollWindow;

        [Header("References to rebinding waiting popup")]
        public GameObject rebindingPopup;
        public TextMeshProUGUI rebindingPopupText;
        public TextMeshProUGUI rebindingPopupTimerText;

        private GlobalSelectableButton currentTypeButton;
        private GUINavigator inputTypeNavigation;
        private GUINavigator joystickSwitcherNavigation;
        private GUINavigator keybindsNavigation;
        private ControllerType selectedControllerType;
        private int selectedControllerId = 0;

        private Rewired.Player player { get { return ReInput.players.GetPlayer(0); } }
        private ControllerMap gameControllerMap {
            get {
                if(controller == null) return null;
                return player.controllers.maps.GetMap(controller.type, controller.id, category, layoutGame);
            }
        }
        private ControllerMap menusControllerMap {
            get {
                if(controller == null) return null;
                return player.controllers.maps.GetMap(controller.type, controller.id, category, layoutMenus);
            }
        }
        private Controller controller { get { return player.controllers.GetController(selectedControllerType, selectedControllerId); } }

        private GameInputManager manager;
        private InputMapper inputMapper;

        public override void Init(GUINavigator catButtonNavigator)
        {
            base.Init(catButtonNavigator);

            // Set up navigator for input types
            inputTypeNavigation = gameObject.AddComponent<GUINavigator>();
            inputTypeNavigation.active = false;
            inputTypeNavigation.direction = GUINavigator.GUINavigatorDirection.Horizontal;
            inputTypeNavigation.oppositeAxisLinking = true;

            inputTypeNavigation.AddNavigationElement(keyboardTypeButton.rectTransform);
            inputTypeNavigation.AddNavigationElement(mouseTypeButton.rectTransform);
            inputTypeNavigation.AddNavigationElement(joystickTypeButton.rectTransform);

            // Set up navigator for joystik buttons
            joystickSwitcherNavigation = gameObject.AddComponent<GUINavigator>();
            joystickSwitcherNavigation.active = false;
            joystickSwitcherNavigation.direction = GUINavigator.GUINavigatorDirection.Horizontal;
            joystickSwitcherNavigation.oppositeAxisLinking = true;

            joystickSwitcherNavigation.AddNavigationElement(prevJoystickButton.rectTransform);
            joystickSwitcherNavigation.AddNavigationElement(nextJoystickButton.rectTransform);

            // Set up navigator for keybinds
            keybindsNavigation = gameObject.AddComponent<GUINavigator>();
            keybindsNavigation.active = false;
            keybindsNavigation.SetFocusCallback(KeybindsNavigationFocusCallback);
            keybindsNavigation.allowWrapping = false;

            // Set input type callbacks
            keyboardTypeButton.SetCallback(OpenKeyboardTypeInput);
            mouseTypeButton.SetCallback(OpenMouseTypeInput);
            joystickTypeButton.SetCallback(OpenJoystickTypeInput);

            // Set joystick switcher callbacks
            nextJoystickButton.SetCallback(SelectNextJoystick);
            prevJoystickButton.SetCallback(SelectPrevJoystick);

            // Set up rewired input mapper
            inputMapper = new InputMapper();
            inputMapper.options.timeout = 5f;
            inputMapper.options.ignoreMouseXAxis = true;
            inputMapper.options.ignoreMouseYAxis = true;
            inputMapper.options.defaultActionWhenConflictFound = InputMapper.ConflictResponse.Add;
            inputMapper.options.allowKeyboardKeysWithModifiers = false;
            inputMapper.options.allowKeyboardModifierKeyAsPrimary = true;
            inputMapper.InputMappedEvent += OnInputMapped;
            inputMapper.TimedOutEvent += OnTimeoutMap;

            manager = FindObjectOfType<GameInputManager>();

            keybindsHolder.SetActive(false);
        }

        public override IEnumerator Open()
        {
            categoryButtonNavigator.nextLinkedNavigator = inputTypeNavigation;
            inputTypeNavigation.previousLinkedNavigator = categoryButtonNavigator;
            SelectControllerType(ControllerType.Keyboard);

            // Listen to controllers being removed or added
            ReInput.ControllerConnectedEvent += OnControllerChanged;
            ReInput.ControllerDisconnectedEvent += OnControllerChanged;

            yield return base.Open();
        }

        public override IEnumerator Close()
        {
            categoryButtonNavigator.nextLinkedNavigator = null;
            inputTypeNavigation.active = false;
            joystickSwitcherNavigation.active = false;
            keybindsNavigation.active = false;

            // Not listening anymore
            ReInput.ControllerConnectedEvent -= OnControllerChanged;
            ReInput.ControllerDisconnectedEvent -= OnControllerChanged;

            yield return base.Close();
        }

        public override IEnumerator Save()
        {
            ReInput.userDataStore.Save();
            yield break;
        }

        private void Update()
        {
            if(rebindingPopup.activeSelf)
                rebindingPopupTimerText.text = String.Format(rebindingPopupTimerMessage, (int)inputMapper.timeRemaining);
        }

        private void OpenJoystickTypeInput()
        {
            SelectControllerType(ControllerType.Joystick);
        }

        private void OpenMouseTypeInput()
        {
            SelectControllerType(ControllerType.Mouse);
        }

        private void OpenKeyboardTypeInput()
        {
            SelectControllerType(ControllerType.Keyboard);
        }

        private void SelectControllerType(ControllerType controllerType)
        {
            if(currentTypeButton)
                currentTypeButton.Deselect();
            if(controllerType == ControllerType.Keyboard)
                currentTypeButton = keyboardTypeButton;
            else if(controllerType == ControllerType.Mouse)
                currentTypeButton = mouseTypeButton;
            else if(controllerType == ControllerType.Joystick)
                currentTypeButton = joystickTypeButton;
            currentTypeButton.Select();

            keybindsHolder.SetActive(true);
            selectedControllerId = 0;
            selectedControllerType = controllerType;

            var r = keybindsHolder.GetComponent<RectTransform>();
            r.offsetMax = new Vector2(
                r.offsetMax.x,
                -(selectedControllerType == ControllerType.Joystick ? keybindsHolderTopJoystick : keybindsHolderTop)
            );

            inputTypeNavigation.nextLinkedNavigator = keybindsNavigation;
            keybindsNavigation.previousLinkedNavigator = inputTypeNavigation;

            // Joysticks have a different interface and might not be attached
            if(selectedControllerType == ControllerType.Joystick)
            {
                joystickDeviceSelector.SetActive(true);
                // If there are no joysticks we hide stuff and display a message
                if(controller == null)
                {
                    joystickSwitcherButtons.SetActive(false);
                    joystickNameText.text = noJoystickMessage;
                    keybindsHolder.SetActive(false);
                    return;
                }

                joystickSwitcherButtons.SetActive(player.controllers.joystickCount > 1);

                // Set up navigation linkers for the addition of buttons
                if(player.controllers.joystickCount > 1)
                {
                    inputTypeNavigation.nextLinkedNavigator = joystickSwitcherNavigation;
                    joystickSwitcherNavigation.previousLinkedNavigator = inputTypeNavigation;

                    joystickSwitcherNavigation.nextLinkedNavigator = keybindsNavigation;
                    keybindsNavigation.previousLinkedNavigator = joystickSwitcherNavigation;
                }

                SelectJoystick(selectedControllerId);
                return;
            }

            joystickDeviceSelector.SetActive(false);
            RecreateKeybindsList();
        }

        private void SelectJoystick(int controllerID)
        {
            selectedControllerId = controllerID;
            if(controller == null)
            {
                joystickSwitcherButtons.SetActive(false);
                joystickNameText.text = joystickErrorMessage;
                keybindsHolder.SetActive(false);
                return;
            }

            joystickNameText.text = controller.name;
            RecreateKeybindsList();
        }

        private void RecreateKeybindsList(bool resetScrollbar = true)
        {
            keybindSpacerTemplate.SetActive(false);
            settingsInputKeybindTemplate.gameObject.SetActive(false);
            keybindList.DestroyAllChildren();
            keybindsNavigation.ClearNavigationElements();

            if(selectedControllerType == ControllerType.Joystick)
                AddActionArrayToKeybinds(joystickActions, gameControllerMap);
            else
                AddActionArrayToKeybinds(keyboardMouseActions, gameControllerMap);
            AddSpacer();
            AddActionArrayToKeybinds(gameActions, gameControllerMap);
            AddSpacer();
            AddActionArrayToKeybinds(menusActions, menusControllerMap);

            if(resetScrollbar)
            {
                ScrollbarHelper.SetupScrollbar(scrollbar, keybindList, scrollWindow);
                ScrollbarHelper.SetScrollbarTo(scrollbar, 0f);
            }
        }

        private void AddSpacer()
        {
            var space = Instantiate(keybindSpacerTemplate);
            space.gameObject.SetActive(true);
            space.transform.SetParent(keybindList.transform, false);
            space.transform.SetAsLastSibling();
        }

        private void AddActionArrayToKeybinds(string[] actions, ControllerMap controllerMap)
        {
            foreach(var actionName in actions)
            {
                // Spacer
                if(actionName == "-")
                {
                    AddSpacer();
                    continue;
                }

                var action = ReInput.mapping.GetAction(actionName);
                if(action == null)
                    continue;

                if(action.type == InputActionType.Axis)
                {
                    // Joysticks get one button for the full axis otherwise there are two buttons
                    if(selectedControllerType == ControllerType.Joystick)
                        AddKeybindRow(action.descriptiveName, action, controllerMap, AxisRange.Full);
                    else
                    {
                        AddKeybindRow(action.negativeDescriptiveName, action, controllerMap, AxisRange.Negative);
                        AddKeybindRow(action.positiveDescriptiveName, action, controllerMap, AxisRange.Positive);
                    }
                    continue;
                }

                if(action.type == InputActionType.Button)
                    AddKeybindRow(action.descriptiveName, action, controllerMap, AxisRange.Positive);
            }
        }

        private SettingsInputKeybind AddKeybindRow(string descriptiveName, InputAction action,
            ControllerMap controllerMap, AxisRange axisRange)
        {
            // Create row
            var newRow = Instantiate(settingsInputKeybindTemplate);
            newRow.gameObject.SetActive(true);
            newRow.transform.SetParent(keybindList.transform, false);
            newRow.transform.SetAsLastSibling();
            keybindsNavigation.AddNavigationElement(newRow.button.rectTransform);

            // set up visuals
            var actionElementMap = GetElementMapForAction(action, controllerMap, axisRange);
            newRow.text.text = descriptiveName;
            newRow.buttonText.text = (actionElementMap == null ? String.Empty : actionElementMap.elementIdentifierName);
            var actionElementMapId = (actionElementMap == null ? -1 : actionElementMap.id);

            // Set up callback
            newRow.button.SetCallback(
                () => { OnKeybindButtonPressed(descriptiveName, controllerMap, action, axisRange, actionElementMapId); }
            );

            return newRow;
        }

        private ActionElementMap GetElementMapForAction(InputAction action, ControllerMap controllerMap,
            AxisRange axisRange = AxisRange.Positive)
        {
            // Find the first ActionElementMap that maps to this Action and is compatible with this field type
            foreach(var actionElementMap in controllerMap.ElementMapsWithAction(action.id))
                if(actionElementMap.ShowInField(axisRange))
                    return actionElementMap;
            return null;
        }

        private void SelectNextJoystick()
        {
            if(selectedControllerId >= player.controllers.joystickCount - 1)
            {
                SelectJoystick(0);
                return;
            }
            SelectJoystick(selectedControllerId+1);
        }

        private void SelectPrevJoystick()
        {
            if(selectedControllerId == 0)
            {
                SelectJoystick(player.controllers.joystickCount - 1);
                return;
            }
            SelectJoystick(selectedControllerId-1);
        }

        private void OnControllerChanged(ControllerStatusChangedEventArgs args)
        {
            if(selectedControllerType == ControllerType.Joystick)
                SelectControllerType(selectedControllerType);
        }

        private void KeybindsNavigationFocusCallback(RectTransform button)
        {
            ScrollbarHelper.ScrollbarAutoFocusToElement(scrollbar, keybindList, scrollWindow,
                button.parent.GetComponent<RectTransform>());
        }

        private void OnInputMapped(InputMapper.InputMappedEventData data)
        {
            RecreateKeybindsList(false);
            rebindingPopup.SetActive(false);
            keybindsNavigation.active = true;
            if(manager != null)
                manager.doingRebind = false;
        }

        private void OnTimeoutMap(InputMapper.TimedOutEventData eventData)
        {
            RecreateKeybindsList(false);
            rebindingPopup.SetActive(false);
            keybindsNavigation.active = true;
            if(manager != null)
                manager.doingRebind = false;
        }

        private void OnKeybindButtonPressed(string descirptiveName, ControllerMap controllerMap, InputAction action,
            AxisRange axisRange, int existingActionElementMapId)
        {
            if(manager != null)
                manager.doingRebind = true;

            keybindsNavigation.active = false;
            rebindingPopup.SetActive(true);
            rebindingPopupText.text = String.Format(rebindingPopupMessage, descirptiveName);

            inputMapper.Start(
                new InputMapper.Context() {
                    actionId = action.id,
                    controllerMap = controllerMap,
                    actionRange = axisRange,
                    actionElementMapToReplace = controllerMap.GetElementMap(existingActionElementMapId)
                }
            );
        }
    }
}