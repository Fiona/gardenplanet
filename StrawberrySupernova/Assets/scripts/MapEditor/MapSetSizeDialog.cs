using System;
using UnityEngine;
using UnityEngine.UI;

public class MapSetSizeDialog : MonoBehaviour
{

    public InputField widthInput;
    public InputField heightInput;

    private Action<int, int> callback;

    public void Show(int width, int height, Action<int, int> callback)
    {
        this.callback = callback;
        gameObject.SetActive(true);
        widthInput.text = width.ToString();
        heightInput.text = height.ToString();
    }

    public void CancelPressed()
    {
        gameObject.SetActive(false);
    }

    public void ApplyPressed()
    {
        gameObject.SetActive(false);
        callback(Int32.Parse(widthInput.text), Int32.Parse(heightInput.text));
    }

}
