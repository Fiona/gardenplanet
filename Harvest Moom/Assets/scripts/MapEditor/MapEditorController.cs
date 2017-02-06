using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapEditorController : MonoBehaviour
{

    public int currentLayer;
    public Tilemap tilemap;
    public Camera camera;
    public Text layerText;

    public void Awake()
    {

        for(int x = 0; x <= 10; x++)
        {
            for(int y = 0; y <= 10; y++)
            {
                tilemap.addTile(((UnityEngine.Random.RandomRange(0.0f, 1.0f) > .5f) ? "grass02" : "grass01"), x, y, 0);
            }
        }

        switchToLayer(0);

    }

    public void Update()
    {
        // Do bump scrolling
        var body = camera.GetComponent<Rigidbody>();
        Vector3? direction = null;

        if(Input.mousePosition[0] < Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.left;
        if(Input.mousePosition[0] > Screen.width - Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.right;
        if(Input.mousePosition[1] < Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.back;
        if(Input.mousePosition[1] > Screen.height - Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.forward;

        if(direction != null)
            body.AddForce(((Vector3)direction) * Consts.MOUSE_BUMP_SPEED * Time.deltaTime);
    }

    /*
      Switches to the specified map layer
     */
    public void switchToLayer(int layer)
    {
        currentLayer = layer;
        layerText.text = String.Format("{0}", layer);
        float y = Consts.CAMERA_Y + (Consts.TILE_HEIGHT * layer);
        camera.transform.position = new Vector3(
            camera.transform.position.x,
            y,
            camera.transform.position.z);
    }

    /*
      Pressed button that makes Z layer go up
     */
    public void LayerUpButtonPressed()
    {
        switchToLayer(currentLayer + 1);
    }

    /*
      Pressed button that makes Z layer go down
     */
    public void LayerDownButtonPressed()
    {
        switchToLayer(currentLayer - 1);
    }

}
