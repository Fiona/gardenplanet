using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace StrawberryNova
{
	
	public class InputManager : MonoBehaviour
	{

	    public Texture2D mouseTexture;

	    Vector2 lastMousePosition = Vector2.zero;
	    Vector2 deltaMousePosition = Vector2.zero;
	    App app;

	    public void Awake()
	    {
	        app = FindObjectOfType<App>();
	        SetMouseTexture(mouseTexture);
	        StartCoroutine(HandlePanning());
	    }

	    public void Update()
	    {
	        deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
	        lastMousePosition = Input.mousePosition;

	        if(app.state == AppState.Editor)
	            UpdateEditor();
	        else if(app.state == AppState.Game)
	            UpdateGame();
	    }

	    public void UpdateGame()
	    {

	        GameController controller = FindObjectOfType<GameController>();
	        if(controller == null)
	            return;
	        if(controller.player == null)
	            return;

	        // Walking
	        if(Input.GetKey(KeyCode.Comma))
	            controller.player.WalkInDirection(Direction.Up);
	        if(Input.GetKey(KeyCode.E))
	            controller.player.WalkInDirection(Direction.Right);
	        if(Input.GetKey(KeyCode.O))
	            controller.player.WalkInDirection(Direction.Down);
	        if(Input.GetKey(KeyCode.A))
	            controller.player.WalkInDirection(Direction.Left);

	        // Turning
	        Vector3 pointTo = Camera.main.ScreenToWorldPoint(
	            Input.mousePosition + new Vector3(0f, 0f, Consts.CAMERA_PLAYER_DISTANCE)
	            );
	        controller.player.TurnToWorldPoint(pointTo);

	        // Jumping
	        if(Input.GetKeyDown(KeyCode.Space))
	            controller.player.Jump();

			// Hovering mouse over objects
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);        
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_WORLD_OBJECTS))
			{
				var interactable = hit.transform.gameObject.GetComponent<WorldObjectInteractable>();
				if(interactable != null)
					interactable.Highlight();
			}

	    }

		bool movingWorldObject;

	    public void UpdateEditor()
	    {

	        MapEditorController controller = FindObjectOfType<MapEditorController>();
	        if(controller == null)
	            return;

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);        

			// If we're editing world objects and we want to move them
			if(controller.editorMode == EditorMode.WorldObject && controller.worldObjectMinorMode == 1)
			{
				// If an object is selected and we hit arrow keys this adjusts the height
				// Mouse wheel rotates the world object
				if(controller.worldObjectMoving != null)
				{
					var objTransform = controller.worldObjectMoving.gameObject.transform;
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

					var axis = Input.GetAxis("Mouse ScrollWheel");
					if(Mathf.Abs(axis) >= Consts.MOUSE_WHEEL_CLICK_SNAP)
					{
						var dir = (axis > 0.0f ? RotationalDirection.AntiClockwise : RotationalDirection.Clockwise);
						var worldObjectManager = FindObjectOfType<WorldObjectManager>();
						worldObjectManager.TurnWorldObjectInDirection(controller.worldObjectMoving, dir);						
					}
				}
			}

	        // Get mouse over tiles
	        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_TILES))
	        {
	            var currentTile = controller.tilemap.GetTileFromGameObject(hit.transform.gameObject);
	            if(currentTile != null)
	            {
	                if(currentTile.layer != controller.currentLayer)
	                {
	                    controller.tilemap.MouseOverTile(null);
	                    return;
	                }
	                controller.tilemap.MouseOverTile(currentTile);
	                var axis = Input.GetAxis("Mouse ScrollWheel");
	                if(Mathf.Abs(axis) >= Consts.MOUSE_WHEEL_CLICK_SNAP)
	                {
						var dir = (axis > 0.0f ? RotationalDirection.AntiClockwise : RotationalDirection.Clockwise);
						if(controller.editorMode == EditorMode.Tile)
						{
							controller.tilemap.RotateTileInDirection(currentTile, dir);
							controller.SetNewTileDirection(currentTile.direction);
						}
						else if(controller.editorMode == EditorMode.Marker)
						{
							var markerManager = FindObjectOfType<MarkerManager>();
							var marker = markerManager.GetMarkerAt(currentTile.x, currentTile.y, currentTile.layer);
							if(marker != null)
								markerManager.RotateMarkerInDirection(marker, dir);						
						}
	                }
	            }
	        }
	        else
	            controller.tilemap.MouseOverTile(null);

	    }

	    public IEnumerator HandlePanning()
	    {
	        if(app.state != AppState.Editor)
	            yield break;

	        MapEditorController controller = FindObjectOfType<MapEditorController>();

	        while(true)
	        {
	            if(Input.GetMouseButtonDown(2))
	            {
	                var initialMousePosition = Input.mousePosition;
	                var pannedPosition = Vector2.zero;
	                Cursor.lockState = CursorLockMode.Locked;
	                while(true)
	                {
	                    controller.PanCamera(-Input.GetAxis("Mouse X") * .5f, -Input.GetAxis("Mouse Y") * .5f);
	                    if (Input.GetMouseButtonUp(2))
	                    {
	                        Cursor.lockState = CursorLockMode.None;
	                        break;
	                    }
	                    yield return new WaitForFixedUpdate();
	                }
	            }
	            yield return new WaitForFixedUpdate();
	        }
	    }

	    public void SetMouseTexture(Texture2D texture)
	    {
	        Cursor.SetCursor(texture, new Vector2(1.0f, 1.0f), CursorMode.Auto);
	    }

	}

}