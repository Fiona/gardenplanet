using System;
using System.Collections;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class ItemHotbar: MonoBehaviour
    {

        public Button[] hotbarButtons;
        public Image[] hotbarImages;
        public TextMeshProUGUI[] hotbarQuantityText;
        [Header("Selection Marker")]
        public Image selectionMarker;
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

        // If true the hotbar is being assigned to from the inventory menu
        private bool assignmentMode;

        // Ends up getting called when a hotbar button is clicked and assignment mode is active
        private Action<int, bool> assignmentCallback;

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
                HandleActiveItemScript();
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
                    hotbarImages[i].sprite = null;
                    hotbarImages[i].color = new Color(1f,1f,1f,0f);
                    hotbarQuantityText[i].gameObject.SetActive(false);
                }
                else
                {
                    hotbarImages[i].sprite = itemEntry.itemType.Image;
                    hotbarImages[i].color = new Color(1f,1f,1f,1f);
                    hotbarQuantityText[i].gameObject.SetActive(true);
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
            UpdateHotbarState();
        }

        public void SelectItemIndex(int newIndex)
        {
            if(assignmentMode)
            {
                assignmentCallback(newIndex, false);
                return;
            }

            if(newIndex == selectedItemIndex)
                return;

            // Apply and set marker position
            selectedItemIndex = newIndex;
            selectionMarker.transform.position = hotbarButtons[selectedItemIndex].transform.position;

            // Do little fade in
            StartCoroutine(LerpHelper.QuickTween(
                (val) => { selectionMarker.color = val; },
                new Color(1f, 1f, 1f, 0f),
                new Color(1f, 1f, 1f, 1f),
                .1f
            ));

            HandleActiveItemScript();
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

        public void StartAssignmentMode(Action<int, bool> assignmentCallback)
        {
            StartCoroutine(LerpHelper.QuickTween(
                (val) => { selectionMarker.color = val; },
                new Color(1f, 1f, 1f, 1f),
                new Color(1f, 1f, 1f, 0f),
                .1f
            ));

            this.assignmentCallback = assignmentCallback;
            assignmentMode = true;
        }

        public void StopAssignmentMode()
        {
            StartCoroutine(LerpHelper.QuickTween(
                (val) => { selectionMarker.color = val; },
                new Color(1f, 1f, 1f, 0f),
                new Color(1f, 1f, 1f, 1f),
                .1f
            ));
            assignmentMode = false;
            SelectItemIndex(selectedItemIndex);
        }

        private void HandleActiveItemScript()
        {
            if(hotbarItems[selectedItemIndex] == null)
                StopItemScript();
            else
                StartItemScript();
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