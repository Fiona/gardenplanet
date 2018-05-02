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
            if(controller.activeTile == null || controller.noTileSelection)
            {
                displayHolder.SetActive(false);
                return;
            }

            // Do info popup
            var objs = controller.activeTile.GetTileWorldObjects();
            foreach(var o in objs)
            {
                var info = o.GetInfoPopup();
                if(info.Text == "")
                    continue;
                controller.ShowInfoPopup(controller.activeTile, info);
            }

            // Display the right version of the selection marker
            displayHolder.SetActive(true);
            errorDisplay.SetActive(false);
            okDisplay.SetActive(false);
            normalDisplay.SetActive(false);
            transform.position = new Vector3(
                controller.activeTile.x * Consts.TILE_SIZE,
                controller.activeTile.layer * Consts.TILE_SIZE,
                controller.activeTile.y * Consts.TILE_SIZE
            );

            if(controller.itemHotbar.selectedItemEntry == null)
            {
                normalDisplay.SetActive(true);
                return;
            }

            if(controller.itemHotbar.CanBeUsedOnTilePos(controller.activeTile))
            {
                okDisplay.SetActive(true);
                controller.GameInputManager.SetMouseTexture(controller.GameInputManager.mouseOkay);
            }
            else
            {
                controller.GameInputManager.SetMouseTexture(controller.GameInputManager.mouseError);
                errorDisplay.SetActive(true);
            }
        }

    }
}