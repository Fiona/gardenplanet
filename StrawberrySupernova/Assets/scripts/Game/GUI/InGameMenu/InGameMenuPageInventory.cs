using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StompyBlondie;
using TMPro;

namespace StrawberryNova
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
            yield return LerpHelper.QuickFadeOut(canvasGroup, Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME,
                LerpHelper.Type.SmoothStep);
            gameObject.SetActive(false);
        }

        public void RegisterCurrentItemToHotbar(int num)
        {
            if(selectedItem == null)
                return;
            if(itemHotbar.Items[num] == selectedItem)
                itemHotbar.SetItemToHotbar(null, num);
            else
            {
                itemHotbar.SetItemToHotbar(selectedItem, num);
                hotbarSelection.transform.position = itemHotbar.hotbarButtons[num].transform.position;
                if(controller.GameInputManager.mouseMode)
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

            playerInventory = FindObjectOfType<GameController>().player.inventory;
            if(playerInventory.Items.Count > 0)
                noItemsMessage.gameObject.SetActive(false);
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

        private IEnumerator SelectItemEntry(Inventory.InventoryItemEntry item, InventoryItemButton buttonPressed)
        {
            if(selectedItemButton != null)
                selectedItemButton.Deselect();
            selectedItemButton = buttonPressed;
            selectedItem = item;

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

            yield return SlideItemInfoIn();
        }

        private IEnumerator SlideItemInfoIn()
        {
            var rect = itemInfo.GetComponent<RectTransform>();
            var onScreen = new Vector2(itemInfoOnScreenPos, rect.anchoredPosition.y);
            var offScreen = new Vector2(itemInfoOffScreenPos, onScreen.y);
            shownItemInfo = true;
            yield return LerpHelper.QuickTween(
                (v) => { rect.anchoredPosition = v; },
                offScreen,
                onScreen,
                .2f, lerpType:LerpHelper.Type.EaseIn
            );
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

    }
}