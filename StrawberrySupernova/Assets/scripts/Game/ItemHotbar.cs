using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
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
        [HideInInspector]
        public bool activeItemIsTileItem;

        // Get the entry that corresponds to the active item
        public Inventory.InventoryItemEntry selectedItemEntry
        {
            get { return hotbarItems[selectedItemIndex]; }
        }
        // Get all the items on the hotbar
        public Inventory.InventoryItemEntry[] Items
        {
            get{ return hotbarItems; }
            set{ hotbarItems = value; }
        }

        private GameController controller;
        private int selectedItemIndex;
        private Inventory.InventoryItemEntry[] hotbarItems = new Inventory.InventoryItemEntry[Consts.HOTBAR_SIZE];

        // Used to keep track of script activations
        private ItemScript activeItemScript;
        private Inventory.InventoryItemEntry activeItemScriptItemEntry;

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            SelectItemIndex(0);
            StartCoroutine(DoHotbar());
        }

        IEnumerator DoHotbar()
        {
            while(true)
            {
                UpdateHotbarState();
                UpdateActiveItem();
                yield return new WaitForFixedUpdate();
            }
        }

        // Update the state of all hotbar buttons
        private void UpdateHotbarState()
        {
            for(int i = 0; i < Consts.HOTBAR_SIZE; i++)
            {
                var itemEntry = hotbarItems[i];
                if(itemEntry != null && !controller.player.inventory.ItemEntryExists(itemEntry))
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

            UpdateActiveItem();
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

        public bool CanBeUsed()
        {
            return activeItemScript != null && !activeItemIsTileItem && activeItemScript.CanBeUsed();
        }

        public IEnumerator UseItemInHand()
        {
            if(selectedItemEntry == null || activeItemScript == null || activeItemIsTileItem)
                yield break;
            yield return StartCoroutine(activeItemScript.Use());
        }

        public bool CanBeUsedOnTilePos(TilePosition tilePos)
        {
            return activeItemScript != null && activeItemIsTileItem && activeItemScript.CanBeUsedOnTilePos(tilePos);
        }

        public IEnumerator UseItemInHandOnTilePos(TilePosition tilePos)
        {
            if(selectedItemEntry == null || activeItemScript == null || !activeItemIsTileItem)
                yield break;
            yield return StartCoroutine(activeItemScript.UseOnTilePos(tilePos));
        }

        public IEnumerator DropItemInHand()
        {
            if(selectedItemEntry == null)
                yield break;
            if(!controller.player.inventory.ItemEntryExists(selectedItemEntry))
            {
                Debug.LogError("Item entry in hotbar not in inventory!");
                StopItemScript();
                yield break;
            }

            // Spawn item
            var newPos = new WorldPosition(controller.player.transform.localPosition);
            newPos.height += .5f;
            var shiftAmount = .1f;
            newPos.x -= UnityEngine.Random.Range(-shiftAmount, shiftAmount);
            newPos.y -= UnityEngine.Random.Range(-shiftAmount, shiftAmount);
            controller.SpawnItemInWorld(selectedItemEntry.itemType, selectedItemEntry.attributes, 1, newPos);

            // Remove from inventory
            controller.itemManager.RemovePlayerItem(selectedItemEntry.itemType,
                selectedItemEntry.attributes, 1);
            UpdateHotbarState();
            SelectItemIndex(selectedItemIndex);
            yield return null;
        }

        private void UpdateActiveItem()
        {
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

        private void StartItemScript()
        {
            if(activeItemScript != null && selectedItemEntry != activeItemScriptItemEntry)
                StopItemScript();

            var itemType = hotbarItems[selectedItemIndex].itemType;
            if(String.IsNullOrEmpty(itemType.Script) || selectedItemEntry == activeItemScriptItemEntry)
                return;

            var script = Type.GetType(typeof(ItemHotbar).Namespace+".Items."+itemType.Script);
            if(script == null)
                return;

            activeItemScript = gameObject.AddComponent(script) as ItemScript;
            activeItemScriptItemEntry = selectedItemEntry;
            activeItemScript.item = activeItemScriptItemEntry;
            activeItemScript.controller = controller;
            activeItemIsTileItem = activeItemScript.IsTileItem();
            activeItemScript.StartsHolding();
        }

        private void StopItemScript()
        {
            activeItemIsTileItem = false;
            if(activeItemScript == null)
                return;
            Destroy(activeItemScript as MonoBehaviour);
            activeItemScriptItemEntry = null;
            activeItemScript = null;
        }

    }
}