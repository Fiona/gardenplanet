using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using StompyBlondie.Common.Types;

namespace GardenPlanet
{
    public class MapEditorModeWorldObjects: MapEditorMode
    {

        WorldObjectManager worldObjectManager;

        GameObject currentWorldObjectPanel;
        Text currentWorldObjectText;
        GameObject currentWorldObjectTemplate;
        Button worldObjectMinorModeButton;

        WorldObjectType selectedWorldObject;
        GameObject currentWorldObjectSelectedObj;
        bool movingWorldObject;
        WorldObject worldObjectSelected;
        int worldObjectMinorMode;

        public override string GetModeName()
        {
            return "World Objects";
        }

        public override string GetGUIPrefabPath()
        {
            return Consts.PREFAB_PATH_EDITOR_MODE_WORLD_OBJECTS;
        }

        public override void Initialize()
        {
            base.Initialize();
            var worldObjectManagerObj = new GameObject("WorldObjectManager");
            worldObjectManager = worldObjectManagerObj.AddComponent<WorldObjectManager>();
            worldObjectManager.LoadFromMap(controller.map);

            currentWorldObjectTemplate.SetActive(false);
            SelectWorldObjectType(worldObjectManager.worldObjectTypes[0].name);
            SwitchWorldObjectMinorMode(0);
            while(selectedWorldObject.hideInEditor)
                NextWorldObjectButtonPressed();
        }

        public override void InitializeGUI()
        {
            base.InitializeGUI();

            currentWorldObjectPanel = guiHolder.transform.Find("CurrentWorldObjectPanel").gameObject;
            currentWorldObjectText =
                currentWorldObjectPanel.transform.Find("CurrentWorldObjectText").GetComponent<Text>();
            currentWorldObjectTemplate =
                currentWorldObjectPanel.transform.Find("CurrentWorldObjectTemplate").gameObject;
            worldObjectMinorModeButton =
                currentWorldObjectPanel.transform.Find("MinorModeButton").GetComponent<Button>();

            worldObjectMinorModeButton.onClick.AddListener(SwitchWorldObjectMinorModeButtonPressed);
            currentWorldObjectPanel.transform.Find("NextWorldObjectButton").GetComponent<Button>().onClick
                .AddListener(NextWorldObjectButtonPressed);
            currentWorldObjectPanel.transform.Find("PrevWorldObjectButton").GetComponent<Button>().onClick
                .AddListener(PreviousWorldObjectButtonPressed);
        }

        public override void Destroy()
        {
            base.Destroy();
            UnityEngine.Object.Destroy(worldObjectManager.gameObject);
        }

        public override void Update()
        {
            // If let go of mouse then we stop moving any world objects we've moving
            if(Input.GetMouseButtonUp(0) && movingWorldObject)
                movingWorldObject = false;

            // If we've hit delete and we're moving theeen... delete it
            if(worldObjectSelected != null && Input.GetKey(KeyCode.Delete))
            {
                DeleteWorldObject(worldObjectSelected);
                return;
            }

            // The rest of this pertains to a selected object
            if(worldObjectSelected == null)
                return;
            var objTransform = worldObjectSelected.gameObject.transform;

            // If we are moving an object
            if(movingWorldObject && controller.editorInputManager.mouseWorldPosition != null)
            {
                // Snap to grid
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || selectedWorldObject.tileObject)
                {
                    if(controller.currentHoveredTile != null)
                    {
                        objTransform.position = new Vector3(
                            controller.currentHoveredTile.tileObj.transform.position.x,
                            objTransform.position.y,
                            controller.currentHoveredTile.tileObj.transform.position.z
                        );
                    }
                }
                else
                {
                    objTransform.position = new Vector3(
                        controller.editorInputManager.mouseWorldPosition.Value.x,
                        objTransform.position.y,
                        controller.editorInputManager.mouseWorldPosition.Value.y
                    );
                }
                worldObjectSelected.x = objTransform.position.x;
                worldObjectSelected.y = objTransform.position.z;
                worldObjectSelected.height = objTransform.position.y;
            }

            // Move up and down if pressing arrow keys
            if(Input.GetKey(KeyCode.UpArrow))
                objTransform.position = new Vector3(
                    objTransform.position.x,
                    objTransform.position.y + 0.01f,
                    objTransform.position.z
                );
            if(Input.GetKey(KeyCode.DownArrow))
                objTransform.position = new Vector3(
                    objTransform.position.x,
                    objTransform.position.y - 0.01f,
                    objTransform.position.z
                );

            // Rotate object if moving mouse wheel
            var axis = Input.GetAxis("Mouse ScrollWheel");
            if(!(Mathf.Abs(axis) >= Consts.MOUSE_WHEEL_CLICK_SNAP))
                return;
            var dir = (axis > 0.0f ? RotationalDirection.AntiClockwise : RotationalDirection.Clockwise);
            worldObjectManager.TurnWorldObjectInDirection(worldObjectSelected, dir);
        }

        public override void Disable()
        {
            base.Disable();
            StopMovingWorldObject();
        }

        public override void TileLocationClicked(TilePosition tilePos, PointerEventData pointerEventData)
        {
            // Wanting to add world objects
            if(worldObjectMinorMode == 0)
            {
                if(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.RightShift) || selectedWorldObject.tileObject)
                    worldObjectManager.AddWorldObject(selectedWorldObject, tilePos);
                else
                {
                    // Without shift being held we don't snap to grid
                    if(controller.editorInputManager.mouseWorldPosition != null)
                    {
                        var worldPos = new WorldPosition{
                            x=controller.editorInputManager.mouseWorldPosition.Value.x,
                            y=controller.editorInputManager.mouseWorldPosition.Value.y,
                            height=(tilePos.layer * Consts.TILE_SIZE)
                        };
                        worldObjectManager.AddWorldObject(selectedWorldObject, worldPos);
                    }
                }
            }
            // If moving world objects
            else if(worldObjectMinorMode == 1)
            {
                // First check to see if we've clicked on a world object, this switches the selection
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(pointerEventData.position);
                if(!movingWorldObject && Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_WORLD_OBJECTS))
                {
                    var worldObject = worldObjectManager.GetWorldObjectByGameObject(hit.transform.gameObject);
                    if(worldObject != null && worldObject != worldObjectSelected)
                    {
                        WorldObjectMinorModeStartMovingWorldObject(worldObject);
                        return;
                    }
                }
                // If we're not switching selection and we already have one then holding
                // down the mouse will move the object
                if(!movingWorldObject)
                    movingWorldObject = true;
            }
        }

        public override void SaveToMap(Map map)
        {
            worldObjectManager.SaveToMap(map);
        }

        public override void ResizeMap(int width, int height)
        {
        }

        /*
         * Use to switch to a minor mode in objects
         */
        public void SwitchWorldObjectMinorMode(int newMinorMode)
        {
            worldObjectMinorMode = newMinorMode;
            if(worldObjectMinorMode == 1)
                worldObjectMinorModeButton.GetComponentInChildren<Text>().text = "Moving";
            else
            {
                StopMovingWorldObject();
                worldObjectMinorModeButton.GetComponentInChildren<Text>().text = "Adding";
            }
        }

        public void StopMovingWorldObject()
        {
            if(worldObjectSelected != null)
            {
                var objRenderer = worldObjectSelected.gameObject.GetComponentInChildren<Renderer>();
                var matProp = new MaterialPropertyBlock();
                matProp.SetColor("_Color", new Color(1f, 1f, 1f));
                objRenderer.SetPropertyBlock(matProp);
            }
            worldObjectSelected = null;
        }

        public void WorldObjectMinorModeStartMovingWorldObject(WorldObject worldObject)
        {
            StopMovingWorldObject();
            var objRenderer = worldObject.gameObject.GetComponentInChildren<Renderer>();
            var matProp = new MaterialPropertyBlock();
            matProp.SetColor("_Color", new Color(0f, 1f, 1f));
            objRenderer.SetPropertyBlock(matProp);
            worldObjectSelected = worldObject;
        }

        /*
         * World object minor mode button pressed
         */
        public void SwitchWorldObjectMinorModeButtonPressed()
        {
            SwitchWorldObjectMinorMode(Math.Abs(worldObjectMinorMode - 1));
        }

        /*
          Pressed button to go to the next world object
         */
        public void NextWorldObjectButtonPressed()
        {
            if(!SelectWorldObjectType(worldObjectManager.GetNextWorldObjectMarkerType(selectedWorldObject).name))
                NextWorldObjectButtonPressed();
        }

        /*
          Pressed button to go to the previous world object
         */
        public void PreviousWorldObjectButtonPressed()
        {
            if(!SelectWorldObjectType(worldObjectManager.GetPreviousWorldObjectMarkerType(selectedWorldObject).name))
                PreviousWorldObjectButtonPressed();
        }

        /*
         * Switches selected world object type
         */
        private bool SelectWorldObjectType(string worldObjectTypeName)
        {

            if(currentWorldObjectSelectedObj != null)
            {
                UnityEngine.Object.Destroy(currentWorldObjectSelectedObj);
                currentWorldObjectSelectedObj = null;
            }

            if(worldObjectTypeName == null)
            {
                currentWorldObjectText.text = "None";
                return true;
            }

            selectedWorldObject = worldObjectManager.GetWorldObjectTypeByName(worldObjectTypeName);
            if(selectedWorldObject.hideInEditor)
                return false;
            currentWorldObjectSelectedObj = UnityEngine.Object.Instantiate(selectedWorldObject.prefab);

            currentWorldObjectSelectedObj.transform.parent = currentWorldObjectPanel.transform;
            currentWorldObjectSelectedObj.AddComponent<RectTransform>();
            var rect = currentWorldObjectSelectedObj.GetComponent<RectTransform>();

            // Base values on template
            currentWorldObjectSelectedObj.layer = currentWorldObjectTemplate.layer;
            rect.localPosition = currentWorldObjectTemplate.GetComponent<RectTransform>().localPosition;
            rect.localScale = currentWorldObjectTemplate.GetComponent<RectTransform>().localScale;
            rect.localRotation = currentWorldObjectTemplate.GetComponent<RectTransform>().localRotation;

            currentWorldObjectText.text = selectedWorldObject.name;

            return true;
        }

        /*
         * Used when getting rid of a world object
         */
        public void DeleteWorldObject(WorldObject worldObjectToDelete)
        {
            if(worldObjectToDelete == worldObjectSelected)
                StopMovingWorldObject();
            worldObjectManager.DeleteWorldObject(worldObjectToDelete);
        }

    }
}