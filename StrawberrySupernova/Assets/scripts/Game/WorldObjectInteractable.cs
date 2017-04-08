using System;
using UnityEngine;

namespace StrawberryNova
{
	public class WorldObjectInteractable: MonoBehaviour
	{
		public WorldObject worldObject;

		Glowable focusGlow;
		Glowable highlightGlow;
		bool doHighlight;
		bool doFocus;
		bool focussed;
		GameController controller;

		public void Start()
		{
			focusGlow = gameObject.AddComponent<Glowable>();		
			highlightGlow = gameObject.AddComponent<Glowable>();
		}

		public void Awake()
		{
			controller = FindObjectOfType<GameController>();
		}

		public void Update()
		{
			if(focussed && worldObject != null && controller != null && controller.objectCurrentlyInteractingWith == null)
			{
				controller.ShowPopup(worldObject.name);
			}
		}

		public void LateUpdate()
		{
			if((doHighlight && doFocus) || controller.objectCurrentlyInteractingWith == worldObject)
			{
				focusGlow.GlowTo(new Color(0f, .5f, 1f), .1f);
				focussed = true;
			}
			else
			{
				focussed = false;
				if(doHighlight)
					highlightGlow.GlowTo(new Color(.6f, .75f, .86f), .05f);
			}
			doHighlight = false;
			doFocus = false;
		}

		public void Highlight()
		{
			doHighlight = true;
		}

		public void Focus()
		{
			doFocus = true;
		}

		public void InteractWith()
		{
			if(!doHighlight || !doFocus)
				return;
			StartCoroutine(controller.PlayerInteractWith(worldObject));
		}

	}
}

