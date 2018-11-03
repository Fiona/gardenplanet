using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace GardenPlanet
{

    public class EditTileDialog : MonoBehaviour
    {

        public GameObject tilePreviewTemplate;
        public GameObject tilePreviewHolder;
        public Text tileNameText;
        public GameObject volumeSettings;
        public GameObject volumeCubeTemplate;
        public GameObject volumePlaneTemplate;
        public Button volumeListButtonTemplate;
        public GameObject volumeListContent;
        public Dropdown newVolumeShapeDropdown;
        public Dropdown volumeSurfaceDropdown;
        public GameObject gridLine;
        public GameObject tileCentreMark;
        public Dropdown autoTileTagDropdown;
        public NavigationMapDebugRenderer navigationMapDebugRenderer;
        public InputField tileSizeXInput;
        public InputField tileSizeYInput;

        private TileType tileType;
        private TileTypeVolume selectedVolume;
        private bool close;
        private MapEditorController controller;
        private GameObject currentTilePreviewObject;
        private GameObject currentVolumeObject;
        private float rotation;
        private bool recreateTilesOfType;
        private List<TileTagType> tileTagTypes;
        private bool showNav;

        public void Awake()
        {
            tilePreviewTemplate.SetActive(false);
            volumeListButtonTemplate.gameObject.SetActive(false);
            CreateGridLines();
        }

        public void CreateGridLines()
        {
            gridLine.SetActive(false);

            // Horizontal
            for(var gridPos = -Consts.TILE_SIZE*2; gridPos <= Consts.TILE_SIZE*2; gridPos += Consts.TILE_SIZE)
            {
                var newGridLine = Instantiate(gridLine);
                newGridLine.transform.SetParent(gridLine.transform.parent);
                newGridLine.transform.localPosition = gridLine.transform.localPosition + new Vector3(gridPos, 0f, 0f);
                newGridLine.transform.localRotation = gridLine.transform.localRotation;
                newGridLine.transform.localScale = gridLine.transform.localScale;
                newGridLine.SetActive(true);
            }
            // Vertical
            for(var gridPos = -Consts.TILE_SIZE*2; gridPos <= Consts.TILE_SIZE*2; gridPos += Consts.TILE_SIZE)
            {
                var newGridLine = Instantiate(gridLine);
                newGridLine.transform.SetParent(gridLine.transform.parent);
                newGridLine.transform.localPosition = gridLine.transform.localPosition + new Vector3(0f, 0f, gridPos);
                newGridLine.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                newGridLine.transform.localScale = gridLine.transform.localScale;
                newGridLine.SetActive(true);
            }
        }

        public void Update()
        {
            SetTilePreviewRotation();
            SetTileCentreMark();
            CalculateNavPoints();
        }

        public void SetTileCentreMark()
        {
            tileCentreMark.transform.localPosition = new Vector3(
                (float)-tileType.xCentre,
                (float)-tileType.yCentre,
                (float)-tileType.zCentre
            );
        }

        public IEnumerator Show(string tileTypeName)
        {

            controller = (MapEditorController)FindObjectOfType(typeof(MapEditorController));
            rotation = 0f;

            try
            {
                this.tileType = StompyBlondie.DeepClone.Clone<TileType>(
                    controller.tileTypeSet.GetTileTypeByName(tileTypeName)
                );
            }
            catch(EditorErrorException)
            {
                yield break;
            }

            gameObject.SetActive(true);

            Destroy(currentTilePreviewObject);
            currentTilePreviewObject = controller.tileTypeSet.InstantiateTile(tileType);

            currentTilePreviewObject.transform.parent = tilePreviewHolder.transform;
            currentTilePreviewObject.AddComponent<RectTransform>();
            var rect = currentTilePreviewObject.GetComponent<RectTransform>();

            // Base values on template
            currentTilePreviewObject.layer = tilePreviewTemplate.layer;
            rect.localPosition = tilePreviewTemplate.GetComponent<RectTransform>().localPosition;
            rect.localScale = tilePreviewTemplate.GetComponent<RectTransform>().localScale;
            rect.localRotation = tilePreviewTemplate.GetComponent<RectTransform>().localRotation;

            // Set volume dropdown options
            newVolumeShapeDropdown.ClearOptions();
            var options = new List<Dropdown.OptionData>();
            foreach(TileTypeVolumeShape volumeType in Enum.GetValues(typeof(TileTypeVolumeShape)))
            {
                options.Add(
                    new Dropdown.OptionData()
                    {
                        text = GetVolumeTypeName(volumeType)
                    }
                );
            }
            newVolumeShapeDropdown.AddOptions(options);

            // Set tag dropdown options
            tileTagTypes = TileTagType.GetAllTileTagTypes();
            autoTileTagDropdown.ClearOptions();
            var typeOptions = new List<Dropdown.OptionData>(){new Dropdown.OptionData(){ text = "" }};
            var current = 0;
            foreach(var _tag in tileTagTypes)
            {
                if(_tag.ID == tileType.autoTag)
                    current = typeOptions.Count;
                typeOptions.Add(new Dropdown.OptionData() {text = _tag.ID});
            }
            autoTileTagDropdown.AddOptions(typeOptions);
            autoTileTagDropdown.value = current;

            // Set volume surface dropdown options
            volumeSurfaceDropdown.ClearOptions();
            options = new List<Dropdown.OptionData>();
            foreach(TileTypeVolumeSurface volumeSurface in Enum.GetValues(typeof(TileTypeVolumeSurface)))
            {
                options.Add(
                    new Dropdown.OptionData()
                    {
                        text = volumeSurface.ToString()
                    }
                );
            }
            volumeSurfaceDropdown.AddOptions(options);

            // Create volume list buttons
            RecreateVolumeList();

            // Set up other elements
            tileSizeXInput.text = tileType.size[0].ToString();
            tileSizeYInput.text = tileType.size[1].ToString();
            CalculateNavPoints();
            tileNameText.text = tileTypeName;
            volumeSettings.SetActive(false);

            // Wait to be told to close
            recreateTilesOfType = false;
            close = false;
            while(!close)
                yield return new WaitForFixedUpdate();
            gameObject.SetActive(false);

            // Update tiles
            if(recreateTilesOfType)
            {
                var tilePosToUpdate = new List<TilePosition>();
                foreach(var tile in controller.tilemap.tilemap)
                {
                    if(tile.tileTypeName == tileTypeName)
                    {
                        tilePosToUpdate.Add(
                            new TilePosition{x = tile.x, y = tile.y, layer = tile.layer, dir = tile.direction}
                        );
                    }
                }
                foreach(var tilePos in tilePosToUpdate)
                {
                    controller.tilemap.RemoveTile(tilePos.x, tilePos.y, tilePos.layer);
                    controller.tilemap.AddTile(tileTypeName, tilePos.x, tilePos.y, tilePos.layer, tilePos.dir);
                }
            }
        }

        public void RecreateVolumeList()
        {
            foreach(Transform button in volumeListContent.transform)
                if(button.gameObject.activeSelf)
                    Destroy(button.gameObject);

            foreach(var volume in tileType.volumes)
            {
                var newButton = Instantiate(volumeListButtonTemplate);
                newButton.gameObject.SetActive(true);
                newButton.GetComponentInChildren<Text>().text = GetVolumeTypeName(volume.shape);
                newButton.transform.SetParent(volumeListContent.transform, false);
                newButton.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        SelectNewVolume(volume);
                    }
                );
            }
        }

        public void SetTilePreviewRotation()
        {
            if(tilePreviewHolder == null)
                return;
            tilePreviewHolder.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, rotation, 0);
        }

        public void SelectNewVolume(TileTypeVolume volume)
        {
            selectedVolume = volume;
            Destroy(currentVolumeObject);
            if(volume == null)
            {
                volumeSettings.SetActive(false);
                currentVolumeObject = null;
            }
            else
            {
                volumeSettings.SetActive(true);
                volumeSurfaceDropdown.value = (int)selectedVolume.surface;
                currentVolumeObject = Instantiate(GetVolumeTypeTemplate(selectedVolume.shape));
                currentVolumeObject.SetActive(true);
                currentVolumeObject.transform.SetParent(currentTilePreviewObject.transform, false);
                PositionAndScaleVolume();
            }
        }

        public string GetVolumeTypeName(TileTypeVolumeShape type)
        {
            switch(type)
            {
                case TileTypeVolumeShape.CollisionBox:
                    return "Collision Box";
                case TileTypeVolumeShape.CollisionPlane:
                    return "Collision Plane";
                default:
                    return "Unknown volume";
            }
        }

        public GameObject GetVolumeTypeTemplate(TileTypeVolumeShape type)
        {
            switch(type)
            {
                case TileTypeVolumeShape.CollisionBox:
                    return volumeCubeTemplate;
                case TileTypeVolumeShape.CollisionPlane:
                    return volumePlaneTemplate;
                default:
                    return null;
            }
        }

        public void PositionAndScaleVolume()
        {
            if(currentVolumeObject == null)
                return;
            currentVolumeObject.transform.localPosition = new Vector3(
                Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)selectedVolume.x,
                Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)selectedVolume.y,
                Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)selectedVolume.z
            );
            currentVolumeObject.transform.localScale = new Vector3(
                ((Consts.VOLUME_SCALE_DEFAULT / 100.0f) * selectedVolume.xScale),// / .01f,
                ((Consts.VOLUME_SCALE_DEFAULT / 100.0f) * selectedVolume.yScale),// / .01f,
                ((Consts.VOLUME_SCALE_DEFAULT / 100.0f) * selectedVolume.zScale)// / .01f
            );
            if(selectedVolume.shape == TileTypeVolumeShape.CollisionPlane)
                currentVolumeObject.transform.localScale = new Vector3(
                    currentVolumeObject.transform.localScale.x * 0.1f,
                    currentVolumeObject.transform.localScale.y * 0.1f,
                    currentVolumeObject.transform.localScale.z * 0.1f
                );
            CalculateNavPoints();
        }

        public void CancelPressed()
        {
            close = true;
        }

        public void ApplyPressed()
        {
            try
            {
                controller.tileTypeSet.ReplaceTileType(tileType.name, tileType);
                controller.tileTypeSet.SaveTileTypeSet();
            }
            catch(EditorErrorException){ }
            close = true;
            recreateTilesOfType = true;
        }

        public void CreateNewVolumePressed()
        {
            var newVolume = new TileTypeVolume((TileTypeVolumeShape)newVolumeShapeDropdown.value);
            tileType.volumes.Add(newVolume);
            RecreateVolumeList();
            SelectNewVolume(newVolume);
        }

        public void DeleteVolumePressed()
        {
            if(selectedVolume == null)
                return;
            tileType.volumes.Remove(selectedVolume);
            RecreateVolumeList();
            SelectNewVolume(null);
        }

        public void VolumePosXMinusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.x -= 1;
            PositionAndScaleVolume();
        }

        public void VolumePosXPlusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.x += 1;
            PositionAndScaleVolume();
        }

        public void VolumePosYMinusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.y -= 1;
            PositionAndScaleVolume();
        }

        public void VolumePosYPlusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.y += 1;
            PositionAndScaleVolume();
        }

        public void VolumePosZMinusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.z -= 1;
            PositionAndScaleVolume();
        }

        public void VolumePosZPlusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.z += 1;
            PositionAndScaleVolume();
        }

        public void VolumeXScaleMinusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.xScale -= 10;
            PositionAndScaleVolume();
        }

        public void VolumeXScalePlusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.xScale += 10;
            PositionAndScaleVolume();
        }

        public void VolumeYScaleMinusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.yScale -= 10;
            PositionAndScaleVolume();
        }

        public void VolumeYScalePlusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.yScale += 10;
            PositionAndScaleVolume();
        }

        public void VolumeZScaleMinusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.zScale -= 10;
            PositionAndScaleVolume();
        }

        public void VolumeZScalePlusPressed()
        {
            if(selectedVolume == null)
                return;
            selectedVolume.zScale += 10;
            PositionAndScaleVolume();
        }

        public void RotateTilePreviewPressed()
        {
            rotation = (rotation + 90f) % 360.0f;
        }

        public void ShowNavPressed()
        {
            showNav = !showNav;
        }

        public void XCentreMinusPressed(){ tileType.xCentre -= Consts.TILE_SIZE/2; }
        public void XCentrePlusPressed(){ tileType.xCentre += Consts.TILE_SIZE/2; }
        public void YCentreMinusPressed(){ tileType.yCentre -= Consts.TILE_SIZE/2; }
        public void YCentrePlusPressed(){ tileType.yCentre += Consts.TILE_SIZE/2; }
        public void ZCentreMinusPressed(){ tileType.zCentre -= Consts.TILE_SIZE/2; }
        public void ZCentrePlusPressed(){ tileType.zCentre += Consts.TILE_SIZE/2; }

        public void VolumeSurfaceDropdownChanged(int index)
        {
            if(selectedVolume == null)
                return;
            selectedVolume.surface = (TileTypeVolumeSurface)index;
        }

        public void AutoTagTileDropdownChanged(int index)
        {
            tileType.autoTag = index == 0 ? "" : tileTagTypes[index-1].ID;
        }

        private void CalculateNavPoints()
        {
            tileType.navigationMap.Reset();
            var halfTileSize = Consts.TILE_SIZE/2;

            for(var tileX = 0; tileX < tileType.size[0]; tileX++)
            {
                for(var tileY = 0; tileY < tileType.size[1]; tileY++)
                {
                    for(var x = -halfTileSize; x <= halfTileSize; x += halfTileSize)
                    {
                        for(var y = -halfTileSize; y <= halfTileSize; y += halfTileSize)
                        {
                            tileType.navigationMap.AddPoint(
                                new Pos(x + (tileX * Consts.TILE_SIZE), y + (tileY * Consts.TILE_SIZE), 0f)
                            );
                        }
                    }
                }
            }

            navigationMapDebugRenderer.offset = new Vector3(
                (float)tileType.xCentre,
                (float)tileType.zCentre,
                (float)-tileType.yCentre
            );
            navigationMapDebugRenderer.navigationMap = tileType.navigationMap;
            navigationMapDebugRenderer.gameObject.SetActive(showNav);
        }

        public void TileSizeXChanged(string newValue)
        {
            tileType.size[0] = Int32.Parse(newValue);
        }

        public void TileSizeYChanged(string newValue)
        {
            tileType.size[1] = Int32.Parse(newValue);
        }
    }

}