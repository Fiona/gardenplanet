using System;
using UnityEngine;

public class EditorErrorException: Exception
{
    public EditorErrorException()
    {
    }

    public EditorErrorException(string message)
        : base(message)
    {
        var controller = (MapEditorController)MonoBehaviour.FindObjectOfType(typeof(MapEditorController));
        controller.mainMenuBar.ShowBadMessage(message);
    }

    public EditorErrorException(string message, Exception inner)
        : base(message, inner)
    {
        var controller = (MapEditorController)MonoBehaviour.FindObjectOfType(typeof(MapEditorController));
        controller.mainMenuBar.ShowBadMessage(message);
    }
}
