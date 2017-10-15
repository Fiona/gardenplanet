using UnityEngine;

namespace StrawberryNova
{
    public class TileSelection: MonoBehaviour
    {
        public GameObject displayHolder;
        public GameObject errorDisplay;
        public GameObject okDisplay;
        public GameObject normalDisplay;
        
        private GameController controller;

        private void Awake()
        {
            controller = FindObjectOfType<GameController>();
        }

        public void Update()
        {
            if(controller.mouseOverTile == null)
            {
                displayHolder.SetActive(false);
                return;
            }
            displayHolder.SetActive(true);
            errorDisplay.SetActive(false);            
            okDisplay.SetActive(false);            
            normalDisplay.SetActive(false);
            transform.position = new Vector3(
                controller.mouseOverTile.x * Consts.TILE_SIZE,
                controller.mouseOverTile.layer * Consts.TILE_SIZE,
                controller.mouseOverTile.y * Consts.TILE_SIZE
            );
             
            normalDisplay.SetActive(true);
            
        }

    }
}