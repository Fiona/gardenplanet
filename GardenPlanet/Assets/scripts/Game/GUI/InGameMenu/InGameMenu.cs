using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StompyBlondie;
using UnityEngine.Timeline;
using StompyBlondie.Utils;

namespace GardenPlanet
{
    public class InGameMenu : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Menu layout of the pages, each line is the name of a page, which corresponds to a child page. A hypen signifies a spacer")]
        [Multiline(10)]
        public string pageLayout;

        [Tooltip("Which page to apen first")]
        public string defaultPage = "Inventory";

        [Tooltip("Which page shows the hotbar")]
        public string[] hotbarPages = {"CloseMenu", "Inventory"};

        [Header("Object references")]
        public InGameMenuTabs tabs;
        public GameObject pageHolder;

        private bool closeMenu;
        private List<KeyValuePair<string, IInGameMenuPage>> pages;
        private IInGameMenuPage currentPage;
        private string currentPageName;
        private IEnumerator currentPageCoroutine;
        private bool firstPage;
        private ItemHotbar itemHotbar;

        public void Awake()
        {
            itemHotbar = FindObjectOfType<ItemHotbar>();

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

                var prefab = Resources.Load(Consts.IN_GAME_MENU_PAGE_PREFAB_PATH + line);
                if(prefab == null)
                {
                    Debug.Log("Can't find resource prefab for page: "+line);
                    continue;
                }

                var newPageObj = (Instantiate(prefab) as GameObject);
                newPageObj.transform.SetParent(pageHolder.transform, false);
                newPageObj.SetActive(false);
                var childPage = newPageObj.GetComponent(pageType) as IInGameMenuPage;
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
            // Hide hotbar if necessary
            if(!hotbarPages.Contains(defaultPage))
                itemHotbar.LeaveScreen();

            // Fade in tabs
            yield return LerpHelper.QuickFadeIn(GetComponent<CanvasGroup>(),
                Consts.GUI_IN_GAME_MENU_FADE_TIME, LerpHelper.Type.SmoothStep);

            closeMenu = false;
            firstPage = true;
            currentPageCoroutine = OpenPage(defaultPage);
            StartCoroutine(currentPageCoroutine);
            yield return new WaitUntil(() => closeMenu);

            // Fade out tabs
            tabs.HideTabs();
            yield return LerpHelper.QuickFadeOut(GetComponent<CanvasGroup>(),
                Consts.GUI_IN_GAME_MENU_FADE_TIME, LerpHelper.Type.SmoothStep);

            if(!hotbarPages.Contains(currentPageName))
                itemHotbar.EnterScreen();
        }

        public void DoPageOpen(string name)
        {
            // If a page is already open, close it early
            if(!firstPage)
            {
                tabs.DeselectTab();
                StartCoroutine(currentPage.Close());
                StopCoroutine(currentPageCoroutine);
            }
            currentPageCoroutine = OpenPage(name);
            StartCoroutine(currentPageCoroutine);
        }

        public void CloseMenu()
        {
            closeMenu = true;
        }

        private IEnumerator OpenPage(string name)
        {
            var pageRequested = GetPageByName(name);
            if(pageRequested == null)
            {
                Debug.Log("In-game menu page not found: " + name);
                yield break;
            }

            if(pageRequested == currentPage)
                yield break;

            if(!firstPage)
            {
                if(hotbarPages.Contains(currentPageName) && !hotbarPages.Contains(name))
                    itemHotbar.LeaveScreen();
                if(hotbarPages.Contains(name) && !hotbarPages.Contains(currentPageName))
                    itemHotbar.EnterScreen();
            }

            firstPage = false;
            currentPage = pageRequested;
            currentPageName = name;

            // Do opening animation
            tabs.SelectTab(name);
            yield return currentPage.Open();

            // Wait for the player to trigger closing the menu
            var manager = FindObjectOfType<GameInputManager>();
            while(!closeMenu)
            {
                while(manager.doingRebind)
                    yield return new WaitForSeconds(.5f);

                if(manager.player.GetButtonUp("Open Menu") || manager.player.GetButtonUp("Cancel"))
                {
                    closeMenu = true;
                    break;
                }
                if(manager.player.GetButtonUp("Next Page"))
                {
                    DoPageOpen(GetNextPageName());
                    yield break;
                }
                if(manager.player.GetButtonUp("Previous Page"))
                {
                    DoPageOpen(GetPreviousPageName());
                    yield break;
                }
                currentPage.Input(manager);
                yield return new WaitForFixedUpdate();
            }

            // Do close animation
            yield return currentPage.Close();
        }

        private IInGameMenuPage GetPageByName(string name)
        {
            foreach(var kvp in pages)
                if(kvp.Key == name)
                    return kvp.Value;
            return null;
        }

        private string GetNextPageName()
        {
            var copyList = GetRealPages();
            return NextPageInList(copyList);
        }

        private string GetPreviousPageName()
        {
            var copyList = GetRealPages();
            copyList.Reverse();
            return NextPageInList(copyList);
        }

        private string NextPageInList(List<KeyValuePair<string, IInGameMenuPage>> list)
        {
            var nextOne = false;
            foreach(var pageKvp in list)
            {
                if(nextOne)
                    return pageKvp.Key;
                if(pageKvp.Key == currentPageName)
                    nextOne = true;
            }
            return list[0].Key;
        }

        private List<KeyValuePair<string, IInGameMenuPage>> GetRealPages()
        {
            var copy = new List<KeyValuePair<string, IInGameMenuPage>>();
            foreach(var p in pages)
                if(p.Key != "-" && p.Key != pages[0].Key)
                    copy.Add(p);
            return copy;
        }

    }
}