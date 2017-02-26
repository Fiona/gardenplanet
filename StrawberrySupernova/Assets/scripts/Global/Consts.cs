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

    // All tiles are scaled by this amount in an attempt to tackle gaps
    public const float SCALE_FUDGE = 0.01f;

    // Any mouse wheel axis movements at or outside of this movement will
    // class as a click in that direction.
    public const float MOUSE_WHEEL_CLICK_SNAP = 0.05f;

    /**
     * Editor related
     **/
    public const float MOUSE_BUMP_BORDER = 40.0f;
    public const float MOUSE_BUMP_SPEED = 500.0f;
    public const float CAMERA_Y = 6.0f;
    public const float VERTICAL_EDGE_DISTANCE = 5.0f;
}

public enum Direction{Left, Right, Up, Down};
public enum RotationalDirection{Clockwise, AntiClockwise};
