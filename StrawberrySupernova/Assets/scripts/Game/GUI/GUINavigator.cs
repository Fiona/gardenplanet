using System;
using System.Collections;
using System.Collections.Generic;
using StompyBlondie;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class GUINavigator : MonoBehaviour
    {
        public enum GUINavigatorDirection {
            Vertical, Horizontal
        };

        [Tooltip("If true, moving up on a selection of nav elements will bring the selector back round again.")]
        public bool allowWrapping = true;

        [Tooltip("Used to turn off and on, it's automatically switched off with linked navigators.")]
        public bool active = true;

        [Tooltip("Directional flow of navigation elements. Vertical elements will be switched between with up/down. " +
                 "Horizontal will be switch between with left/right.")]
        public GUINavigatorDirection direction = GUINavigatorDirection.Vertical;

        [Tooltip("If the user is on the last of the list of elements and they go back, the provided GUINavigator "+
                 "will be made active and this one will be made inactive.")]
        public GUINavigator previousLinkedNavigator = null;

        [Tooltip("If the user is on the end of the list of elements and they go forward, the provided GUINavigator "+
                 "will be made active and this one will be made inactive.")]
        public GUINavigator nextLinkedNavigator = null;

        [Tooltip("If true then switching beetwen links happens with pointing in the opposite directions to what is " +
                 "selected as the navigation direction for this navigator.")]
        public bool oppositeAxisLinking = false;

        [Tooltip("If true, when activated by a next navigator, the last element will be automatically focussed and vise-versa.")]
        public bool autoFlowFromLinkedNavigator = true;

        [Tooltip("How much of a lead time we have between switches so we don't immediately switch")]
        public float linkTime = .1f;

        [HideInInspector]
        private static float linkTimer = -1f;

        private List<RectTransform> navigationElements;
        private GameInputManager input;
        private int focussedNavigationElement;
        private int selectingNavigationElement;
        private int savedFocussedElement = -1;
        private RectTransform navPointer;
        private int cancelNavigationElement;
        private Action<RectTransform> focusCallback;

        public void AddNavigationElement(RectTransform element, bool isCancel = false)
        {
            navigationElements.Add(element);
            if(isCancel)
                cancelNavigationElement = navigationElements.Count - 1;
        }

        public void ClearNavigationElements()
        {
            navigationElements = new List<RectTransform>();
            focussedNavigationElement = -1;
            savedFocussedElement = -1;
            UnfocusAll();
        }

        public void StartActiveLinkFromNext()
        {
            active = true;
            if(autoFlowFromLinkedNavigator)
                FocusNavigationElement(navigationElements.Count-1);
            else
                FocusNavigationElement(savedFocussedElement > -1 ? savedFocussedElement : 0);
        }

        public void StartActiveLinkFromPrevious()
        {
            active = true;
            if(autoFlowFromLinkedNavigator)
                FocusNavigationElement(0);
            else
                FocusNavigationElement(savedFocussedElement > -1 ? savedFocussedElement : 0);
        }

        public void HideNavPointer()
        {
            navPointer.GetComponentInChildren<Image>().color = Color.clear;
        }

        public void SetFocusCallback(Action<RectTransform> callback)
        {
            focusCallback = callback;
        }

        private void Awake()
        {
            navigationElements = new List<RectTransform>();
            focussedNavigationElement = -1;
            selectingNavigationElement = -1;
            cancelNavigationElement = -1;
            input = FindObjectOfType<GameInputManager>();
            focusCallback = null;
        }

        private void Start()
        {
            navPointer = Instantiate(Resources.Load(Consts.PREFAB_PATH_GUI_NAVIGATION_POINTER) as GameObject)
                .GetComponent<RectTransform>();
            navPointer.SetParent(FindObjectOfType<Canvas>().transform);
            navPointer.transform.SetSiblingIndex(navPointer.transform.GetSiblingIndex() - 1);
            navPointer.localScale = Vector3.one;
            if(linkTimer < 0)
                linkTimer = Time.time;
            StartCoroutine(DoNavigation());
        }

        private void OnDisable()
        {
            if(navPointer != null)
                navPointer.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if(navPointer != null)
            {
                navPointer.gameObject.SetActive(true);
                StartCoroutine(DoNavigation());
            }
        }

        private void OnDestroy()
        {
            if(navPointer != null)
                Destroy(navPointer.gameObject);
        }

        private IEnumerator DoNavigation()
        {
            while(true)
            {
                // Do nothing if in mouse mode, no elements or inactive
                while(input.mouseMode || navigationElements.Count == 0 || !active)
                {
                    yield return StartCoroutine(UnfocusAll(keepFocus:true));
                    yield return new WaitForFixedUpdate();
                }
                navPointer.gameObject.SetActive(true);
                // Select first option when out of mouse mode with a small delay so other UI elements appear first
                if(focussedNavigationElement == -1)
                {
                    yield return new WaitForSeconds(.5f);
                    FocusNavigationElement(savedFocussedElement > -1 ? savedFocussedElement : 0);
                    savedFocussedElement = -1;
                }

                // Wait a bit if we just switched
                if(Time.time < linkTimer + linkTime)
                {
                    yield return new WaitForFixedUpdate();
                }

                // Moving down
                var nav = false;
                if(input.player.GetButtonRepeating(
                    (direction == GUINavigatorDirection.Horizontal ? "Menu Right" : "Menu Down")
                ))
                {
                    FocusNextNavigationElement();
                    nav = true;
                }

                // Moving up
                if(input.player.GetButtonRepeating(
                    (direction == GUINavigatorDirection.Horizontal ? "Menu Left" : "Menu Up")
                ))
                {
                    FocusPreviousNavigationElement();
                    nav = true;
                }

                // Selecting
                if(input.player.GetButtonDown("Confirm"))
                {
                    SelectDownCurrentNavigationElement();
                    nav = true;
                }

                if(input.player.GetButtonUp("Confirm"))
                {
                    SelectUpCurrentNavigationElement();
                    nav = true;
                }

                // Cancelling
                if(input.player.GetButtonUp("Cancel") && cancelNavigationElement != -1)
                {
                    ExecuteOn(cancelNavigationElement, ExecuteEvents.pointerEnterHandler);
                    ExecuteOn(cancelNavigationElement, ExecuteEvents.pointerClickHandler);
                    nav = true;
                }

                // Linking along opposite direction axes
                if(!nav && oppositeAxisLinking)
                {
                    // Going to previous link
                    if(input.player.GetButtonRepeating((direction == GUINavigatorDirection.Horizontal
                           ? "Menu Up"
                           : "Menu Left")) &&
                       previousLinkedNavigator != null)
                    {
                        linkTimer = Time.time;
                        previousLinkedNavigator.StartActiveLinkFromNext();
                        active = false;
                    }
                    // Going to next link
                    if(input.player.GetButtonRepeating((direction == GUINavigatorDirection.Horizontal
                           ? "Menu Down"
                           : "Menu Right")) &&
                       nextLinkedNavigator != null)
                    {
                        linkTimer = Time.time;
                        nextLinkedNavigator.StartActiveLinkFromPrevious();
                        active = false;
                    }
                }
                // Update pos
                SetPointerPosition();
                yield return new WaitForFixedUpdate();
            }
        }

        private void FocusNavigationElement(int elementNum)
        {
            // If this is the first element navigated to we should fade in the pointer
            if(focussedNavigationElement == -1)
                StartCoroutine(LerpHelper.QuickFadeIn(navPointer.gameObject.GetComponentInChildren<Image>(), .2f));

            // Set position of pointer
            focussedNavigationElement = elementNum;
            SetPointerPosition();

            // Send mouse enter event
            ExecuteOn(focussedNavigationElement, ExecuteEvents.pointerEnterHandler);

            // Callback
            if(focusCallback != null)
                focusCallback(navigationElements[focussedNavigationElement]);
        }

        private void UnfocusCurrent()
        {
            // Send mouse exit event
            if(focussedNavigationElement != -1)
                ExecuteOn(focussedNavigationElement, ExecuteEvents.pointerExitHandler);
        }

        private void SetPointerPosition()
        {
            if(focussedNavigationElement == -1)
                return;
            navPointer.position = navigationElements[focussedNavigationElement].position;
            navPointer.anchoredPosition -= new Vector2(navigationElements[focussedNavigationElement].sizeDelta.x/2, 0f);
        }

        private IEnumerator UnfocusAll(bool keepFocus = false)
        {
            // If nothing is focussed immediately hide the pointer
            var navPointerImage = navPointer.gameObject.GetComponentInChildren<Image>();
            if(focussedNavigationElement == -1)
            {
                navPointerImage.color = new Color(1f, 1f, 1f, 0f);
                yield break;
            }
            // Store what we're currently on if wanting to keep focus between activations
            if(keepFocus)
                savedFocussedElement = focussedNavigationElement;
            // Send mouse exit event
            ExecuteOn(focussedNavigationElement, ExecuteEvents.pointerExitHandler);
            focussedNavigationElement = -1;
            // Fade out nav pointer
            yield return LerpHelper.QuickFadeOut(navPointerImage, .2f);
        }

        private void FocusPreviousNavigationElement()
        {
            UnfocusCurrent();
            if(focussedNavigationElement == 0)
            {
                // Handle linking if links are on the same axis
                if(previousLinkedNavigator != null && !oppositeAxisLinking)
                {
                    linkTimer = Time.time;
                    previousLinkedNavigator.StartActiveLinkFromNext();
                    active = false;
                    return;
                }
                // Wrapping around elements
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
                // Handle linking if links are on the same axis
                if(nextLinkedNavigator != null && !oppositeAxisLinking)
                {
                    linkTimer = Time.time;
                    nextLinkedNavigator.StartActiveLinkFromPrevious();
                    active = false;
                    return;
                }
                // Wrapping around elements
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
            if(element == -1)
                return;
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(
                navigationElements[element].gameObject, pointer, eventHandler
            );
        }

    }
}