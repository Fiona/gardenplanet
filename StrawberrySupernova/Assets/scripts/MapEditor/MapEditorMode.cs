using UnityEngine;
using UnityEngine.EventSystems;

namespace StrawberryNova
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

        public virtual void InitializeGUI()
        {
            guiHolder = Object.Instantiate(Resources.Load(GetGUIPrefabPath())) as GameObject;
            guiHolder.transform.SetParent(Object.FindObjectOfType<Canvas>().transform, false);
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
            guiHolder.SetActive(true);
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

