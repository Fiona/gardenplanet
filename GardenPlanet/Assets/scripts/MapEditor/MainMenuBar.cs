using System.Collections;
using UnityEngine;

namespace GardenPlanet
{
	
	public class MainMenuBar : MonoBehaviour
	{

	    public GameObject menuBarOverlay;
	    public MapSetSizeDialog mapSetSizeDialog;
	    public MapNameDialog mapNameDialog;
	    public LoadMapDialog loadMapDialog;

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

	    // New map
	    public void NewMapPressed()
	    {
	        CloseAllDropdowns();
	        StartCoroutine(DoNewMapPressed());
	    }

	    public IEnumerator DoNewMapPressed()
	    {
			var nameStore = new StompyBlondie.Ref<string>("");
	        yield return StartCoroutine(mapNameDialog.Show(nameStore));
	        if(nameStore.Value.Trim() == "")
	            yield break;
	        if(Map.DoesMapNameExist(nameStore.Value))
	            ShowBadMessage("A map with this name already exists.");
	        else
	            controller.LoadMap(nameStore.Value);
	    }

	    // Save map
	    public void SaveMapPressed()
	    {
	        CloseAllDropdowns();
	        StartCoroutine(DoSaveMapPressed());
	    }

	    public IEnumerator DoSaveMapPressed()
	    {
	        if(controller.map.filename == null)
	        {
				var nameStore = new StompyBlondie.Ref<string>("");
	            yield return StartCoroutine(mapNameDialog.Show(nameStore));
	            if(nameStore.Value.Trim() == "")
	                yield break;
	            if(Map.DoesMapNameExist(nameStore.Value))
	            {
	                ShowBadMessage("A map with this name already exists.");
	                yield break;
	            }
	            controller.map.filename = nameStore.Value;
	        }
	        controller.SaveMap();
	    }

	    // Load map
	    public void LoadMapPressed()
	    {
	        CloseAllDropdowns();
	        StartCoroutine(DoLoadMapPressed());
	    }

	    public IEnumerator DoLoadMapPressed()
	    {
			var nameStore = new StompyBlondie.Ref<string>("");
	        yield return StartCoroutine(loadMapDialog.Show(nameStore));

	        if(nameStore.Value == "")
	            yield break;

	        if(!Map.DoesMapNameExist(nameStore.Value))
	        {
	            ShowBadMessage("Map cannot be found.");
	            yield break;
	        }

	        controller.LoadMap(nameStore.Value);
	    }

	    // Quit
	    public void QuitButtonPressed()
	    {
	        CloseAllDropdowns();
	        controller.yesNoDialog.Show("Are you sure you want to <b>quit the editor</b>?", this.DoQuitButton);
	    }

	    public void DoQuitButton()
	    {
	        FindObjectOfType<App>().StartNewState(AppState.Title);
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

}