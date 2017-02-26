using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBar : MonoBehaviour
{

    public GameObject menuBarOverlay;
    public MapSetSizeDialog mapSetSizeDialog;
    public MapNameDialog mapNameDialog;

    private MenuBarMessage message;
    private MapEditorController controller;

    public void Awake()
    {
        controller = (MapEditorController)FindObjectOfType(typeof(MapEditorController));
        message = (MenuBarMessage)FindObjectOfType(typeof(MenuBarMessage));
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

    public void ShowGoodMessage(string messageToShow)
    {
        message.AddMessage(messageToShow, EditorMessageType.Good);
    }

    public void ShowBadMessage(string messageToShow)
    {
        message.AddMessage(messageToShow, EditorMessageType.Bad);
    }

    public void ShowMehMessage(string messageToShow)
    {
        message.AddMessage(messageToShow, EditorMessageType.Meh);
    }

    /*
      --------
      File menu
      --------
    */

    public void NewMapPressed()
    {
        CloseAllDropdowns();
        StartCoroutine(DoNewMapPressed());
    }

    public IEnumerator DoNewMapPressed()
    {
        string newName = null;
        var nameStore = new Ref<string>(newName);
        yield return StartCoroutine(mapNameDialog.Show(nameStore));
        //controller.StartNewMap();
    }


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
