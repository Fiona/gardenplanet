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
        public Button[] hotbarButtons;
        Inventory.InventoryItemEntry selectedItem;
        Inventory playerInventory;
        Sprite missingItemImage;
        ItemHotbar itemHotbar;

        public void Start()
        {
            itemHotbar = FindObjectOfType<ItemHotbar>();
            missingItemImage = Resources.Load<Sprite>("textures/items/missing_item_image");
        }

        public void Open()
        {            
            selectedItem = null;
            itemInfo.GetComponent<CanvasGroup>().alpha = 0;
            itemButtonTemplate.SetActive(false);

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
                newButton.GetComponentsInChildren<Image>()[1].sprite = item.itemType.Image;
                newButton.transform.SetParent(itemListContent.transform, false);
                var itemIndex = index;
                newButton.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        SelectItemEntry(item);
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

        public void SelectItemEntry(Inventory.InventoryItemEntry item)
        {            
            selectedItem = item;

            itemInfo.GetComponent<CanvasGroup>().alpha = 1;
            itemInfoName.text = selectedItem.itemType.DisplayName;
            itemInfoDescription.text = selectedItem.itemType.Description;
            if(selectedItem.itemType.Image == null)
                itemInfoImage.sprite = missingItemImage;
            else
                itemInfoImage.sprite = selectedItem.itemType.Image;
            
            SetupHotbarButtons();
        }

        void SetupHotbarButtons()
        {
            for(int i = 0; i < Consts.HOTBAR_SIZE; i++)
            {
                var button = hotbarButtons[i];
                var img = button.gameObject.GetComponentsInChildren<Image>(true)[1];
                var text = button.gameObject.GetComponentInChildren<Text>(true);
                if(itemHotbar.Items[i] == null)
                {
                    text.gameObject.SetActive(true);
                    img.gameObject.SetActive(false);
                }
                else
                {
                    text.gameObject.SetActive(false);
                    img.gameObject.SetActive(true);
                    img.sprite = itemHotbar.Items[i].itemType.Image;
                }
            }
        }

        public void RegisterCurrentItemToHotbar(int num)
        {
            if(selectedItem == null)
                return;
            if(itemHotbar.Items[num] == selectedItem)
                itemHotbar.SetItemToHotbar(null, num);
            else
                itemHotbar.SetItemToHotbar(selectedItem, num);
            SetupHotbarButtons();
        }

    }
}

