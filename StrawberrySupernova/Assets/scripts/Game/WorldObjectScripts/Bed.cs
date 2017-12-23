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
			yield return StompyBlondie.DialoguePopup.ShowDialoguePopup(
				"Louise",
				"Hello I am Louise. This is your bed. That might be a bit weird, but just go with it."
			);
			yield return StompyBlondie.MessagePopup.ShowMessagePopup("You did a thing!!");
			var result = new StompyBlondie.Ref<int>(-1);
			yield return StartCoroutine(StompyBlondie.ChoicePopup.ShowChoicePopup(
				"Do you want to go to bed and sleep till the morning?",
				new string[]{"Yes please", "Nah"},
				result
			));
			if(result.Value == 1)
			{
                yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeOut(2f));
				controller.PlayerDoSleep();
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeIn(3f));
				yield return StompyBlondie.DialoguePopup.ShowDialoguePopup(
					"Louise",
					"Morning!"
				);

			}
		}
	}
}

