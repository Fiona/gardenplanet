using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{

    public Texture2D mouseTexture;

    private MapEditorController controller;
    private Vector2 lastMousePosition = Vector2.zero;
    private Vector2 deltaMousePosition = Vector2.zero;

    public void Awake()
    {
        controller = FindObjectOfType<MapEditorController>();
        SetMouseTexture(mouseTexture);
        StartCoroutine(HandlePanning());
    }

    public void Update()
    {

        deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        // Get mouse over tiles
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_TILES))
            controller.tilemap.MouseOverTile(hit.transform.gameObject);
        else
            controller.tilemap.MouseOverTile(null);

    }

    public IEnumerator HandlePanning()
    {
        while(true)
        {
            if(Input.GetMouseButtonDown(2))
            {
                var initialMousePosition = Input.mousePosition;
                var pannedPosition = Vector2.zero;
                Cursor.lockState = CursorLockMode.Locked;
                while(true)
                {
                    pannedPosition = (initialMousePosition - Input.mousePosition) * .001f;
                    if(Mathf.Abs(pannedPosition[0]) > 0.1f || Mathf.Abs(pannedPosition[1]) > 0.1f)
                        controller.PanCamera(pannedPosition[0], -pannedPosition[1]);
                    if(Input.GetMouseButtonUp(2))
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
