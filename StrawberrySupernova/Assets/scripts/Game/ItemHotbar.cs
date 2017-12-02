﻿using System;
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
        [Header("Active item")]
        public GameObject activeItem;
        [Header("Hotbar button images")]
        public Sprite buttonImage;
        public Sprite buttonHoverImage;
        public Sprite buttonPressedImage;
        [Header("Selected button images")]
        public Sprite buttonAcitveImage;
        public Sprite buttonActiveHoverImage;
        public Sprite buttonActivePressedImage;

        public Inventory.InventoryItemEntry selectedItemEntry
        {
            get { return hotbarItems[selectedItemIndex]; }
        }
        public Inventory.InventoryItemEntry[] Items
        {
            get{ return hotbarItems; }
            set{ hotbarItems = value; }
        }

        private GameController controller;
        private IItemScript activeItemScript;
        private Inventory.InventoryItemEntry activeScriptEntry;

        private int selectedItemIndex;
        private Inventory.InventoryItemEntry[] hotbarItems = new Inventory.InventoryItemEntry[Consts.HOTBAR_SIZE];

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            SelectItemIndex(0);
            StartCoroutine(DoHotbar());
        }

        IEnumerator DoHotbar()
        {
            var inventory = controller.player.inventory;

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
            SelectItemIndex(selectedItemIndex);
        }

        public void SelectItemIndex(int newIndex)
        {
            // Change current hotbar item back to unselected state
            SpriteState unclickedSpriteState = new SpriteState();
            unclickedSpriteState.highlightedSprite = buttonHoverImage;
            unclickedSpriteState.pressedSprite = buttonPressedImage;
            hotbarButtons[selectedItemIndex].spriteState = unclickedSpriteState;
            hotbarButtons[selectedItemIndex].GetComponent<Image>().sprite = buttonImage;
            hotbarButtons[selectedItemIndex].transform.localScale = new Vector3(1f, 1f, 1f);

            // Set new one
            selectedItemIndex = newIndex;

            // Update state of newly selected item
            SpriteState activeSpriteState = new SpriteState();
            activeSpriteState.highlightedSprite = buttonActiveHoverImage;
            activeSpriteState.pressedSprite = buttonActivePressedImage;
            hotbarButtons[selectedItemIndex].spriteState = activeSpriteState;
            hotbarButtons[selectedItemIndex].GetComponent<Image>().sprite = buttonAcitveImage;
            hotbarButtons[selectedItemIndex].transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

            // Make active item display reflect selected item and deal with scripts
            var activeText = activeItem.GetComponentInChildren<Text>(true);
            var activeImage = activeItem.GetComponentsInChildren<Image>(true)[1];
            if(hotbarItems[selectedItemIndex] == null)
            {
                activeText.gameObject.SetActive(false);
                activeImage.gameObject.SetActive(false);
                StopItemScript();
            }
            else
            {
                activeText.gameObject.SetActive(true);
                activeText.text = hotbarItems[selectedItemIndex].quantity.ToString();
                activeImage.gameObject.SetActive(true);
                activeImage.sprite = hotbarItems[selectedItemIndex].itemType.Image;
                StartItemScript();
            }
        }

        public void SelectPreviousItem()
        {
            if(selectedItemIndex == 0)
                SelectItemIndex(Consts.HOTBAR_SIZE - 1);
            else
                SelectItemIndex(selectedItemIndex-1);
        }

        public void SelectNextItem()
        {
            if(selectedItemIndex == Consts.HOTBAR_SIZE - 1)
                SelectItemIndex(0);
            else
                SelectItemIndex(selectedItemIndex+1);
        }

        public bool CanBeUsedOnTilePos(TilePosition tilePos)
        {
            return activeItemScript != null && activeItemScript.CanBeUsedOnTilePos(tilePos);
        }

        public IEnumerator UseItemInHandOnTilePos(TilePosition tilePos)
        {
            if(selectedItemEntry == null || activeItemScript == null)
                yield return null;
            controller.StartCutscene();
            yield return StartCoroutine(activeItemScript.UseOnTilePos(tilePos));
            controller.EndCutscene();
        }

        private void StartItemScript()
        {
            var itemType = hotbarItems[selectedItemIndex].itemType;
            if(String.IsNullOrEmpty(itemType.Script) || activeScriptEntry == hotbarItems[selectedItemIndex])
                return;

            var script = Type.GetType(typeof(ItemHotbar).Namespace+".Items."+itemType.Script);
            if(script == null)
                return;

            activeScriptEntry = hotbarItems[selectedItemIndex];
            activeItemScript = gameObject.AddComponent(script) as IItemScript;
            activeItemScript.StartsHolding(activeScriptEntry);
        }

        private void StopItemScript()
        {
            if(activeItemScript == null)
                return;
            Destroy(activeItemScript as MonoBehaviour);
            activeScriptEntry = null;
            activeItemScript = null;
        }

    }
}