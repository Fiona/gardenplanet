using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace StrawberryNova
{

	public static class Consts
	{

		// TODO: Make these player options
		public const bool PLAYER_SETTING_STATIC_INFO_POPUP = false;

	    // -----------------------------
	    // CONFIG SETTINGS
	    // -----------------------------

	    // Where to boot the app into
	    public const AppState INITIAL_APP_STATE = AppState.Title;

	    // -----------------------------
	    // TILE DRAWING
	    // -----------------------------

		// Internal collision layer numbers
		public const int COLLISION_LAYER_TILES = 8;
		public const int COLLISION_LAYER_WORLD_OBJECTS = 9;
        public const int COLLISION_LAYER_MOUSE_HOVER_PLANE = 10;
        public const int COLLISION_LAYER_ITEMS = 13;

	    // Tiles are this size on all dimensions
	    public const float TILE_SIZE = 0.5f;

	    // All tiles are scaled by this amount in an attempt to tackle gaps
		public const float SCALE_FUDGE = 0.00001f;

	    // Value used to adjust positions of tile volumes
		public const float VOLUME_POSITION_SHIFT_PER_UNIT = 0.250f;

	    // Default scale of tile volumes
	    public const float VOLUME_SCALE_DEFAULT = 1f;

	    // -----------------------------
	    // INPUT
	    // -----------------------------

		// Rewired player ID
		public const int REWIRED_PLAYER_ID = 0;

	    // Any mouse wheel axis movements at or outside of this movement will
	    // class as a click in that direction.
	    public const float MOUSE_WHEEL_CLICK_SNAP = 0.1f;

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

	    public const float PLAYER_ROTATION_SPEED = 10f;

	    public const float PLAYER_JUMP_FORCE = 800f;

		public const float PLAYER_INTERACT_DISTANCE = .75f;
		public const float PLAYER_PICKUP_DISTANCE = .50f;

        // What hour the player wakes up at
		public const int PLAYER_WAKE_HOUR = 6;

		// What hour the player passes out at and wakes up at
		public const int PLAYER_PASS_OUT_HOUR = 2;
		public const int PLAYER_PASS_OUT_WAKE_HOUR = 9;

        // How many stacks of items the player can have at once
        public const int PLAYER_INVENTORY_MAX_STACKS = 20;

		// How many tiles away the player can use tools from
		public const int PLAYER_TOOLS_RANGE = 2;

		// How much energy the player starts with
		public const float PLAYER_START_ENERGY = 5.0f;

		// How high the player's energy can go
		public const float PLAYER_MAX_ENERGY = 10.0f;

		// -----------------------------
		// GAMEPLAY
		// -----------------------------

		public const int GAME_START_YEAR = 1;

		// How many seconds it takes for the in-world timer to go forward a minute
		public const float SECONDS_PER_GAME_MINUTE = 1f;

		// Game time settings
		public const int NUM_MINUTES_IN_HOUR = 60;
		public const int NUM_HOURS_IN_DAY = 24;
		public const int NUM_DAYS_IN_SEASON = 28;
		public readonly static string[] SEASONS = {"Spring", "Summer", "Autumn", "Winter"};
		public readonly static string[] WEEKDAYS = {
			"Monday", "Tuesday", "Wednesday", "Thursday",
			"Friday", "Saturday", "Sunday"
		};

        public const int HOTBAR_SIZE = 10;

		public const int CROP_GROWTH_HOUR = 3;

		public const float CROP_DAMAGE_PER_HOE_HIT = 25f;

        // -----------------------------
        // GUI
        // -----------------------------

        public const float GUI_IN_GAME_MENU_FADE_TIME = .2f;
		public const float GUI_IN_GAME_MENU_PAGE_FADE_TIME = .2f;
		public const float GUI_ENERGY_PER_HEART = 1.0f;

		// -----------------------------
		// TILE TAGS
		// -----------------------------

		public const string TILE_TAG_FARM = "Farm";

		// -----------------------------
		// RESOURCE PATHS
		// -----------------------------

        public const string PREFAB_PATH_WORLD_TIMER = "prefabs/gui/WorldTimer";
		public const string PREFAB_PATH_ITEM_HOTBAR = "prefabs/gui/ItemHotbar";
        public const string PREFAB_PATH_ATMOSPHERE = "prefabs/world/Atmosphere";
		public const string PREFAB_PATH_INFO_POPUP = "prefabs/gui/InfoPopup";
        public const string PREFAB_PATH_IN_GAME_MENU = "prefabs/gui/InGameMenu";
		public const string PREFAB_PATH_PLAYER_ENERGY = "prefabs/gui/PlayerEnergy";
        public const string TEXTURE_PATH_GUI_MOUSE = "textures/gui/mouse";
		public const string TILES_PREFABS_PATH = "prefabs/world/tiles/";
		public const string WORLD_OBJECTS_PREFABS_PATH = "prefabs/world/worldobjects/";
		public const string ITEMS_PREFABS_PATH = "prefabs/world/items/";
		public const string ITEMS_PREFAB_MISSING = "prefabs/world/items/missing";
		public const string IN_GAME_MENU_TAB_ICONS_PATH_PREFIX = "textures/gui/menu_tab_icon_";
		public const string IN_GAME_MENU_PAGE_PREFAB_PATH = "prefabs/gui/InGameMenuPages/";
		public const string PREFAB_PATH_GUI_NAVIGATION_POINTER = "prefabs/gui/GUINavigationPointer";

	    // -----------------------------
	    // FILESYSTEM
	    // -----------------------------

		public const string FILE_GLOBAL_CONFIG = "globalconfig.json";
	    public const string FILE_DEFAULT_TILE_SET = "tiles";
	    public const string DATA_DIR = "vault";
	    public const string DATA_DIR_MAPS = "maps";
	    public const string FILE_EXTENSION_MAP = "map";
	    public const string FILE_EXTENSION_TILE_SET = "set";
		public const string DATA_DIR_WORLD_OBJECT_DATA = "worldobjects";
		public const string FILE_EXTENSION_WORLD_OBJECT_DATA = "json";
        public const string DATA_DIR_ITEM_TYPE_DATA = "items";
        public const string FILE_EXTENSION_ITEM_TYPE_DATA = "json";

	    // -----------------------------
	    // EDITOR
	    // -----------------------------

	    public const float MOUSE_BUMP_BORDER = 80.0f;
	    public const float MOUSE_BUMP_SPEED = 2000.0f;
	    public const float CAMERA_Y = 6.0f;
	    public const float VERTICAL_EDGE_DISTANCE = 5.0f;

		public const string PREFAB_PATH_EDITOR_MODE_TILE_DRAWING = "mapeditor/gui/EditorModeTileDrawing";
        public const string PREFAB_PATH_EDITOR_MODE_MARKER = "mapeditor/gui/EditorModeMarker";
		public const string PREFAB_PATH_EDITOR_MODE_WORLD_OBJECTS = "mapeditor/gui/EditorModeWorldObjects";
		public const string PREFAB_PATH_EDITOR_MODE_TILE_TAGS = "mapeditor/gui/EditorModeTileTags";

	}

}