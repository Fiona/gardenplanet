using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{

    public Texture2D mouseTexture;

    private Vector2 lastMousePosition = Vector2.zero;
    private Vector2 deltaMousePosition = Vector2.zero;
    private App app;

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
        if(Input.GetKeyUp(KeyCode.Space))
            controller.player.Jump();

    }

    public void UpdateEditor()
    {

        MapEditorController controller = FindObjectOfType<MapEditorController>();
        if(controller == null)
            return;

        // Get mouse over tiles
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);        
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
                    controller.tilemap.RotateTileInDirection(
                        currentTile,
                        (axis > 0.0f ? RotationalDirection.AntiClockwise : RotationalDirection.Clockwise)
                        );
                    controller.SetNewTileDirection(currentTile.direction);
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
                    
                    // Old fuked version
                    //pannedPosition = (initialMousePosition - Input.mousePosition) * .001f;
                    //if(Mathf.Abs(pannedPosition[0]) > 0.1f || Mathf.Abs(pannedPosition[1]) > 0.1f)
                    //    controller.PanCamera(pannedPosition[0], -pannedPosition[1]);

                    // New shiney version
                    controller.PanCamera(-Input.GetAxis("Mouse X") * .8f, -Input.GetAxis("Mouse Y") * .8f);
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
