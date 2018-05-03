using System.Collections;
using UnityEngine;

namespace GardenPlanet
{
    public class EditorInputManager: MonoBehaviour
    {

        [HideInInspector] public bool directInputEnabled = true;
        [HideInInspector] public Vector2? mouseWorldPosition;

        private MapEditorController controller;
        private float previousScrollWheelAxis;

        public void Awake()
        {
            controller = FindObjectOfType<MapEditorController>();
            SetUpMouse();
            StartCoroutine(HandlePanning());
        }

        public void SetUpMouse()
        {
            Texture2D mouseTexture = Resources.Load(Consts.TEXTURE_PATH_GUI_MOUSE) as Texture2D;
            SetMouseTexture(mouseTexture);
        }

        public void Start()
        {
            StartCoroutine(ManageInputEditor());
        }

        public IEnumerator ManageInputEditor()
        {

            while(true)
            {

                mouseWorldPosition = null;

                while(!directInputEnabled)
                    yield return new WaitForFixedUpdate();

                // Get mouse over tiles
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_MOUSE_HOVER_PLANE))
                {
                    var rayNormal = hit.transform.TransformDirection(hit.normal);
                    if(rayNormal == hit.transform.up)
                    {
                        mouseWorldPosition = new Vector2(hit.point.x, hit.point.z);
                        int tileOverX = (int)((hit.point.x + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                        int tileOverY = (int)((hit.point.z + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                        var tileOn = controller.tilemap.GetTileAt(tileOverX, tileOverY, controller.currentLayer);
                        if(tileOn == null)
                        {
                            controller.tilemap.MouseOverTile(null);
                            yield return new WaitForFixedUpdate();
                            continue;
                        }
                        else
                            controller.tilemap.MouseOverTile(tileOn);
                    }
                }
                else
                    controller.tilemap.MouseOverTile(null);

                yield return new WaitForFixedUpdate();

            }

        }

        public IEnumerator HandlePanning()
        {
            while(true)
            {
                if(Input.GetMouseButtonDown(2))
                {
                    var initialMousePosition = Input.mousePosition;
                    var pannedPosition = Vector2.zero;
                    Cursor.lockState = CursorLockMode.Locked;
                    while(true)
                    {
                        controller.PanCamera(-Input.GetAxis("Mouse X") * .5f, -Input.GetAxis("Mouse Y") * .5f);
                        if (Input.GetMouseButtonUp(2))
                        {
                            Cursor.lockState = CursorLockMode.None;
                            SetUpMouse();
                            break;
                        }
                        yield return new WaitForFixedUpdate();
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }

        /*
         Stops the player doing anything directly like moving the character.
         Still allows mouse clicks to continue.
         */
        public void LockDirectInput()
        {
            directInputEnabled = false;
        }

        /*
         Undoes LockDirectInput
         */
        public void UnlockDirectInput()
        {
            directInputEnabled = true;
        }

        public void SetMouseTexture(Texture2D texture)
        {
            Cursor.SetCursor(texture, new Vector2(1.0f, 1.0f), CursorMode.Auto);
        }

    }
}