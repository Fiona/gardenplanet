using System;
using UnityEngine;

namespace StrawberryNova
{

	public class GameErrorException: Exception
	{
		public GameErrorException()
		{
		}

		public GameErrorException(string message)
			: base(message)
		{
		}

		public GameErrorException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	public class EditorErrorException: Exception
	{
	    public EditorErrorException()
	    {
	    }

	    public EditorErrorException(string message)
	        : base(message)
	    {
	        var controller = (MapEditorController)MonoBehaviour.FindObjectOfType(typeof(MapEditorController));
			if(controller != null)
				controller.mainMenuBar.ShowBadMessage(message);
	    }

	    public EditorErrorException(string message, Exception inner)
	        : base(message, inner)
	    {
	        var controller = (MapEditorController)MonoBehaviour.FindObjectOfType(typeof(MapEditorController));
			if(controller != null)
				controller.mainMenuBar.ShowBadMessage(message);
	    }
	}

}