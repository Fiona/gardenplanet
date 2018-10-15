using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GardenPlanet
{

    public static class Consts
    {

        // -----------------------------
        // CONFIG SETTINGS
        // -----------------------------

        // Where to boot the app into
        public const AppState INITIAL_APP_STATE = AppState.CreateACharacter;

        // What the first map name is
        public const string START_MAP = "devtest";

        // -----------------------------
        // TILE DRAWING
        // -----------------------------

        // Internal collision layer numbers
        public const int COLLISION_LAYER_TILES = 8;
        public const int COLLISION_LAYER_WORLD_OBJECTS = 9;
        public const int COLLISION_LAYER_MOUSE_HOVER_PLANE = 10;
        public const int COLLISION_LAYER_PLAYER = 12;
        public const int COLLISION_LAYER_ITEMS = 13;
        public const int COLLISION_LAYER_GHOST_WORLD_OBJECTS = 15;

        public const int COLLISION_LAYER_CHARACTERS = 14;

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

        // Rewired input behaviour IDs
        public const int REWIRED_INPUT_BEHAVIOUR_DEFAULT = 0;
        public const int REWIRED_INPUT_BEHAVIOUR_MENUDIRECTIONS = 1;

        // Any mouse wheel axis movements at or outside of this movement will
        // class as a click in that direction.
        public const float MOUSE_WHEEL_CLICK_SNAP = 0.1f;

        // How long to wait before registering another item drop
        public const float DROP_ITEM_COOLDOWN = .25f;

        // -----------------------------
        // CAMERA
        // -----------------------------

        // How far away the camera usually is from the player
        public const float CAMERA_PLAYER_DISTANCE = 3.0f;

        // How far from the ground the rotation usually is
        public const float CAMERA_DEFAULT_ROTATION = 45.0f;

        // How fast the camera will lock to the player
        public const float CAMERA_PLAYER_LOCK_SPEED = 3.0f;

        // -----------------------------
        // CHARACTERS
        // -----------------------------

        public const float CHARACTER_MOVE_ACCELERATION = 80f;

        public const float CHARACTER_MAX_WALK_SPEED = .8f;

        public const float CHARACTER_MAX_RUN_SPEED = 1.5f;

        public const float CHARACTER_ROTATION_SPEED = 10f;

        public const float CHARACTER_JUMP_FORCE = 800f;

        public static readonly float[] CHARACTER_BETWEEN_BLINK_WAIT_RANGE = { 2f, 3f };

        public static readonly float[] CHARACTER_BLINK_TIME_RANGE = { .05f, .1f };

        public static string CHAR_ID_PLAYER = "__PLAYER__";

        // -----------------------------
        // PLAYER
        // -----------------------------

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

        // How far the auto pickup radius is
        public const float PLAYER_AUTO_PICKUP_RADIUS = .2f;

        // How many hearts till the player does a yawn
        public const float PLAYER_YAWN_ENERGY_THRESHOLD = 2f;

        // -----------------------------
        // GAMEPLAY
        // -----------------------------

        public const int GAME_START_YEAR = 1;

        // How many seconds it takes for the in-world timer to go forward a minute
        public const float SECONDS_PER_GAME_MINUTE = 1f;

        // Game time settings
        public const int NUM_MINUTES_IN_HOUR = 60;
        public const int NUM_HOURS_IN_DAY = 24;
        public const int NUM_DAYS_IN_SEASON = 30;

        public readonly static string[] WEEKDAYS = {
            "Monday", "Tuesday", "Wednesday", "Thursday",
            "Friday", "Saturday", "Sunday"
        };

        public readonly static Season[] SEASONS =
        {
            new Season{shortName = "spring", displayName = "Spring"},
            new Season{shortName = "summer", displayName = "Summer"},
            new Season{shortName = "autumn", displayName = "Autumn"},
            new Season{shortName = "winter", displayName = "Winter"}
        };

        public readonly static string[] SEASON_THIRD_PREFIXES = {"Early-", "Mid-", "Late-"};

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
        public const string TEXTURE_PATH_GUI_MOUSE = "textures/gui/mouse_normal";
        public const string TEXTURE_PATH_GUI_MOUSE_HOVER = "textures/gui/mouse_hover";
        public const string TEXTURE_PATH_GUI_MOUSE_OKAY = "textures/gui/mouse_okay";
        public const string TEXTURE_PATH_GUI_MOUSE_ERROR = "textures/gui/mouse_error";
        public const string TILES_PREFABS_PATH = "prefabs/world/tiles/";
        public const string WORLD_OBJECTS_PREFABS_PATH = "prefabs/world/worldobjects/";
        public const string ITEMS_PREFABS_PATH = "prefabs/world/items/";
        public const string ITEMS_PREFAB_MISSING = "prefabs/world/items/missing";
        public const string IN_GAME_MENU_TAB_ICONS_PATH_PREFIX = "textures/gui/menu_tab_icon_";
        public const string IN_GAME_MENU_PAGE_PREFAB_PATH = "prefabs/gui/InGameMenuPages/";
        public const string PREFAB_PATH_GUI_NAVIGATION_POINTER = "prefabs/gui/GUINavigationPointer";
        public const string PREFAB_PATH_IN_GAME_MENU_BUTTON = "prefabs/gui/InGameMenuButton";
        public const string PREFAB_PATH_LOADING_SCREEN = "prefabs/LoadingScreen";
        public const string MATERIAL_PATH_HOED_SOIL = "materials/HoedSoil";
        public const string MATERIAL_PATH_WATERED_HOED_SOIL = "materials/WateredHoedSoil";
        public const string PREFAB_PATH_PLAYER = "prefabs/world/Player";
        public const string PREFAB_PATH_CHARACTER = "prefabs/world/Character";
        public const string PREFAB_PATH_EFFECTS = "effects/";

        public const string CHARACTERS_BASE_VISUAL_PATH = "characters/base/";
        public const string CHARACTERS_TOPS_VISUAL_PATH = "characters/appearence/tops/";
        public const string CHARACTERS_BOTTOMS_VISUAL_PATH = "characters/appearence/bottoms/";
        public const string CHARACTERS_SHOES_VISUAL_PATH = "characters/appearence/shoes/";
        public const string CHARACTERS_HEAD_ACCESSORIES_VISUAL_PATH = "characters/appearence/head_accessories/";
        public const string CHARACTERS_BACK_ACCESSORIES_VISUAL_PATH = "characters/appearence/back_accessories/";
        public const string CHARACTERS_HAIR_VISUAL_PATH = "characters/appearence/hair/";
        public const string CHARACTERS_EYES_TEXTURE_PATH = "characters/appearence/eyes/";
        public const string CHARACTERS_MOUTHS_TEXTURE_PATH = "characters/appearence/mouths/";
        public const string CHARACTERS_NOSES_TEXTURE_PATH = "characters/appearence/noses/";
        public const string CHARACTERS_EYEBROWS_TEXTURE_PATH = "characters/appearence/eyebrows/";
        public const string CHARACTERS_FACE_DETAILS_TEXTURE_PATH = "characters/appearence/face_details/";

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
        public const string FILE_GAME_SETTINGS_FILE = "gamesettings.dat";
        public const string FILE_TILE_MARKERS_FILE = "tilemarkers.json";

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
        public const string PREFAB_PATH_EDITOR_ATTRIBUTES_DIALOG = "mapeditor/gui/EditAttributesDialog";

    }

}