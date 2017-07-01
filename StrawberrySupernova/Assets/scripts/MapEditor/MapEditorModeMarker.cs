using UnityEngine.EventSystems;
    
namespace StrawberryNova
{
    public class MapEditorModeMarker: MapEditorMode
    {
        
        public override string GetModeName()
        {
            return "Tile Markers";
        }

        public override string GetGUIPrefabPath()
        {
            return Consts.PREFAB_PATH_EDITOR_MODE_MARKER;
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public override void InitializeGUI()
        {
            base.InitializeGUI();
        }

        public override void TileLocationClicked(TilePosition tilePos, PointerEventData pointerEventData)
        {
        }
        
    }
}