using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Consts
{

    // Interal number for tile collision layer
    public const int COLLISION_LAYER_TILES = 8;

    // Tiles on different layers are placed this far apart
    public const float TILE_HEIGHT = 0.5f;

    /**
     * Editor related
     **/
    public const float MOUSE_BUMP_BORDER = 20.0f;
    public const float MOUSE_BUMP_SPEED = 500.0f;
    public const float CAMERA_Y = 6.0f;
}
