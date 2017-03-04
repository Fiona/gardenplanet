using System;
using UnityEngine;

public class GameController: MonoBehaviour
{

    [Header("Object References")]
    public PlayerCamera mainCamera;
    public Player player;

    [HideInInspector]
    public Tilemap tilemap;
    [HideInInspector]
    public TileTypeSet tileTypeSet;
    [HideInInspector]
    public Map map;

    public void Awake()
    {
        tileTypeSet = new TileTypeSet("default");
        map = new Map("main");

        var tilemapObj = new GameObject("Tilemap");
        tilemap = tilemapObj.AddComponent<Tilemap>();
        tilemap.LoadFromMap(map, tileTypeSet);

        mainCamera.SetTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE, 5.0f);
    }

}
