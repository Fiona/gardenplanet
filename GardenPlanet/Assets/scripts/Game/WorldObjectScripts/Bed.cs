using System.Collections;
using StompyBlondie.Common;

namespace GardenPlanet
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
			const string question = "Go to bed and sleep till the morning?";
			var result = new Ref<int>(-1);
			yield return StartCoroutine(StompyBlondie.ChoicePopup.ShowYesNoPopup(question, result));
			if(result.Value == 1)
				yield return StartCoroutine(controller.PlayerSleep(gameObject));
		}
	}
}

