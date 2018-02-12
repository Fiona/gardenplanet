using System;
using System.Collections;
using Rewired;
using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace StrawberryNova
{
    /*

     [ Keyboard ] [ Mouse ] > [ Joystick ]

            <   Joystick 1   >

     -------------------------------------
     | Move Vertical [ Left Stick X ]    |
     | Move Horizontal [ Left Stick Y ]  |
     |          ++                       |
     | Etc [ Etc ]                       |
     -------------------------------------

            [ Reset to defaults ]

     ---------------------

     These are the inputs I want to be able to rebind

== Keyboard and Mouse Only ==
Move Left
Move Right
Move Up
Move Down

Look Left
Look Right
Look Up
Look Down

== Joystick Only ==

Move Vertical
Move Horizontal

Look Vertical
Look Horizontal

== All ==

Use Item
Use Object
Drop Item
Previous Hotbar Item
Next Hotbar Item

Direction Lock

Confirm
Cancel
Menu Up
Menu Down
Menu Left
Menu Right
Next Page
Previous Page

Open Menu
     */

    public class SettingsCategoryInput: SettingsCategory
    {

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

        [Header("References to keybinding scroll list")]
        public GameObject keybindSpacerTemplate;
        public SettingsInputKeybind settingsInputKeybindTemplate;
        public GameObject keybindList;
        public Scrollbar scrollbar;
        public RectTransform scrollWindow;

        private GlobalSelectableButton currentTypeButton;
        private GUINavigator inputTypeNavigation;
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

            // Set input type callbacks
            keyboardTypeButton.SetCallback(OpenKeyboardTypeInput);
            mouseTypeButton.SetCallback(OpenMouseTypeInput);
            joystickTypeButton.SetCallback(OpenJoystickTypeInput);

            keybindsHolder.SetActive(false);
        }

        public override IEnumerator Open()
        {
            categoryButtonNavigator.nextLinkedNavigator = inputTypeNavigation;
            inputTypeNavigation.previousLinkedNavigator = categoryButtonNavigator;
            SelectControllerType(ControllerType.Keyboard);
            yield return base.Open();
        }

        public override IEnumerator Close()
        {
            categoryButtonNavigator.nextLinkedNavigator = null;
            yield return base.Close();
        }

        private void Update()
        {

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
            selectedControllerType = controllerType;
            RecreateKeybindsList();
        }

        private void RecreateKeybindsList()
        {
            keybindSpacerTemplate.SetActive(false);
            settingsInputKeybindTemplate.gameObject.SetActive(false);
            keybindList.DestroyAllChildren();

            if(selectedControllerType == ControllerType.Joystick)
                AddActionArrayToKeybinds(joystickActions, gameControllerMap);
            else
                AddActionArrayToKeybinds(keyboardMouseActions, gameControllerMap);
            AddSpacer();
            AddActionArrayToKeybinds(gameActions, gameControllerMap);
            AddSpacer();
            AddActionArrayToKeybinds(menusActions, menusControllerMap);

            ScrollbarHelper.SetupScrollbar(scrollbar, keybindList, scrollWindow);
            ScrollbarHelper.SetScrollbarTo(scrollbar, 0f);
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
                    {
                        var newRow = AddKeybindRow();
                        newRow.text.text = action.descriptiveName;
                        newRow.buttonText.text = GetCurrentBindNameForAction(action, controllerMap, AxisRange.Full);
                    }
                    else
                    {
                        var negRow = AddKeybindRow();
                        negRow.text.text = action.negativeDescriptiveName;
                        negRow.buttonText.text = GetCurrentBindNameForAction(action, controllerMap, AxisRange.Negative);
                        var posRow = AddKeybindRow();
                        posRow.text.text = action.positiveDescriptiveName;
                        posRow.buttonText.text = GetCurrentBindNameForAction(action, controllerMap, AxisRange.Positive);
                    }
                    continue;
                }

                if(action.type == InputActionType.Button)
                {
                    var newRow = AddKeybindRow();
                    newRow.text.text = action.descriptiveName;
                    newRow.buttonText.text = GetCurrentBindNameForAction(action, controllerMap);
                }
            }
        }

        private SettingsInputKeybind AddKeybindRow()
        {
            var newRow = Instantiate(settingsInputKeybindTemplate);
            newRow.gameObject.SetActive(true);
            newRow.transform.SetParent(keybindList.transform, false);
            newRow.transform.SetAsLastSibling();
            return newRow;
        }

        private string GetCurrentBindNameForAction(InputAction action, ControllerMap controllerMap,
            AxisRange axisRange = AxisRange.Positive)
        {
            // Find the first ActionElementMap that maps to this Action and is compatible with this field type
            foreach(var actionElementMap in controllerMap.ElementMapsWithAction(action.id))
                if(actionElementMap.ShowInField(axisRange))
                    return actionElementMap.elementIdentifierName;
            return String.Empty;
        }
    }
}