using UnityEngine;

namespace StrawberryNova
{

	// Top level state
	public enum AppState{Title, Editor, Game};

	// Concepts
	public enum Direction{Down, Left, Up, Right};
	public enum EightDirection{Down, DownLeft, Left, LeftUp, Up, UpRight, Right, RightDown};
	public enum RotationalDirection{Clockwise, AntiClockwise};

	// Tile related
	public enum TileTypeVolumeType{CollisionPlane, CollisionBox};

	public class TilePosition
	{
		public int x;
		public int y;
		public int layer;
		public Direction dir;
		public string name = "";
	}

	public class ObjectTilePosition: TilePosition
	{
		public GameObject gameObject;
	}

	// Representation of points in the world
	public class WorldPosition
	{
		public float x;
		public float y;
		public float height;
		public EightDirection dir;
		public string name = "";
	}

	public class ObjectWorldPosition: WorldPosition
	{
		public GameObject gameObject;
	}

	// Editor
	public enum EditorMessageType{Good, Bad, Meh};

    // TODO: delete
	public enum EditorMode{Tile, WorldObject, Marker};

}

