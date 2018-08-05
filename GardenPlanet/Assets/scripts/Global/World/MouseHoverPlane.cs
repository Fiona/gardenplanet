using System;
using UnityEngine;

namespace GardenPlanet
{
    public class MouseHoverPlane: MonoBehaviour
    {
        BoxCollider planeCollision;
        App app;

        public void Awake()
        {
            app = FindObjectOfType<App>();
        }

        public void Start()
        {
            //RecreateCollisionPlane();
        }
            
        public void RecreateCollisionPlane(Tilemap tilemap)
        {
            int layer = 0;
            if(app.state == AppState.Editor)
            {
                layer = FindObjectOfType<MapEditorController>().currentLayer;
            }
            Destroy(planeCollision);
            planeCollision = gameObject.AddComponent<BoxCollider>();
            planeCollision.size = new Vector3(Consts.TILE_SIZE*tilemap.width, 0.1f, Consts.TILE_SIZE * tilemap.height);
            planeCollision.center = new Vector3(
                Consts.TILE_SIZE*tilemap.width/2-(Consts.TILE_SIZE/2),
                layer * Consts.TILE_SIZE,
                Consts.TILE_SIZE*tilemap.height/2-(Consts.TILE_SIZE/2)
            );
            planeCollision.gameObject.layer = Consts.COLLISION_LAYER_MOUSE_HOVER_PLANE;
        }
    }
}

