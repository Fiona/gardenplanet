using System;
using System.Collections.Generic;
using System.Linq;
using Rewired.Utils.Classes.Data;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace GardenPlanet
{

    // Top level state
    public enum AppState
    {
        Logo,
        Title,
        Editor,
        Game,
        CreateACharacter
    };

    // Concepts
    public enum Direction
    {
        Down,
        Left,
        Up,
        Right
    };

    public enum EightDirection
    {
        Down,
        DownLeft,
        Left,
        LeftUp,
        Up,
        UpRight,
        Right,
        RightDown
    };

    public enum RotationalDirection
    {
        Clockwise,
        AntiClockwise
    }

    // Tile related
    public enum TileTypeVolumeShape
    {
        CollisionPlane,
        CollisionBox
    }

    public enum TileTypeVolumeSurface
    {
        WALL,
        FLOOR
    }

    public class TilePosition
    {
        public int x;
        public int y;
        public int layer;
        public EightDirection dir;
        public string name = "";

        private static WorldObjectManager _worldObjectManager;

        public TilePosition()
        {
        }

        public TilePosition(int x, int y, int layer)
        {
            this.x = x;
            this.y = y;
            this.layer = layer;
        }

        public TilePosition(Vector3 pos)
        {
            x = Mathf.RoundToInt(pos.x / Consts.TILE_SIZE);
            layer = Mathf.RoundToInt(pos.y / Consts.TILE_SIZE);
            y = Mathf.RoundToInt(pos.z / Consts.TILE_SIZE);
        }

        public TilePosition(WorldPosition pos)
        {
            x = Mathf.RoundToInt(pos.x / Consts.TILE_SIZE);
            layer = Mathf.RoundToInt(pos.height / Consts.TILE_SIZE);
            y = Mathf.RoundToInt(pos.y / Consts.TILE_SIZE);
        }

        public float TileDistance(TilePosition otherPos)
        {
            return Mathf.Sqrt(
                Mathf.Pow(otherPos.x-x, 2) +
                Mathf.Pow(otherPos.y-y, 2)
            );
        }

        public TilePosition Offset(int _x, int _y, int _layer = 0)
        {
            return new TilePosition(x + _x, y + _y, layer + _layer);
        }

        public bool ContainsCollidableWorldObjects()
        {
            var pos = new Vector3(
                x*Consts.TILE_SIZE,
                (layer*Consts.TILE_SIZE)-(Consts.TILE_SIZE/2),
                y*Consts.TILE_SIZE
            );
            var checkWorldObjectsOverlap = Physics.OverlapBox(
                pos,
                new Vector3(Consts.TILE_SIZE/2, Consts.TILE_SIZE/2, Consts.TILE_SIZE/2),
                Quaternion.identity,
                layerMask:1 << Consts.COLLISION_LAYER_WORLD_OBJECTS
            );
            return checkWorldObjectsOverlap.Length > 0;
        }

        public List<WorldObject> GetTileWorldObjects()
        {
            if(TilePosition._worldObjectManager == null)
                TilePosition._worldObjectManager = UnityEngine.Object.FindObjectOfType<WorldObjectManager>();
            return _worldObjectManager.GetWorldObjectsAtTilePos(this);
        }

        public List<WorldObject> GetTileWorldObjects(string name)
        {
            if(TilePosition._worldObjectManager == null)
                TilePosition._worldObjectManager = UnityEngine.Object.FindObjectOfType<WorldObjectManager>();
            var objList = _worldObjectManager.GetWorldObjectsAtTilePos(this);
            return objList.Where(singleObj => singleObj.name == name).ToList();
        }

        public int[] ToArray()
        {
            return new int[3] {x, y, layer};
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", x, y, layer);
        }
    }

    // Representation of tiles in a particular map
    public class MapTilePosition : TilePosition
    {
        public Map map;

        public MapTilePosition(Map map, int x, int y, int layer) : base(x, y, layer)
        {
            this.map = map;
        }

        public MapTilePosition(Map map, Vector3 pos) : base(pos)
        {
            this.map = map;
        }

        public MapTilePosition(Map map, WorldPosition pos) : base(pos)
        {
            this.map = map;
        }
    }

    public class ObjectTilePosition : TilePosition
    {
        public GameObject gameObject;
    }

    public struct TileTag
    {
        public int X;
        public int Y;
        public int Layer;
        public string TagType;
    }

    // Representation of points in the world
    public class WorldPosition
    {
        public float x;
        public float y;
        public float height;
        public EightDirection dir;
        public string name = "";

        public WorldPosition()
        {
        }

        public WorldPosition(float x, float y, float height)
        {
            this.x = x;
            this.y = y;
            this.height = height;
        }

        public WorldPosition(TilePosition tilePos)
        {
            x = tilePos.x * Consts.TILE_SIZE;
            y = tilePos.y * Consts.TILE_SIZE;
            height = tilePos.layer * Consts.TILE_SIZE;
            dir = tilePos.dir;
        }

        public WorldPosition(Vector3 transformPos)
        {
            x = transformPos.x;
            y = transformPos.z;
            height = transformPos.y;
        }

        public Vector3 TransformPosition()
        {
            return new Vector3(x, height, y);
        }
    }

    // Representation of points in a particular map
    public class MapWorldPosition : WorldPosition
    {
        public Map map;

        public MapWorldPosition(Map map, float x, float y, float height) : base(x, y, height)
        {
            this.map = map;
        }

        public MapWorldPosition(Map map, TilePosition tilePos) : base(tilePos)
        {
            this.map = map;
        }

        public MapWorldPosition(Map map, Vector3 transformPos) : base(transformPos)
        {
            this.map = map;
        }
    }

    // World object positions
    public class ObjectWorldPosition: WorldPosition
    {
        public GameObject gameObject;
    }

    // Dialog popup positions
    public enum PopupPositions
    {
        TopLeft, Top, TopRight,
        CentreLeft, Centre, CentreRight,
        BottomLeft, Bottom, BottomRight
    }

    // Editor
    public enum EditorMessageType{Good, Bad, Meh};

    // Season info
    public struct Season
    {
        public string shortName;
        public string displayName;

        public static int GetSeasonByShortName(string shortName)
        {
            var i = 1;
            foreach(var s in Consts.SEASONS)
            {
                if(s.shortName == shortName)
                    return i;
                i++;
            }
            return 0;
        }
    };
}