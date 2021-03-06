using UnityEngine;
using UnityEngine.EventSystems;

namespace GardenPlanet
{
    public abstract class MapEditorMode
    {

        protected GameObject guiHolder;
        protected MapEditorController controller;

        /*
         * Override to provide the printable name of the mode
         */
        public abstract string GetModeName();

        /*
         * Override to provide the resources path of the main GUI prefab for this mode
         */
        public abstract string GetGUIPrefabPath();

        /*
         * Use to give a response to clicking on a tile position with the mouse
         */
        public abstract void TileLocationClicked(TilePosition tilePos, PointerEventData pointerEventData);

        /*
         * Use to add any data to map file when one is saving
         */
        public abstract void SaveToMap(Map map);

        /*
         * Use to add functionality when the map is resized
         */
        public abstract void ResizeMap(int width, int height);

        public virtual void InitializeGUI()
        {
            guiHolder = Object.Instantiate(Resources.Load(GetGUIPrefabPath())) as GameObject;
            var mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
            guiHolder.transform.SetParent(mainCanvas.transform, false);
        }

        public virtual void Initialize()
        {
            controller = Object.FindObjectOfType<MapEditorController>();
            InitializeGUI();
        }

        public virtual void Enable()
        {
            guiHolder.SetActive(true);
        }

        public virtual void Disable()
        {
            guiHolder.SetActive(false);
        }

        public virtual void Destroy()
        {
            Object.Destroy(guiHolder);
        }

        public virtual void Update()
        {
        }
    }
}