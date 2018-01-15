using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using StompyBlondie;

namespace StrawberryNova
{
    public class InGameMenu: MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Menu layout of the pages, each line is the name of a page, which corresponds to a child page. A hypen signifies a spacer")]
        [Multiline(10)]
        public string pageLayout;

        [Tooltip("Which page to apen first")]
        public string defaultPage = "Inventory";

        [Header("Object references")]
        public InGameMenuTabs tabs;

        private bool closeMenu;
        private List<KeyValuePair<string, IInGameMenuPage>> pages;
        private IInGameMenuPage currentPage;
        private IEnumerator currentPageCoroutine;
        private bool firstPage;

        public void Awake()
        {
            // Create pages
            pages = new List<KeyValuePair<string, IInGameMenuPage>>();

            foreach(string line in pageLayout.Split('\n'))
            {
                if(line == "-")
                {
                    pages.Add(new KeyValuePair<string, IInGameMenuPage>("-", null));
                    continue;
                }

                var pageType = Type.GetType(typeof(InGameMenu).Namespace+".InGameMenuPage"+line);
                if(pageType == null)
                {
                    Debug.Log("Can't find class of type: InGameMenuPage"+line);
                    continue;
                }

                var childPage = GetComponentInChildren(pageType, true) as IInGameMenuPage;
                if(childPage == null)
                {
                    Debug.Log("Can't find object of type in children: InGameMenuPage"+line);
                    continue;
                }
                pages.Add(new KeyValuePair<string, IInGameMenuPage>(line, childPage));
            }

            // Create tabs from these
            tabs.CreateTabsFromPages(pages);
        }

        public IEnumerator OpenMenu()
        {
            // Fade in tabs
            yield return StartCoroutine(LerpHelper.QuickFadeIn(GetComponent<CanvasGroup>(), Consts.GUI_IN_GAME_MENU_FADE_TIME));

            closeMenu = false;
            firstPage = true;
            currentPageCoroutine = OpenPage(defaultPage);
            StartCoroutine(currentPageCoroutine);
            yield return new WaitUntil(() => closeMenu);

            // Fade out tabs
            yield return StartCoroutine(LerpHelper.QuickFadeOut(GetComponent<CanvasGroup>(), Consts.GUI_IN_GAME_MENU_FADE_TIME));
        }

        public IEnumerator OpenPage(string name)
        {
            var pageRequested = GetPageByName(name);
            if(pageRequested == null)
            {
                Debug.Log("In-game menu page not found: " + name);
                yield break;
            }

            if(pageRequested == currentPage)
                yield break;

            // If a page is already open, close it early
            if(!firstPage)
            {
                yield return StartCoroutine(currentPage.Close());
                StopCoroutine(currentPageCoroutine);
            }

            firstPage = false;
            currentPage = pageRequested;

            // Do opening animation
            yield return StartCoroutine(currentPage.Open());

            // Wait for the player to trigger closing the menu
            while(!closeMenu)
            {
                var manager = FindObjectOfType<GameInputManager>();
                if(manager.player.GetButtonDown("Open Menu") || manager.player.GetButtonDown("Cancel"))
                {
                    closeMenu = true;
                    break;
                }
                yield return new WaitForFixedUpdate();
            }

            // Do close animation
            yield return StartCoroutine(currentPage.Close());
        }

        private IInGameMenuPage GetPageByName(string name)
        {
            foreach(var kvp in pages)
                if(kvp.Key == name)
                    return kvp.Value;
            return null;
        }
    }
}