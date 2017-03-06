using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
	
	public class MapNameDialog : MonoBehaviour
	{

	    public InputField nameInputField;
	    public Button applyButton;

	    private bool close;

		public IEnumerator Show(StompyBlondie.Ref<string> nameStore)
	    {
	        close = false;
	        gameObject.SetActive(true);
	        nameInputField.text = nameStore.Value;
	        while(!close)
	            yield return new WaitForFixedUpdate();
	        gameObject.SetActive(false);
	        nameStore.Value = nameInputField.text.Trim();
	    }

	    public void CancelPressed()
	    {
	        close = true;
	    }

	    public void ApplyPressed()
	    {
	        close = true;
	    }

	    public void OnValueChangedName(string newVal)
	    {
	        if(nameInputField.text.Trim() == "")
	            applyButton.interactable = false;
	        else
	            applyButton.interactable = true;
	    }

	}

}