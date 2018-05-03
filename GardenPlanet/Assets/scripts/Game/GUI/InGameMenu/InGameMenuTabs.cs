using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace GardenPlanet
{
    public class InGameMenuTabs: MonoBehaviour
    {

        [Header("Object references")]
        public InGameMenu inGameMenu;
        public GameObject tabButtonTemplate;
        public GameObject tabSpacerTemplate;
        public GameObject tabHolder;

        private Dictionary<string, InGameMenuTabButton> buttons;
        private InGameMenuTabButton selectedTabButton;

        private void Awake()
        {
            tabButtonTemplate.SetActive(false);
            tabSpacerTemplate.SetActive(false);
        }

        public void CreateTabsFromPages(List<KeyValuePair<string, IInGameMenuPage>> pages)
        {
            // Cleanup existing tabs
            foreach(Transform button in tabHolder.transform)
                Destroy(button.gameObject);

            buttons = new Dictionary<string, InGameMenuTabButton>();

            // Create tab for each page
            foreach(var kvp in pages)
            {
                // Tab spacers
                if(kvp.Key == "-")
                {
                    var spacer = Instantiate(tabSpacerTemplate);
                    spacer.SetActive(true);
                    spacer.transform.SetParent(tabHolder.transform, false);
                    continue;
                }

                // Create new button
                var newButton = Instantiate(tabButtonTemplate).GetComponent<InGameMenuTabButton>();
                newButton.transform.SetParent(tabHolder.transform, false);
                newButton.Initialise(
                    kvp.Key,
                    kvp.Value.GetDisplayName(),
                    () => TabButtonClicked(kvp.Key)
                    );
                buttons[kvp.Key] = newButton;
            }
        }

        public void TabButtonClicked(string name)
        {
            inGameMenu.DoPageOpen(name);
        }

        public void SelectTab(string name)
        {
            if(!buttons.ContainsKey(name))
            {
                Debug.Log("Can't select tab because it doesn't exist.");
                return;
            }

            buttons[name].Select();
            selectedTabButton = buttons[name];
        }

        public void DeselectTab()
        {
            if(selectedTabButton == null)
            {
                Debug.Log("Can't deselect tab because one wasn't selected.");
                return;
            }

            selectedTabButton.Deselect();
            selectedTabButton = null;
        }

        public void HideTabs()
        {
            foreach(var button in buttons)
                button.Value.SlideOut();
        }

    }
}