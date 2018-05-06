using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
    
namespace GardenPlanet
{
    public class MapEditorModeMarker: MapEditorMode
    {
        
        MarkerManager markerManager;
        TileMarkerType selectedMarker;
        TileMarkerType previousMarker;
        Direction newMarkerDirection;
        
        Image currentMarkerPreview;
        Text currentMarkerText;
        
        public override string GetModeName()
        {
            return "Tile Markers";
        }

        public override string GetGUIPrefabPath()
        {
            return Consts.PREFAB_PATH_EDITOR_MODE_MARKER;
        }

        public override void Initialize()
        {
            base.Initialize();
            var markerManagerObj = new GameObject("MarkerManager");
            markerManager = markerManagerObj.AddComponent<MarkerManager>();
            markerManager.LoadFromMap(controller.map);
            SelectMarkerType(markerManager.tileMarkerTypes[0].name);
            newMarkerDirection = Direction.Down;            
        }

        public override void InitializeGUI()
        {
            base.InitializeGUI();
            
            var currentMarkerPanel = guiHolder.transform.Find("CurrentMarkerPanel").gameObject;            
            currentMarkerPreview = currentMarkerPanel.transform.Find("CurrentMarker").GetComponent<Image>();
            currentMarkerText = currentMarkerPanel.transform.Find("CurrentMarkerText").GetComponent<Text>();
            
            currentMarkerPanel.transform.Find("NextMarkerButton").GetComponent<Button>().onClick.AddListener(NextMarkerButtonPressed);
            currentMarkerPanel.transform.Find("PreviousMarkerButton").GetComponent<Button>().onClick.AddListener(PreviousMarkerButtonPressed);
            currentMarkerPanel.transform.Find("EmptyMarkerButton").GetComponent<Button>().onClick.AddListener(EmptyMarkerButtonPressed);
        }

        public override void Destroy()
        {
            base.Destroy();
            UnityEngine.Object.Destroy(markerManager.gameObject);
        }
        
        public override void Update()
        {
            if(controller.currentHoveredTile == null)
                return;
            
            var axis = Input.GetAxis("Mouse ScrollWheel");

            if(Mathf.Abs(axis) >= Consts.MOUSE_WHEEL_CLICK_SNAP)
            {
                var dir = (axis > 0.0f ? RotationalDirection.AntiClockwise : RotationalDirection.Clockwise);                
                var marker = markerManager.GetMarkerAt(
                    controller.currentHoveredTile.x,
                    controller.currentHoveredTile.y,
                    controller.currentHoveredTile.layer
                    );
                if(marker != null)
                    markerManager.RotateMarkerInDirection(marker, dir);						
                newMarkerDirection = marker.dir;                
            }            
        }        

        public override void TileLocationClicked(TilePosition tilePos, PointerEventData pointerEventData)
        {
            if(pointerEventData.button == PointerEventData.InputButton.Left)
            {
                markerManager.RemoveMarkerAt(tilePos.x, tilePos.y, tilePos.layer);
                markerManager.AddMarkerAt(selectedMarker, tilePos.x, tilePos.y, tilePos.layer, newMarkerDirection);
            }
            else if(pointerEventData.button == PointerEventData.InputButton.Right)
            {
                var marker = markerManager.GetMarkerAt(
                    controller.currentHoveredTile.x, controller.currentHoveredTile.y, controller.currentHoveredTile.layer
                );
                SelectMarkerType(marker == null ? null : marker.name);
            }            
        }

        public override void SaveToMap(Map map)
        {        
            markerManager.SaveToMap(map);
        }
        
        public override void ResizeMap(int width, int height)
        {
            markerManager.ResizeMap(width, height);            
        }        
        
        private void SelectMarkerType(string markerTypeName)
        {
            if(markerTypeName == null)
            {
                currentMarkerText.text = "-";
                selectedMarker = null;
                currentMarkerPreview.gameObject.SetActive(false);
                return;
            }
            try
            {
                currentMarkerPreview.gameObject.SetActive(true);
                selectedMarker = markerManager.GetTileMarkerTypeByName(markerTypeName);
                currentMarkerPreview.sprite = selectedMarker.sprite;
            }
            catch(EditorErrorException){ }
            currentMarkerText.text = markerTypeName;
        }
     
        /*
          Pressed button to go to the next tile marker
         */
        public void NextMarkerButtonPressed()
        {
            if(selectedMarker == null)
            {
                SelectMarkerType(previousMarker == null ? null : previousMarker.name);
                return;
            }
            SelectMarkerType(markerManager.GetNextTileMarkerType(selectedMarker).name);
        }

        /*
          Pressed button to go to the previous tile marker
         */
        public void PreviousMarkerButtonPressed()
        {
            if(selectedMarker == null)
            {
                SelectMarkerType(previousMarker == null ? null : previousMarker.name);
                return;
            }
            SelectMarkerType(markerManager.GetPreviousTileMarkerType(selectedMarker).name);
        }

        /*
          Pressed button to cancel out the marker selection.
         */
        public void EmptyMarkerButtonPressed()
        {
            if(selectedMarker == null)
            {
                if(previousMarker == null)
                    SelectMarkerType(markerManager.tileMarkerTypes[0].name);
                else
                    SelectMarkerType(previousMarker.name);
                return;
            }
            previousMarker = selectedMarker;
            SelectMarkerType(null);
        }
        
    }
}