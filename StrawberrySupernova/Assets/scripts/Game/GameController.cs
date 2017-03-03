using System;
using UnityEngine;

class GameController: MonoBehaviour
{

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
    }

}
