using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	    public Dropdown newVolumeTypeDropdown;
	    public Toggle isWallToggle;
        public GameObject gridLine;
        public GameObject tileCentreMark;

	    private TileType tileType;
	    private TileTypeVolume selectedVolume;
	    private bool close;
	    private MapEditorController controller;
	    private GameObject currentTilePreviewObject;
	    private GameObject currentVolumeObject;
	    private float rotation;
        private bool recreateTilesOfType;

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

	        // Set dropdown options
	        newVolumeTypeDropdown.ClearOptions();
	        var options = new List<Dropdown.OptionData>();
	        foreach(TileTypeVolumeType volumeType in Enum.GetValues(typeof(TileTypeVolumeType)))
	        {
	            options.Add(
	                new Dropdown.OptionData()
	                {
	                    text = GetVolumeTypeName(volumeType)
	                }
	            );
	        }
	        newVolumeTypeDropdown.AddOptions(options);

	        // Create volume list buttons
	        RecreateVolumeList();

	        // Set up other elements
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
	            newButton.GetComponentInChildren<Text>().text = GetVolumeTypeName(volume.type);
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
	            isWallToggle.isOn = volume.isWall;
	            currentVolumeObject = Instantiate(GetVolumeTypeTemplate(selectedVolume.type));
	            currentVolumeObject.SetActive(true);
	            currentVolumeObject.transform.SetParent(currentTilePreviewObject.transform, false);
	            PositionAndScaleVolume();
	        }
	    }

	    public string GetVolumeTypeName(TileTypeVolumeType type)
	    {
	        switch(type)
	        {
	            case TileTypeVolumeType.CollisionBox:
	                return "Collision Box";
	            case TileTypeVolumeType.CollisionPlane:
	                return "Collision Plane";
	            default:
	                return "Unknown volume";
	        }
	    }

	    public GameObject GetVolumeTypeTemplate(TileTypeVolumeType type)
	    {
	        switch(type)
	        {
	            case TileTypeVolumeType.CollisionBox:
	                return volumeCubeTemplate;
	            case TileTypeVolumeType.CollisionPlane:
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
	        if(selectedVolume.type == TileTypeVolumeType.CollisionPlane)
	            currentVolumeObject.transform.localScale = new Vector3(
	                currentVolumeObject.transform.localScale.x * 0.1f,
	                currentVolumeObject.transform.localScale.y * 0.1f,
	                currentVolumeObject.transform.localScale.z * 0.1f
	                );
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
	        var newVolume = new TileTypeVolume((TileTypeVolumeType)newVolumeTypeDropdown.value);
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

	    public void IsWallToggled()
	    {
	        if (selectedVolume == null)
	            return;
	        selectedVolume.isWall = isWallToggle.isOn;        
	    }

	    public void RotateTilePreviewPressed()
	    {			
			rotation = (rotation + 90f) % 360.0f;
	    }

        public void XCentreMinusPressed(){ tileType.xCentre -= Consts.TILE_SIZE/2; }
        public void XCentrePlusPressed(){ tileType.xCentre += Consts.TILE_SIZE/2; }
        public void YCentreMinusPressed(){ tileType.yCentre -= Consts.TILE_SIZE/2; }
        public void YCentrePlusPressed(){ tileType.yCentre += Consts.TILE_SIZE/2; }
        public void ZCentreMinusPressed(){ tileType.zCentre -= Consts.TILE_SIZE/2; }
        public void ZCentrePlusPressed(){ tileType.zCentre += Consts.TILE_SIZE/2; }

	}

}