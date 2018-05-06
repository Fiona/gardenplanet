using TMPro;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GardenPlanet
{
    public class MapEditorModeTileTags: MapEditorMode
    {
        private TileTagManager tagManager;
        private TileTagType currentSelectedTagType;
        private List<Button> tagButtons;
        
        public override string GetModeName()
        {
            return "Tile Tagging";
        }

        public override string GetGUIPrefabPath()
        {
            return Consts.PREFAB_PATH_EDITOR_MODE_TILE_TAGS;
        }

        public override void Initialize()
        {
            base.Initialize();
            var tagManagerObj = new GameObject("TileTagManager");
            tagManager = tagManagerObj.AddComponent<TileTagManager>();
            tagManager.LoadFromMap(controller.map);
            LateInitializeGUI();
            SelectTagType(tagManager.TileTagTypes[0], 0);
        }

        private void LateInitializeGUI()
        {
            var tagButtonList = guiHolder.transform.Find("CurrentTagPanel/TagButtonList").gameObject;
            var tagButtonTemplate = tagButtonList.transform.Find("TagButtonTemplate").gameObject;
            tagButtonTemplate.SetActive(false);
            tagButtons = new List<Button>();

            foreach(var tagType in tagManager.TileTagTypes)
            {
                var newTagButton = UnityEngine.Object.Instantiate(tagButtonTemplate);
                newTagButton.transform.Find("TagButtonText").GetComponent<TextMeshProUGUI>().text = tagType.ID;
                newTagButton.transform.Find("TagButtonImage").GetComponent<Image>().sprite = tagType.sprite;
                newTagButton.transform.SetParent(tagButtonList.transform, false);   
                var num = tagButtons.Count;
                newTagButton.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        SelectTagType(tagType, num);
                    }
                );
                newTagButton.SetActive(true);
                tagButtons.Add(newTagButton.GetComponent<Button>());
            }
        }
        
        public override void Destroy()
        {
            base.Destroy();
            UnityEngine.Object.Destroy(tagManager.gameObject);            
        }

        public override void Enable()
        {
            base.Enable();
            foreach(var tagObj in tagManager.EditorTileTagObjects.Values)
                tagObj.SetActive(true);
        }
        
        public override void Disable()
        {
            base.Disable();
            foreach(var tagObj in tagManager.EditorTileTagObjects.Values)
                tagObj.SetActive(false);
        }

        public override void TileLocationClicked(TilePosition tilePos, PointerEventData pointerEventData)
        {
            if(pointerEventData.button == PointerEventData.InputButton.Left)
            {
                var existingTag = tagManager.FindTagAt(tilePos.x, tilePos.y, tilePos.layer);
                if(existingTag != null)
                    tagManager.RemoveTag((TileTag)existingTag);
                
                var newTag = new TileTag()
                {
                    TagType = currentSelectedTagType.ID,
                    X = tilePos.x,
                    Y = tilePos.y,                    
                    Layer = tilePos.layer
                };
                tagManager.AddTag(newTag);
            }
            else if(pointerEventData.button == PointerEventData.InputButton.Right)
            {
                var existingTag = tagManager.FindTagAt(tilePos.x, tilePos.y, tilePos.layer);
                if(existingTag != null)
                    tagManager.RemoveTag((TileTag)existingTag);                
            }
        }

        public override void SaveToMap(Map map)
        {
            tagManager.SaveToMap(map);
        }

        public override void ResizeMap(int width, int height)
        {
            tagManager.ResizeMap(width, height);            
        }

        private void SelectTagType(TileTagType newTagType, int buttonIndex)
        {
            currentSelectedTagType = newTagType;
            foreach(var button in tagButtons)
                button.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            tagButtons[buttonIndex].GetComponent<Image>().color = new Color(.5f, 1.0f, .5f, 1.0f);
        }
                
    }
}