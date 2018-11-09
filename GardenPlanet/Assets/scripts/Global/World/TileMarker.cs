using UnityEngine;
using StompyBlondie.Common.Types;

namespace GardenPlanet
{
    public class TileMarker
    {
        public int x;
        public int y;
        public int layer;
        public EightDirection direction;
        public TileMarkerType type;
        public Attributes attributes;
        public GameObject gameObject;
    }
}