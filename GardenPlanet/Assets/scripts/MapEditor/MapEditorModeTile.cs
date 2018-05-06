using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GardenPlanet
{
    public class MapEditorModeTile: MapEditorMode
    {

        GameObject currentTilePanel;
        Text currentTileText;
        GameObject currentTileTemplate;
        EditTileDialog editTileDialog;
        string currentTileName;
        string previousTileType;
        GameObject currentTileTypeSelectedObj;
        Direction newTileDirection;

        public override string GetModeName()
        {
            return "Tile Drawing";
        }

        public override string GetGUIPrefabPath()
        {
            return Consts.PREFAB_PATH_EDITOR_MODE_TILE_DRAWING;
        }

        public override void Initialize()
        {
            base.Initialize();
            SelectTileType(controller.tileTypeSet.types[0].name);
            newTileDirection = Direction.Down;
        }

        public override void InitializeGUI()
        {
            base.InitializeGUI();

            // Get child objects
            editTileDialog = guiHolder.GetComponentsInChildren(typeof(EditTileDialog), true)[0] as EditTileDialog;
            currentTilePanel = guiHolder.transform.Find("CurrentTilePanel").gameObject;
            currentTileTemplate = currentTilePanel.transform.Find("CurrentTileTemplate").gameObject;
            currentTileTemplate.SetActive(false);
            currentTileText = currentTilePanel.transform.Find("CurrentTileText").gameObject.GetComponent<Text>();

            // Wire up buttons
            currentTilePanel.transform.Find("NextTileButton").GetComponent<Button>().onClick.AddListener(NextTileButtonPressed);
            currentTilePanel.transform.Find("PrevTileButton").GetComponent<Button>().onClick.AddListener(PreviousTileButtonPressed);
            currentTilePanel.transform.Find("EditTileButton").GetComponent<Button>().onClick.AddListener(EditTileButtonPressed);
            currentTilePanel.transform.Find("EmptyTileButton").GetComponent<Button>().onClick.AddListener(EmptyTileButtonPressed);
        }

        public override void Update()
        {
            if(controller.currentHoveredTile == null)
                return;
            
            var axis = Input.GetAxis("Mouse ScrollWheel");

            if(Mathf.Abs(axis) >= Consts.MOUSE_WHEEL_CLICK_SNAP)
            {
                var dir = (axis > 0.0f ? RotationalDirection.AntiClockwise : RotationalDirection.Clockwise);
                controller.tilemap.RotateTileInDirection(controller.currentHoveredTile, dir);
                newTileDirection = controller.currentHoveredTile.direction;
            }            
        }
        
        public override void Destroy()
        {
            base.Destroy();
            if(currentTileTypeSelectedObj != null)
                UnityEngine.Object.Destroy(currentTileTypeSelectedObj);
        }

        public override void SaveToMap(Map map)
        {        
        }

        public override void ResizeMap(int width, int height)
        {
        }

        public override void TileLocationClicked(TilePosition tilePos, PointerEventData pointerEventData)
        {
            if(pointerEventData.button == PointerEventData.InputButton.Left)
            {
                controller.tilemap.RemoveTile(tilePos.x, tilePos.y, tilePos.layer);
                controller.tilemap.AddTile(currentTileName, tilePos.x, tilePos.y, tilePos.layer, newTileDirection);
            }
            else if(pointerEventData.button == PointerEventData.InputButton.Right)
                SelectTileType(controller.currentHoveredTile.tileTypeName);            
        }

        /*
          Switches selected tile type for drawing
         */
        private void SelectTileType(string tileTypeName)
        {

            if(currentTileTypeSelectedObj != null)
            {
                UnityEngine.Object.Destroy(currentTileTypeSelectedObj);
                currentTileTypeSelectedObj = null;
            }

            if(tileTypeName == null)
            {
                currentTileName = null;
                currentTileText.text = "Empty";
                return;
            }

            currentTileTypeSelectedObj = controller.tileTypeSet.InstantiateTile(tileTypeName);

            currentTileTypeSelectedObj.transform.parent = currentTilePanel.transform;
            currentTileTypeSelectedObj.AddComponent<RectTransform>();
            var rect = currentTileTypeSelectedObj.GetComponent<RectTransform>();

            // Base values on template
            currentTileTypeSelectedObj.layer = currentTileTemplate.layer;
            rect.localPosition = currentTileTemplate.GetComponent<RectTransform>().localPosition;
            rect.localScale = currentTileTemplate.GetComponent<RectTransform>().localScale;
            rect.localRotation = currentTileTemplate.GetComponent<RectTransform>().localRotation;
            
            currentTileName = tileTypeName;
            currentTileText.text = currentTileName;

        }

        /*
          Edit tile button
        */
        public void EditTileButtonPressed()
        {
            if(currentTileName != null)
                guiHolder.GetComponent<StompyBlondie.DummyMonoBehaviour>().StartCoroutine(editTileDialog.Show(currentTileName));
        }

        /*
          Pressed button to go to the next tile
         */
        public void NextTileButtonPressed()
        {

            if(currentTileName == null)
            {
                SelectTileType(previousTileType);
                return;
            }

            TileType typeObject = null;
            try
            {
                typeObject = controller.tileTypeSet.GetTileTypeByName(currentTileName);
            }
            catch(EditorErrorException){ }

            var currentIndex = controller.tileTypeSet.types.IndexOf(typeObject);
            int chosenIndex;

            if(currentIndex+1 == controller.tileTypeSet.types.Count)
                chosenIndex = 0;
            else
                chosenIndex = currentIndex+1;

            SelectTileType(controller.tileTypeSet.types[chosenIndex].name);

        }

        /*
          Pressed button to go to the previous tile
         */
        public void PreviousTileButtonPressed()
        {

            if(currentTileName == null)
            {
                SelectTileType(previousTileType);
                return;
            }

            TileType typeObject = null;
            try
            {
                typeObject = controller.tileTypeSet.GetTileTypeByName(currentTileName);
            }
            catch(EditorErrorException){ }

            var currentIndex = controller.tileTypeSet.types.IndexOf(typeObject);
            int chosenIndex;

            if(currentIndex == 0)
                chosenIndex = controller.tileTypeSet.types.Count - 1;
            else
                chosenIndex = currentIndex-1;

            SelectTileType(controller.tileTypeSet.types[chosenIndex].name);

        }

        /*
          Pressed button to cancel out the selection.
         */
        public void EmptyTileButtonPressed()
        {
            if(currentTileName == null)
            {
                if(string.IsNullOrEmpty(previousTileType))
                    SelectTileType(controller.tileTypeSet.types[0].name);
                else
                    SelectTileType(previousTileType);
                return;
            }
            previousTileType = currentTileName;
            SelectTileType(null);
        }
            
    }
}