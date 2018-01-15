using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace StrawberryNova
{
    public class InGameMenuTabs: MonoBehaviour
    {

        [Header("Object references")]
        public InGameMenu inGameMenu;
        public GameObject tabButtonTemplate;
        public GameObject tabSpacerTemplate;
        public GameObject tabHolder;

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
            }
        }

        public void TabButtonClicked(string name)
        {
            StartCoroutine(inGameMenu.OpenPage(name));
        }

    }
}