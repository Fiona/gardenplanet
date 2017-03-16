using System;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
	public class Bed: MonoBehaviour, IWorldObjectScript
	{
		GameController controller;
		public void Start()
		{
			controller = FindObjectOfType<GameController>();
		}

		public bool ShouldTick()
		{
			return false;
		}	

		public IEnumerator Create()
		{
			yield return null;
		}

		public IEnumerator Spawn()
		{
			yield return null;
		}

		public IEnumerator Tick()
		{
			yield return null;
		}

		public IEnumerator PlayerInteract()
		{
			var result = new StompyBlondie.Ref<int>(-1);
			yield return StartCoroutine(StompyBlondie.ChoicePopup.ShowChoicePopup(
				"Do you want to go to bed and sleep till the morning?",
				new string[]{"Yes please", "Nah"},
				result
			));
			if(result.Value == 1)
			{
				yield return StartCoroutine(StompyBlondie.ScreenFade.FadeOut(2f));
				controller.PlayerDoSleep();
				yield return StartCoroutine(StompyBlondie.ScreenFade.FadeIn(3f));
			}
		}
	}
}

