using System;
using UnityEngine;

namespace StrawberryNova
{
	/*
	 * Handles the sun, lighting and weather.
	 */
	public class Atmosphere: MonoBehaviour
	{
		WorldTimer worldTimer;

		void Start()
		{
			worldTimer = FindObjectOfType<WorldTimer>();
		}

		void Update()
		{			
			var hourT = ((float)worldTimer.hour / (float)Consts.NUM_HOURS_IN_DAY);
			var minT = (float)worldTimer.minute / ((float)Consts.NUM_MINUTES_IN_HOUR * (float)Consts.NUM_HOURS_IN_DAY);
			float rotateY = Mathf.Lerp(0f, 360f, hourT + minT);

			const float halfDay = Consts.NUM_HOURS_IN_DAY / 2f;
			var hourTZ = ((float)worldTimer.hour % halfDay) / halfDay;
			float rotateZ = (worldTimer.hour < halfDay) ?
				Mathf.Lerp(-35f, 0, hourTZ + minT) :
				Mathf.Lerp(0, -35f, hourTZ + minT);

			transform.rotation = Quaternion.Euler(0f, rotateY, rotateZ);

		}
	}
}

