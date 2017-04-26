using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class ItemHotbar: MonoBehaviour
    {

        public Button[] hotbarButtons;
        public Image[] hotbarImages;
        public Text[] hotbarQuantityText;
        [Header("Hotbar button images")]
        public Sprite buttonImage;
        public Sprite buttonHoverImage;
        public Sprite buttonPressedImage;
        [Header("Selected button images")]
        public Sprite buttonAcitveImage;
        public Sprite buttonActiveHoverImage;
        public Sprite buttonActivePressedImage;

        int selectedItemIndex;
        Inventory.InventoryItemEntry[] hotbarItems = new Inventory.InventoryItemEntry[Consts.HOTBAR_SIZE];

        public Inventory.InventoryItemEntry[] Items
        {
            get{ return hotbarItems; }
            set{ hotbarItems = value; }
        }

        public void Start()
        {
            SelectItemIndex(0);
            StartCoroutine(DoHotbar());
        }

        IEnumerator DoHotbar()
        {
            var inventory = FindObjectOfType<GameController>().player.inventory;

            while(true)
            {
                // Update the state of all hotbar buttons
                for(int i = 0; i < Consts.HOTBAR_SIZE; i++)
                {
                    var itemEntry = hotbarItems[i];
                    if(itemEntry != null && !inventory.ItemEntryExists(itemEntry))
                        hotbarItems[i] = null;

                    if(hotbarItems[i] == null)
                    {
                        hotbarImages[i].gameObject.SetActive(false);
                        hotbarQuantityText[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        hotbarImages[i].gameObject.SetActive(true);
                        hotbarQuantityText[i].gameObject.SetActive(true);
                        hotbarImages[i].sprite = itemEntry.itemType.Image;
                        hotbarQuantityText[i].text = itemEntry.quantity.ToString();
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public void SetItemToHotbar(Inventory.InventoryItemEntry selectedItem, int num)
        {
            // Unset anywhere that the item currently is
            if(selectedItem != null)
                for(int i = 0; i < Consts.HOTBAR_SIZE; i++)
                    if(hotbarItems[i] == selectedItem)
                        hotbarItems[i] = null;
            hotbarItems[num] = selectedItem;
        }
            
        public void SelectItemIndex(int newIndex)
        {
            // Change images back on old one
            SpriteState unclickedSpriteState = new SpriteState();
            unclickedSpriteState.highlightedSprite = buttonHoverImage;
            unclickedSpriteState.pressedSprite = buttonPressedImage;
            hotbarButtons[selectedItemIndex].spriteState = unclickedSpriteState;
            hotbarButtons[selectedItemIndex].GetComponent<Image>().sprite = buttonImage;

            // Set new one
            selectedItemIndex = newIndex;

            SpriteState activeSpriteState = new SpriteState();
            activeSpriteState.highlightedSprite = buttonActiveHoverImage;
            activeSpriteState.pressedSprite = buttonActivePressedImage;
            hotbarButtons[selectedItemIndex].spriteState = activeSpriteState;
            hotbarButtons[selectedItemIndex].GetComponent<Image>().sprite = buttonAcitveImage;
        }

    }
}

