using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using StompyBlondie;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class GUINavigator: MonoBehaviour
    {
        [Tooltip("If true, moving up on a selection of nav elements will bring the selector back round again.")]
        public bool allowWrapping = true;

        private List<RectTransform> navigationElements;
        private GameController controller;
        private int focussedNavigationElement;
        private int selectingNavigationElement;
        private RectTransform navPointer;
        private int cancelNavigationElement;

        public void AddNavigationElement(RectTransform element, bool isCancel = false)
        {
            navigationElements.Add(element);
            if(isCancel)
                cancelNavigationElement = navigationElements.Count - 1;
        }

        public void HideNavPointer()
        {
            navPointer.GetComponentInChildren<Image>().color = Color.clear;
        }

        private void Awake()
        {
            navigationElements = new List<RectTransform>();
            focussedNavigationElement = -1;
            selectingNavigationElement = -1;
            cancelNavigationElement = -1;
            controller = FindObjectOfType<GameController>();
        }

        private void Start()
        {
            navPointer = Instantiate(Resources.Load(Consts.PREFAB_PATH_GUI_NAVIGATION_POINTER) as GameObject)
                .GetComponent<RectTransform>();
            navPointer.SetParent(controller.canvasRect.transform);
            navPointer.localScale = Vector3.one;
            StartCoroutine(DoNavigation());
        }

        private void OnDestroy()
        {
            Destroy(navPointer.gameObject);
        }

        private IEnumerator DoNavigation()
        {
            while(true)
            {
                // Do nothing if in mouse mode or no elements
                while(controller.GameInputManager.mouseMode || navigationElements.Count == 0)
                {
                    yield return StartCoroutine(UnfocusAll());
                    yield return new WaitForFixedUpdate();
                }
                navPointer.gameObject.SetActive(true);
                // Select first option when out of mouse mode with a small delay so other UI elements appear first
                if(focussedNavigationElement == -1)
                {
                    yield return new WaitForSeconds(.5f);
                    FocusNavigationElement(0);
                }
                // Moving down
                if(controller.GameInputManager.player.GetButtonDown("Menu Down"))
                    FocusNextNavigationElement();
                // Moving up
                if(controller.GameInputManager.player.GetButtonDown("Menu Up"))
                    FocusPreviousNavigationElement();
                // Selecting
                if(controller.GameInputManager.player.GetButtonDown("Confirm"))
                    SelectDownCurrentNavigationElement();
                if(controller.GameInputManager.player.GetButtonUp("Confirm"))
                    SelectUpCurrentNavigationElement();
                // Cancelling
                if(controller.GameInputManager.player.GetButtonUp("Cancel") && cancelNavigationElement != -1)
                {
                    ExecuteOn(cancelNavigationElement, ExecuteEvents.pointerEnterHandler);
                    ExecuteOn(cancelNavigationElement, ExecuteEvents.pointerClickHandler);
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void FocusNavigationElement(int elementNum)
        {
            // If this is the first element navigated to we should fade in the pointer
            if(focussedNavigationElement == -1)
                StartCoroutine(LerpHelper.QuickFadeIn(navPointer.gameObject.GetComponentInChildren<Image>(), .5f));

            // Set position of pointer
            focussedNavigationElement = elementNum;
            navPointer.position = navigationElements[elementNum].position;
            navPointer.anchoredPosition -= new Vector2(navigationElements[elementNum].sizeDelta.x/2, 0f);

            // Send mouse enter event
            ExecuteOn(focussedNavigationElement, ExecuteEvents.pointerEnterHandler);
        }

        private void UnfocusCurrent()
        {
            // Send mouse exit event
            if(focussedNavigationElement != -1)
                ExecuteOn(focussedNavigationElement, ExecuteEvents.pointerExitHandler);
        }

        private IEnumerator UnfocusAll()
        {
            // If nothing is focussed immediately hide the pointer
            var navPointerImage = navPointer.gameObject.GetComponentInChildren<Image>();
            if(focussedNavigationElement == -1)
            {
                navPointerImage.color = new Color(1f, 1f, 1f, 0f);
                yield break;
            }
            // Send mouse exit event
            ExecuteOn(focussedNavigationElement, ExecuteEvents.pointerExitHandler);
            focussedNavigationElement = -1;
            // Fade out nav pointer
            yield return LerpHelper.QuickFadeOut(navPointerImage, .5f);
        }

        private void FocusPreviousNavigationElement()
        {
            UnfocusCurrent();
            if(focussedNavigationElement == 0)
            {
                if(allowWrapping)
                    FocusNavigationElement(navigationElements.Count-1);
                return;
            }
            FocusNavigationElement(focussedNavigationElement-1);
        }

        private void FocusNextNavigationElement()
        {
            UnfocusCurrent();
            if(focussedNavigationElement == navigationElements.Count - 1)
            {
                if(allowWrapping)
                    FocusNavigationElement(0);
                return;
            }
            FocusNavigationElement(focussedNavigationElement+1);
        }

        private void SelectDownCurrentNavigationElement()
        {
            if(focussedNavigationElement == -1)
                return;
            selectingNavigationElement = focussedNavigationElement;
            // Send mouse down event
            ExecuteOn(focussedNavigationElement, ExecuteEvents.pointerDownHandler);
        }

        private void SelectUpCurrentNavigationElement()
        {
            if(selectingNavigationElement == -1)
                return;
            // Send mouse up event
            ExecuteOn(selectingNavigationElement, ExecuteEvents.pointerUpHandler);

            // if this is the focussed one then we clicked it
            if(selectingNavigationElement == focussedNavigationElement)
                ExecuteOn(selectingNavigationElement, ExecuteEvents.pointerClickHandler);

            selectingNavigationElement = -1;
        }

        private void ExecuteOn<T>(int element, ExecuteEvents.EventFunction<T> eventHandler) where T: IEventSystemHandler
        {
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(
                navigationElements[element].gameObject, pointer, eventHandler
            );
        }

    }
}