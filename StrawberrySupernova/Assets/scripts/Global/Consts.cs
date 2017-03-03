using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Consts
{

    // Where to boot the app into
    public const AppState INITIAL_APP_STATE = AppState.Title;

    // Interal number for tile collision layer
    public const int COLLISION_LAYER_TILES = 8;

    // Tiles on different layers are placed this far apart
    public const float TILE_HEIGHT = 0.5f;

    // All tiles are scaled by this amount in an attempt to tackle gaps
    public const float SCALE_FUDGE = 0.01f;

    // Value used to adjust positions of tile volumes
    public const float VOLUME_POSITION_SHIFT_PER_UNIT = 0.00250f;

    // Default scale of tile volumes
    public const float VOLUME_SCALE_DEFAULT = 0.01f;

    // Any mouse wheel axis movements at or outside of this movement will
    // class as a click in that direction.
    public const float MOUSE_WHEEL_CLICK_SNAP = 0.05f;

    // File system related consts
    public const string FILE_DEFAULT_TILE_SET = "tiles";
    public const string DATA_DIR = "vault";
    public const string DATA_DIR_MAPS = "maps";
    public const string FILE_EXTENSION_MAP = "map";
    public const string FILE_EXTENSION_TILE_SET = "set";

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
public enum EditorMessageType{Good, Bad, Meh};
public enum TileTypeVolumeType{CollisionPlane, CollisionBox};
public enum AppState{Title, Editor, Game};
