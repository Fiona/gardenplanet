using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using StompyBlondie.Common.Types;

namespace GardenPlanet
{
    public class MapEditorModeMarker: MapEditorMode
    {

        private MarkerManager markerManager;
        private TileMarkerType selectedMarker;
        private TileMarkerType previousMarker;
        private EightDirection newMarkerDirection;

        private Image currentMarkerPreview;
        private Text currentMarkerText;
        private GameObject attributesDialog;
        private TileMarker attributeEditingMarker;

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
            SelectMarkerType(markerManager.tileMarkerTypes[0]);
            newMarkerDirection = EightDirection.Down;
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
            if(controller.currentHoveredTile == null || attributeEditingMarker != null)
                return;

            var marker = markerManager.GetMarkerAt(
                controller.currentHoveredTile.x,
                controller.currentHoveredTile.y,
                controller.currentHoveredTile.layer
            );
            if(marker == null)
                return;

            var axis = Input.GetAxis("Mouse ScrollWheel");
            if(Mathf.Abs(axis) >= Consts.MOUSE_WHEEL_CLICK_SNAP)
            {
                var dir = (axis > 0.0f ? RotationalDirection.AntiClockwise : RotationalDirection.Clockwise);
                markerManager.RotateMarkerInDirection(marker, dir);
                newMarkerDirection = marker.direction;
            }

            if(Input.GetKeyUp(KeyCode.Return))
                OpenAttributesDialog(marker);
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
                SelectMarkerType(marker == null ? null : marker.type);
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

        private void SelectMarkerType(TileMarkerType markerType)
        {
            if(markerType == null)
            {
                currentMarkerText.text = "-";
                selectedMarker = null;
                currentMarkerPreview.gameObject.SetActive(false);
                return;
            }
            try
            {
                currentMarkerPreview.gameObject.SetActive(true);
                selectedMarker = markerType;
                currentMarkerPreview.sprite = selectedMarker.sprite;
            }
            catch(EditorErrorException){ }
            currentMarkerText.text = markerType.name;
        }

        /*
          Pressed button to go to the next tile marker
         */
        public void NextMarkerButtonPressed()
        {
            if(selectedMarker == null)
            {
                SelectMarkerType(previousMarker);
                return;
            }
            SelectMarkerType(markerManager.GetNextTileMarkerType(selectedMarker));
        }

        /*
          Pressed button to go to the previous tile marker
         */
        public void PreviousMarkerButtonPressed()
        {
            if(selectedMarker == null)
            {
                SelectMarkerType(previousMarker);
                return;
            }
            SelectMarkerType(markerManager.GetPreviousTileMarkerType(selectedMarker));
        }

        /*
          Pressed button to cancel out the marker selection.
         */
        public void EmptyMarkerButtonPressed()
        {
            if(selectedMarker == null)
            {
                if(previousMarker == null)
                    SelectMarkerType(markerManager.tileMarkerTypes[0]);
                else
                    SelectMarkerType(previousMarker);
                return;
            }
            previousMarker = selectedMarker;
            SelectMarkerType(null);
        }

        /*
         * Opening attributes editor for the selected marker
         */
        private void OpenAttributesDialog(TileMarker marker)
        {
            attributeEditingMarker = marker;
            attributesDialog = Object.Instantiate(
                Resources.Load<GameObject>(Consts.PREFAB_PATH_EDITOR_ATTRIBUTES_DIALOG)
                );
            attributesDialog.transform.SetParent(guiHolder.transform, false);
            attributesDialog.GetComponent<EditAttributesDialog>().EditAttributes(
                marker.attributes, CancelAttributesCallback, ApplyAttributesCallback
            );
        }

        public void CancelAttributesCallback()
        {
            Object.Destroy(attributesDialog);
            attributeEditingMarker = null;
        }

        public void ApplyAttributesCallback(Attributes attributes)
        {
            attributeEditingMarker.attributes = new Attributes(attributes);
            Object.Destroy(attributesDialog);
            attributeEditingMarker = null;
        }

    }
}