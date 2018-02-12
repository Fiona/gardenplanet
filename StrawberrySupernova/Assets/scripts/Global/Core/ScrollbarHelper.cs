using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace StompyBlondie
{
    public static class ScrollbarHelper
    {
        // Sets the display and content size of the passed scrollbar to match the content.
        public static void SetupScrollbar(Scrollbar scrollbar, GameObject content, RectTransform contentWindow)
        {
            if(content.transform.childCount == 0)
            {
                scrollbar.gameObject.SetActive(false);
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

            // Get content and scroll window heights
            var heightOfScrollWindow = contentWindow.rect.height;
            var heightOfContent = content.GetComponent<RectTransform>().sizeDelta.y;

            // No need for scroll bar if content inside scroll
            if(heightOfContent < heightOfScrollWindow)
            {
                scrollbar.gameObject.SetActive(false);
                return;
            }

            // Set size of scroll bar handle
            var scrollbarOverflow = heightOfContent - heightOfScrollWindow;
            scrollbar.size = 1f - (scrollbarOverflow / heightOfScrollWindow);
            if(scrollbar.size < .1f)
                scrollbar.size = .1f;

            // Set callback for setting value of scroll bar
            scrollbar.onValueChanged.AddListener((val) =>
            {
                var _heightOfScrollWindow = contentWindow.rect.height;
                var _heightOfContent = content.GetComponent<RectTransform>().sizeDelta.y;
                var overflow = Math.Abs(_heightOfContent - _heightOfScrollWindow);
                content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, overflow * val);
            });
        }

        public static void SetScrollbarTo(Scrollbar scrollbar, float value)
        {
            scrollbar.StartCoroutine(
                LerpHelper.QuickTween(
                    (float val) => { scrollbar.value = val; },
                    scrollbar.value,
                    value,
                    .2f,
                    lerpType:LerpHelper.Type.Exponential
                )
            );
        }

    }
}