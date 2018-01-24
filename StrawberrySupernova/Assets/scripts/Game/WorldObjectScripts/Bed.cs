using System;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
	public class Bed: WorldObjectScript
	{
		GameController controller;
		public void Start()
		{
			controller = FindObjectOfType<GameController>();
		}

		public override IEnumerator PlayerInteract()
		{
			yield return StartCoroutine(StompyBlondie.DialoguePopup.ShowDialoguePopup("Scoolie", "Hi I'm testing this!"));
			var result = new StompyBlondie.Ref<int>(-1);
			yield return StartCoroutine(StompyBlondie.ChoicePopup.ShowChoicePopup(
				"Go to bed and sleep till the morning?",
				new string[]{"Yep", "Nope"},
				result
			));
			if(result.Value == 1)
				yield return StartCoroutine(controller.PlayerSleep());
		}

	}
}

