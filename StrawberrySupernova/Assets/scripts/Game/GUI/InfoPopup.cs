﻿using System;
using StompyBlondie;
using TMPro;
using UnityEngine;

namespace StrawberryNova
{
    public class InfoPopup: MonoBehaviour
    {
        public InfoPopupDisplay displayRight;
        public InfoPopupDisplay displayLeft;
        public InfoPopupDisplay displayStatic;

        private GameController controller;
        private bool showingThisFrame;
        private bool currentlyShowing;
        private Vector3 targetPos;

        public void Awake()
        {
            controller = FindObjectOfType<GameController>();
        }

        public void LateUpdate()
        {
            if(Consts.PLAYER_SETTING_STATIC_INFO_POPUP)
                DoStaticDisplay();
            else
                DoTargetedDisplay();

            // Override to be off if input disabled
            if(!controller.GameInputManager.directInputEnabled)
                showingThisFrame = false;

            // Do fade in and out
            var setAlphaMethod = new Action<float>(
                (val) =>
                {
                    displayLeft.canvasGroup.alpha = val;
                    displayRight.canvasGroup.alpha = val;
                    displayStatic.canvasGroup.alpha = val;
                }
            );

            if(showingThisFrame && !currentlyShowing)
            {
                StartCoroutine(LerpHelper.QuickTween(setAlphaMethod, 0f, 1f, .4f));
                currentlyShowing = true;
            }

            if(!showingThisFrame && currentlyShowing)
            {
                StartCoroutine(LerpHelper.QuickTween(setAlphaMethod, 1f, 0f, .4f));
                currentlyShowing = false;
            }

            showingThisFrame = false;
        }

        public void Show(TilePosition tilePos, string textToShow, string extraTextToShow = "")
        {
            Show(new WorldPosition(tilePos), textToShow, extraTextToShow);
        }

        public void Show(WorldPosition worldPos, string textToShow, string extraTextToShow = "")
        {
            displayLeft.SetText(textToShow, extraTextToShow);
            displayRight.SetText(textToShow, extraTextToShow);
            displayStatic.SetText(textToShow, extraTextToShow);
            targetPos = worldPos.TransformPosition();
            showingThisFrame = true;
        }

        private void DoStaticDisplay()
        {
            transform.localPosition = Vector3.zero;
            displayLeft.gameObject.SetActive(false);
            displayRight.gameObject.SetActive(false);
            displayStatic.gameObject.SetActive(true);
        }

        private void DoTargetedDisplay()
        {
            // place the info box
            var halfCanvasSize = controller.canvasRect.sizeDelta / 2;
            var viewportPos = Camera.main.WorldToViewportPoint(targetPos);
            var proportionalPosition = new Vector2(
                viewportPos.x * controller.canvasRect.sizeDelta.x,
                viewportPos.y * controller.canvasRect.sizeDelta.y
            );
            transform.localPosition = proportionalPosition - halfCanvasSize;

            // determine direction of popup by checking which screen quadant it's in and displaying the correct one
            int screenQuad;
            if(transform.localPosition.x < 0 && transform.localPosition.y < 0)
                screenQuad = 0;
            else if(transform.localPosition.x < 0 && transform.localPosition.y > 0)
                screenQuad = 1;
            else if(transform.localPosition.x > 0 && transform.localPosition.y > 0)
                screenQuad = 2;
            else
                screenQuad = 3;

            RectTransform displayTransform = null;

            if(screenQuad == 0 || screenQuad == 1)
                displayTransform = displayRight.rectTransform;
            else
                displayTransform = displayLeft.rectTransform;

            if(displayTransform == displayLeft.rectTransform)
            {
                displayLeft.gameObject.SetActive(true);
                displayRight.gameObject.SetActive(false);
            }
            else
            {
                displayLeft.gameObject.SetActive(false);
                displayRight.gameObject.SetActive(true);
            }

            displayStatic.gameObject.SetActive(false);

            // Determine if the popup is off screen, if so it should be hidden
            var screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            var objectCorners = new Vector3[4];
            displayTransform.GetWorldCorners(objectCorners);
            var tempScreenSpaceCorner = controller.canvasRect.GetComponent<Canvas>().worldCamera.WorldToScreenPoint(objectCorners[screenQuad]);
            if(!screenBounds.Contains(tempScreenSpaceCorner))
                showingThisFrame = false;
        }

    }
}