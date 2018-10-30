using System;
using System.Collections;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GardenPlanet
{
    public class ItemHotbar: MonoBehaviour
    {

        public Button[] hotbarButtons;
        public Image[] hotbarImages;
        public TextMeshProUGUI[] hotbarQuantityText;
        public ItemHotbarNameplate nameplate;

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

        // Keeps the Y position for moving on and off the screen
        private float onScreenYPos;
        private RectTransform rectTransform;

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            SelectItemIndex(0);
            StartCoroutine(DoHotbar());
            rectTransform = GetComponent<RectTransform>();
            onScreenYPos = rectTransform.anchoredPosition.y;
        }

        IEnumerator DoHotbar()
        {
            while(true)
            {
                UpdateHotbarState();
                HandleActiveItemScript();
                if(!assignmentMode && selectedItemEntry != null)
                    nameplate.Show(
                        selectedItemEntry.itemType.DisplayName,
                        hotbarButtons[selectedItemIndex].transform
                    );
                else
                    nameplate.Hide();
                yield return new WaitForFixedUpdate();
            }
        }

        // Update the state of all hotbar buttons
        private void UpdateHotbarState()
        {
            for(int i = 0; i < Consts.HOTBAR_SIZE; i++)
            {
                var itemEntry = hotbarItems[i];
                if(itemEntry != null && !controller.world.player.inventory.ItemEntryExists(itemEntry))
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

            if(newIndex == selectedItemIndex || !controller.GameInputManager.directInputEnabled)
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

            // Tell controller we're holding a new item
            if(selectedItemEntry == null)
                controller.PlayerStopHoldingItem();
            else
                controller.PlayerStartHoldingItem(selectedItemEntry.itemType, selectedItemEntry.attributes);
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

        public bool RemoveItemInHand()
        {
            if(selectedItemEntry == null)
                return false;
            if(!controller.world.player.inventory.ItemEntryExists(selectedItemEntry))
            {
                Debug.LogError("Item entry in hotbar not in inventory!");
                StopItemScript();
                return false;
            }

            // Remove from inventory
            return controller.world.player.inventory.RemoveItem(selectedItemEntry, 1);
        }

        public void UpdateItemInHand()
        {
            UpdateHotbarState();
            SelectItemIndex(selectedItemIndex);
            HandleActiveItemScript();
            if(selectedItemEntry == null)
                controller.PlayerStopHoldingItem();
            else
                controller.PlayerStartHoldingItem(selectedItemEntry.itemType, selectedItemEntry.attributes);
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

        public void EnterScreen()
        {
            StartCoroutine(LerpHelper.QuickTween(
                (val) => { rectTransform.anchoredPosition = val; },
                new Vector2(0, -onScreenYPos),
                new Vector2(0, onScreenYPos),
                .2f,
                lerpType:LerpHelper.Type.SmoothStep
            ));
        }

        public void LeaveScreen()
        {
            StartCoroutine(LerpHelper.QuickTween(
                (val) => { rectTransform.anchoredPosition = val; },
                new Vector2(0, onScreenYPos),
                new Vector2(0, -onScreenYPos),
                .2f,
                lerpType:LerpHelper.Type.SmoothStep
            ));
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