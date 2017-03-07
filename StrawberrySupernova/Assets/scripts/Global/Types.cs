using UnityEngine;

namespace StrawberryNova
{

	// Top level state
	public enum AppState{Title, Editor, Game};

	// Concepts
	public enum Direction{Left, Right, Up, Down};
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

	// Editor
	public enum EditorMessageType{Good, Bad, Meh};
	public enum EditorMode{Tile, Object, Marker};

}

