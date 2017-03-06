using UnityEngine;

namespace StrawberryNova
{

	public class DropdownMenu : MonoBehaviour
	{

	    [Header("Object references")]
	    public GameObject dropdownContents;
	    public MainMenuBar mainMenuBar;

	    public void Awake()
	    {
	        CloseMenu();
	    }

	    public void OpenMenu()
	    {

	        if(dropdownContents.activeSelf)
	        {
	            CloseMenu();
	            mainMenuBar.MenuClosed();
	            return;
	        }

	        mainMenuBar.CloseAllDropdowns();
	        dropdownContents.SetActive(true);
	        mainMenuBar.MenuOpened();

	    }

	    public void CloseMenu()
	    {
	        dropdownContents.SetActive(false);
	    }

	}

}