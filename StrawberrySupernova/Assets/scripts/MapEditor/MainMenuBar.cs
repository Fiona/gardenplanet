using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBar : MonoBehaviour
{

    public GameObject menuBarOverlay;
    public MapSetSizeDialog mapSetSizeDialog;

    private MapEditorController controller;

    public void Awake()
    {
        controller = (MapEditorController)FindObjectOfType(typeof(MapEditorController));
        menuBarOverlay.SetActive(false);
        CloseAllDropdowns();
    }

    public void CloseAllDropdowns()
    {
        DropdownMenu[] otherDropdowns = FindObjectsOfType(typeof(DropdownMenu)) as DropdownMenu[];
        foreach(var other in otherDropdowns)
            other.CloseMenu();
        menuBarOverlay.SetActive(false);
    }

    public void MenuOpened()
    {
        menuBarOverlay.SetActive(true);
    }

    public void MenuClosed()
    {
        menuBarOverlay.SetActive(false);
    }

    /*
      --------
      File menu
      --------
    */

    public void QuitButtonPressed()
    {
        CloseAllDropdowns();
        controller.yesNoDialog.Show("Are you sure you want to <b>quit the editor</b>?", this.DoQuitButton);
    }

    public void DoQuitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /*
      --------
      Map menu
      --------
    */

    public void MapSetSizePressed()
    {
        CloseAllDropdowns();
        mapSetSizeDialog.Show(controller.tilemap.width,
                              controller.tilemap.height,
                              this.DoMapSetSize);
    }

    public void DoMapSetSize(int width, int height)
    {
        controller.ResizeTilemapTo(width, height);
    }

}
