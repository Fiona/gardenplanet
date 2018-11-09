using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using StompyBlondie.Utils;

namespace GardenPlanet
{
    public class InGameMenuPageInventory : MonoBehaviour, IInGameMenuPage
    {
        [Header("Settings")]
        public string displayName = "Inventory";
        public float itemInfoOffScreenPos;
        public float itemInfoOnScreenPos;

        [Header("Object references")]
        public GameObject itemInfo;
        public TextMeshProUGUI itemInfoDescription;
        public Image itemInfoImage;
        public TextMeshProUGUI numItemsText;
        public GameObject noItemsMessage;
        public InventoryItemButton itemButtonTemplate;
        public GameObject itemListContent;
        public Scrollbar scrollbar;
        public RectTransform scrollWindow;
        public GameObject itemAttributeTemplate;
        public GameObject ItemAttributeHolder;
        public CanvasGroup hotbarSelection;

        private Inventory.InventoryItemEntry selectedItem;
        private Inventory playerInventory;
        private Sprite missingItemImage;
        private ItemHotbar itemHotbar;
        private InventoryItemButton selectedItemButton;
        private float heightOfContent;
        private float heightOfScrollWindow;
        private bool shownItemInfo;
        private GameController controller;
        private List<InventoryItemButton> buttons;
        private bool doingItemSelection;
        private bool hotbarAssignment;
        private int currentHotbarButton = 1;
        private Coroutine inputCoroutine = null;

        public void Awake()
        {
            itemHotbar = FindObjectOfType<ItemHotbar>();
            missingItemImage = Resources.Load<Sprite>("textures/items/missing_item_image");
            controller = FindObjectOfType<GameController>();
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public IEnumerator Open()
        {
            gameObject.SetActive(true);
            hotbarSelection.gameObject.SetActive(false);
            itemHotbar.StartAssignmentMode(RegisterCurrentItemToHotbar);

            PopulateItemList();
            SetupScrollbar();

            if(selectedItem != null)
                StartCoroutine(SlideItemInfoIn());

            // Auto select first item if not in mouse mode
            if(selectedItem == null && !controller.GameInputManager.mouseMode && playerInventory.Items.Count > 0)
                StartCoroutine(SelectItemEntry(playerInventory.Items[0], buttons[0]));

            // Want the selection thing to be over the hotbar
            hotbarSelection.transform.SetParent(controller.canvasRect.transform, true);
            hotbarSelection.transform.localScale = Vector3.one;
            hotbarSelection.transform.SetSiblingIndex(hotbarSelection.transform.GetSiblingIndex() - 1);

            // Fade in
            var canvasGroup = GetComponent<CanvasGroup>();
            yield return LerpHelper.QuickFadeIn(canvasGroup, Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME,
                LerpHelper.Type.SmoothStep);
        }

        public IEnumerator Close()
        {
            if(shownItemInfo)
                StartCoroutine(SlideItemInfoOut());
            itemHotbar.StopAssignmentMode();
            var canvasGroup = GetComponent<CanvasGroup>();
            hotbarSelection.transform.SetParent(gameObject.transform, true);
            if(inputCoroutine != null)
            {
                StopCoroutine(inputCoroutine);
                inputCoroutine = null;
            }

            yield return LerpHelper.QuickFadeOut(canvasGroup, Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME,
                LerpHelper.Type.SmoothStep);
            gameObject.SetActive(false);
        }

        public void Input(GameInputManager inputManager)
        {
            if(inputCoroutine == null)
                inputCoroutine = StartCoroutine(DoInput(inputManager));
        }

        public void RegisterCurrentItemToHotbar(int num, bool fromAssignmentMode = false)
        {
            if(selectedItem == null)
                return;
            if(hotbarAssignment && fromAssignmentMode == false)
                return;
            if(itemHotbar.Items[num] == selectedItem)
                itemHotbar.SetItemToHotbar(null, num);
            else
            {
                itemHotbar.SetItemToHotbar(selectedItem, num);
                hotbarSelection.transform.position = itemHotbar.hotbarButtons[num].transform.position;
                if(controller.GameInputManager.mouseMode && !hotbarAssignment)
                {
                    hotbarSelection.gameObject.SetActive(true);
                    StartCoroutine(LerpHelper.QuickFadeOut(hotbarSelection, 1f, LerpHelper.Type.SmoothStep));
                }
            }
        }

        public void ScrollbarValueChanged(float value)
        {
            var overflow = Math.Abs(heightOfContent - heightOfScrollWindow);
            itemListContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, overflow * value);
        }

        private void PopulateItemList()
        {
            if(itemListContent.transform.childCount > 0)
                return;

            selectedItem = null;
            itemInfo.GetComponent<CanvasGroup>().alpha = 0;
            itemButtonTemplate.gameObject.SetActive(false);

            playerInventory = FindObjectOfType<GameController>().world.player.inventory;
            if(playerInventory.Items.Count > 0)
                noItemsMessage.gameObject.SetActive(false);

            buttons = new List<InventoryItemButton>();
            for(int index = 0; index < playerInventory.Items.Count; index++)
            {
                var item = playerInventory.Items[index];
                var newButton = Instantiate(itemButtonTemplate);
                newButton.gameObject.SetActive(true);
                newButton.SetCallback(
                    delegate { StartCoroutine(SelectItemEntry(item, newButton)); }
                );
                newButton.Initialise(item);
                newButton.transform.SetParent(itemListContent.transform, false);
                buttons.Add(newButton);
            }

            UpdateNumItemsText();
        }

        private void SetupScrollbar()
        {
            if(itemListContent.transform.childCount == 0)
            {
                scrollbar.gameObject.SetActive(false);
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(itemListContent.GetComponent<RectTransform>());

            // Get content and scroll window heights
            heightOfScrollWindow = scrollWindow.sizeDelta.y;
            heightOfContent = itemListContent.GetComponent<RectTransform>().sizeDelta.y;

            // No need for scroll bar if content inside scroll
            if(heightOfContent < heightOfScrollWindow)
            {
                scrollbar.gameObject.SetActive(false);
                return;
            }

            // Set size of scroll bar handle
            var overflow = heightOfContent - heightOfScrollWindow;
            scrollbar.size = 1f - (overflow / heightOfScrollWindow);
            if(scrollbar.size < .1f)
                scrollbar.size = .1f;

            // Set callback for setting value of scroll bar
            scrollbar.onValueChanged.AddListener(ScrollbarValueChanged);
        }

        private void UpdateNumItemsText()
        {
            numItemsText.text = String.Format(
                "{0}/{1}",
                playerInventory.Items.Count,
                playerInventory.maximumItemStacks
            );
        }


        private IEnumerator DoInput(GameInputManager inputManager)
        {
            while(true)
            {
                // Assigning to hotbar captures input
                while(hotbarAssignment)
                {
                    // moving along hotbar
                    if(inputManager.player.GetButtonRepeating("Menu Left"))
                    {
                        currentHotbarButton--;
                        if(currentHotbarButton < 1)
                            currentHotbarButton = Consts.HOTBAR_SIZE;
                    }

                    if(inputManager.player.GetButtonRepeating("Menu Right"))
                    {
                        currentHotbarButton++;
                        if(currentHotbarButton > Consts.HOTBAR_SIZE)
                            currentHotbarButton = 1;
                    }

                    hotbarSelection.transform.position =
                        itemHotbar.hotbarButtons[currentHotbarButton - 1].transform.position;
                    // Selecting hotbar button
                    if(inputManager.player.GetButtonDown("Confirm"))
                    {
                        RegisterCurrentItemToHotbar(currentHotbarButton - 1, true);
                        StopHotbarAssignment();
                    }

                    // Exiting mode
                    if(inputManager.player.GetButtonDown("Cancel"))
                        StopHotbarAssignment();
                    yield return new WaitForFixedUpdate();
                }

                // Navigating items
                if(inputManager.player.GetButtonRepeating("Menu Down"))
                    SelectNextItem();
                if(inputManager.player.GetButtonRepeating("Menu Up"))
                    SelectPreviousItem();
                if(inputManager.player.GetButtonRepeating("Menu Left") && scrollbar.gameObject.activeSelf)
                    SelectPreviousItem(3);
                if(inputManager.player.GetButtonRepeating("Menu Right") && scrollbar.gameObject.activeSelf)
                    SelectNextItem(3);
                if(inputManager.player.GetButtonDown("Confirm"))
                    StartHotbarAssignment();

                // Assigning to hotbar
                for(var i = 0; i < 10; i++)
                    if(inputManager.player.GetButtonUp(String.Format("Assign To Hotbar {0}", i + 1)))
                        RegisterCurrentItemToHotbar(i);

                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator SelectItemEntry(Inventory.InventoryItemEntry item, InventoryItemButton buttonPressed)
        {
            if(doingItemSelection)
                yield break;

            if(selectedItemButton != null)
                selectedItemButton.Deselect();
            selectedItemButton = buttonPressed;
            selectedItem = item;

            // Incase the page triggered the select item through input actions, this triggers the selection anim
            if(!buttonPressed.selected)
                buttonPressed.Select();

            if(shownItemInfo)
                yield return SlideItemInfoOut();

            // Show description and image
            itemInfo.GetComponent<CanvasGroup>().alpha = 1;
            itemInfoDescription.text = selectedItem.itemType.Description;
            if(selectedItem.itemType.Image == null)
                itemInfoImage.sprite = missingItemImage;
            else
                itemInfoImage.sprite = selectedItem.itemType.Image;

            // Sort out attributes
            itemAttributeTemplate.SetActive(false);
            foreach(Transform t in ItemAttributeHolder.transform)
                Destroy(t.gameObject);

            foreach(var attr in item.GetAttributeDisplay())
            {
                var newAttr = Instantiate(itemAttributeTemplate);
                newAttr.gameObject.SetActive(true);
                newAttr.transform.SetParent(ItemAttributeHolder.transform, false);
                newAttr.transform.Find("AttributeNameText").GetComponent<TextMeshProUGUI>().text = attr.Key;
                newAttr.transform.Find("AttributeNameValue").GetComponent<TextMeshProUGUI>().text = attr.Value;
            }

            StartCoroutine(SlideItemInfoIn());

            // Do scroll window
            if(scrollbar.gameObject.activeSelf)
            {
                var buttonY = Mathf.Abs(selectedItemButton.GetComponent<RectTransform>().anchoredPosition.y);
                if(buttonY < heightOfScrollWindow / 2)
                    SetScrollbarTo(0f);
                else if(buttonY > heightOfContent - (heightOfScrollWindow / 2))
                {
                    SetScrollbarTo(1f);
                }
                else
                {
                    SetScrollbarTo(buttonY / heightOfContent);
                }
            }

            doingItemSelection = false;
        }

        private void SetScrollbarTo(float value)
        {
            StartCoroutine(
                LerpHelper.QuickTween(
                    (float val) => { scrollbar.value = val; },
                    scrollbar.value,
                    value,
                    .2f,
                    lerpType:LerpHelper.Type.Exponential
                )
            );
        }

        private IEnumerator SlideItemInfoIn()
        {
            var rect = itemInfo.GetComponent<RectTransform>();
            var onScreen = new Vector2(itemInfoOnScreenPos, rect.anchoredPosition.y);
            var offScreen = new Vector2(itemInfoOffScreenPos, onScreen.y);
            while(doingItemSelection)
                yield return new WaitForFixedUpdate();
            yield return LerpHelper.QuickTween(
                (v) => { rect.anchoredPosition = v; },
                offScreen,
                onScreen,
                .2f, lerpType:LerpHelper.Type.EaseIn
            );
            shownItemInfo = true;
        }

        private IEnumerator SlideItemInfoOut()
        {
            var rect = itemInfo.GetComponent<RectTransform>();
            var onScreen = new Vector2(itemInfoOnScreenPos, rect.anchoredPosition.y);
            var offScreen = new Vector2(itemInfoOffScreenPos, onScreen.y);
            yield return LerpHelper.QuickTween(
                (v) => { rect.anchoredPosition = v; },
                onScreen,
                offScreen,
                .2f, lerpType:LerpHelper.Type.EaseOut
            );
            shownItemInfo = false;
        }

        private void SelectNextItem(int num = 1)
        {
            if(playerInventory.Items.Count == 0)
                return;
            var nextOne = false;
            for(int index = 0; index < playerInventory.Items.Count; index++)
            {
                if(nextOne)
                {
                    StartCoroutine(SelectItemEntry(playerInventory.Items[index], buttons[index]));
                    return;
                }

                if(playerInventory.Items[index] == selectedItem)
                {
                    index += num - 1;
                    nextOne = true;
                }
            }

            // If we're jumping multiple items we shouldn't wrap, instead should snap to the end
            var itemIndex = num == 1 ? 0 : playerInventory.Items.Count-1;
            StartCoroutine(SelectItemEntry(playerInventory.Items[itemIndex], buttons[itemIndex]));
        }

        private void SelectPreviousItem(int num = 1)
        {
            if(playerInventory.Items.Count == 0)
                return;
            var nextOne = false;
            for(int index = playerInventory.Items.Count -1; index >= 0; index--)
            {
                if(nextOne)
                {
                    StartCoroutine(SelectItemEntry(playerInventory.Items[index], buttons[index]));
                    return;
                }

                if(playerInventory.Items[index] == selectedItem)
                {
                    index -= num - 1;
                    nextOne = true;
                }
            }

            // If we're jumping multiple items we shouldn't wrap, instead should snap to the start
            var itemIndex = num == 1 ? playerInventory.Items.Count-1 : 0;
            StartCoroutine(SelectItemEntry(playerInventory.Items[itemIndex], buttons[itemIndex]));
        }

        private void StartHotbarAssignment()
        {
            if(selectedItem == null || hotbarAssignment)
                return;
            hotbarAssignment = true;
            hotbarSelection.gameObject.SetActive(true);
            hotbarSelection.transform.position = itemHotbar.hotbarButtons[currentHotbarButton-1].transform.position;
            StartCoroutine(LerpHelper.QuickFadeIn(hotbarSelection, .25f, LerpHelper.Type.SmoothStep));
        }

        private void StopHotbarAssignment()
        {
            if(!hotbarAssignment)
                return;
            hotbarAssignment = false;
            StartCoroutine(LerpHelper.QuickFadeOut(hotbarSelection, .25f, LerpHelper.Type.SmoothStep));
        }

    }
}