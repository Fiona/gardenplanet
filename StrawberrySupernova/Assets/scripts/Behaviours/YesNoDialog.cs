using System;
using UnityEngine;
using UnityEngine.UI;

public class YesNoDialog : MonoBehaviour
{

    [Header("ObjectReferences")]
    public Button yesButton;
    public Button noButton;
    public Text text;

    private Action callbackYes = null;
    private Action callbackNo = null;

    public void Show(string textToShow, Action callbackYes = null, Action callbackNo = null,
                     string yesButtonText = "Yes", string noButtonText = "No")
    {
        this.callbackNo = callbackNo;
        this.callbackYes = callbackYes;

        yesButton.GetComponentInChildren<Text>().text = yesButtonText;
        noButton.GetComponentInChildren<Text>().text = noButtonText;
        text.text = textToShow;

        gameObject.SetActive(true);
    }

    public void PressedYes()
    {
        gameObject.SetActive(false);
        if(callbackYes != null)
            callbackYes();
    }

    public void PressedNo()
    {
        gameObject.SetActive(false);
        if(callbackNo != null)
            callbackNo();
    }

}
