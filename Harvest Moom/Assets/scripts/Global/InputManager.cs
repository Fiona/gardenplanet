using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{

    public Texture2D mouseTexture;
    public Tilemap tilemap;

    public void Awake()
    {
        Cursor.SetCursor(mouseTexture, new Vector2(1.0f, 1.0f), CursorMode.Auto);
    }

    public void Update()
    {

        // Get mouse over tiles
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, 100.0f, 1 << Consts.COLLISION_LAYER_TILES))
        {
            tilemap.mouseOverTile(hit.transform.gameObject);
        }

    }

}
