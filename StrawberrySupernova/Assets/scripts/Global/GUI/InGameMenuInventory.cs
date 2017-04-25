using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace StrawberryNova
{
    public class InGameMenuInventory: MonoBehaviour
    {
        public GameObject itemInfo;
        public Text itemInfoName;
        public Text itemInfoDescription;
        public Image itemInfoImage;
        public Text numItemsText;
        public Text noItemsText;
        public GameObject itemButtonTemplate;
        public GameObject itemListContent;
        int selectedItemIndex = -1;
        Inventory playerInventory;
        Sprite missingItemImage;

        public void Open()
        {
            selectedItemIndex = -1;
            itemInfo.GetComponent<CanvasGroup>().alpha = 0;
            itemButtonTemplate.SetActive(false);

            missingItemImage = Resources.Load<Sprite>("textures/items/missing_item_image");

            // Populate item list
            playerInventory = FindObjectOfType<GameController>().player.inventory;
            if(playerInventory.Items.Count > 0)
                noItemsText.gameObject.SetActive(false);
            for(int index = 0; index < playerInventory.Items.Count; index++)
            {
                var item = playerInventory.Items[index];
                var newButton = Instantiate(itemButtonTemplate);
                newButton.SetActive(true);
                foreach(var textObj in newButton.GetComponentsInChildren<Text>())
                {
                    // Horrible
                    if(textObj.name == "Name")
                        textObj.text = item.itemType.DisplayName;
                    else if(textObj.name == "Amount")
                        textObj.text = item.quantity.ToString();
                }
                newButton.transform.SetParent(itemListContent.transform, false);
                var itemIndex = index;
                newButton.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        SelectItemEntry(itemIndex);
                    }
                );
            }

            UpdateNumItemsText();
        }

        public void UpdateNumItemsText()
        {
            numItemsText.text = String.Format(
                "{0}/{1}",
                playerInventory.Items.Count,
                playerInventory.maximumItemStacks
            );
        }

        public void SelectItemEntry(int itemIndex)
        {
            selectedItemIndex = itemIndex;
            itemInfo.GetComponent<CanvasGroup>().alpha = 1;
            itemInfoName.text = playerInventory.Items[itemIndex].itemType.DisplayName;
            itemInfoDescription.text = playerInventory.Items[itemIndex].itemType.Description;
            if(playerInventory.Items[itemIndex].itemType.Image == null)
                itemInfoImage.sprite = missingItemImage;
            else
                itemInfoImage.sprite = playerInventory.Items[itemIndex].itemType.Image;
        }

    }
}

