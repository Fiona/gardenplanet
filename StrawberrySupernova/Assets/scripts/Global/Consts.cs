using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace StrawberryNova
{
	
	public static class Consts
	{

	    // -----------------------------
	    // CONFIG SETTINGS
	    // -----------------------------

	    // Where to boot the app into
	    public const AppState INITIAL_APP_STATE = AppState.Title;

	    // -----------------------------
	    // TILE DRAWING
	    // -----------------------------

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

	    // -----------------------------
	    // INPUT
	    // -----------------------------

	    // Any mouse wheel axis movements at or outside of this movement will
	    // class as a click in that direction.
	    public const float MOUSE_WHEEL_CLICK_SNAP = 0.05f;

	    // -----------------------------
	    // CAMERA
	    // -----------------------------

	    // How far away the camera usually is from the player
	    public const float CAMERA_PLAYER_DISTANCE = 4.0f;

	    // How far from the ground the rotation usually is
	    public const float CAMERA_DEFAULT_ROTATION = 45.0f;

	    // -----------------------------
	    // PLAYER
	    // -----------------------------

	    public const float PLAYER_SPEED = 100f;

	    public const float PLAYER_ROTATION_SPEED = 4f;

	    public const float PLAYER_JUMP_FORCE = 450f;

	    // -----------------------------
	    // FILESYSTEM
	    // -----------------------------

	    public const string FILE_DEFAULT_TILE_SET = "tiles";
	    public const string DATA_DIR = "vault";
	    public const string DATA_DIR_MAPS = "maps";
	    public const string FILE_EXTENSION_MAP = "map";
	    public const string FILE_EXTENSION_TILE_SET = "set";

	    // -----------------------------
	    // EDITOR
	    // -----------------------------

	    public const float MOUSE_BUMP_BORDER = 40.0f;
	    public const float MOUSE_BUMP_SPEED = 500.0f;
	    public const float CAMERA_Y = 6.0f;
	    public const float VERTICAL_EDGE_DISTANCE = 5.0f;

	}
		
}