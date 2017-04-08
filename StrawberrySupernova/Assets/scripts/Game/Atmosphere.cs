using System;
using System.Linq;
using UnityEngine;

namespace StrawberryNova
{
	[Serializable]
	public class AtmosphereTime
	{
		public Color colour;
		public int hour;
	}
	/*
	 * Handles the sun, lighting and weather.
	 */
	public class Atmosphere: MonoBehaviour
	{
		public AtmosphereTime[] atmosphereTimes;
		public AtmosphereTime[] ambientLightTimes;
		public Light sun;

		WorldTimer worldTimer;

		void Start()
		{
			worldTimer = FindObjectOfType<WorldTimer>();
		}

		void Update()
		{	
			// Rotate around the Y to change shadow direction
            var hourT = ((float)worldTimer.gameTime.TimeHour / (float)Consts.NUM_HOURS_IN_DAY);
            var minT = (float)worldTimer.gameTime.TimeMinute / ((float)Consts.NUM_MINUTES_IN_HOUR * (float)Consts.NUM_HOURS_IN_DAY);
			float rotateY = Mathf.Lerp(0f, 360f, hourT + minT);

			// Tilt a bit to change the shadow size
			const float halfDay = Consts.NUM_HOURS_IN_DAY / 2f;
            var hourTZ = ((float)worldTimer.gameTime.TimeHour % halfDay) / halfDay;
            float rotateZ = (worldTimer.gameTime.TimeHour < halfDay) ?
				Mathf.Lerp(-35f, 0, hourTZ + minT) :
				Mathf.Lerp(0, -35f, hourTZ + minT);

			transform.rotation = Quaternion.Euler(0f, rotateY, rotateZ);

			// Get the light colours
            sun.color = GetColourFromTimeArray(atmosphereTimes, worldTimer.gameTime.TimeHour, worldTimer.gameTime.TimeMinute);
            RenderSettings.ambientLight = GetColourFromTimeArray(ambientLightTimes, worldTimer.gameTime.TimeHour, worldTimer.gameTime.TimeMinute);
		}

		/*
		 * Gets the colour a light should be at when passed an array of AtmosphereTime objects.
		 * Pass current hour and minute get those values.
		 */
		Color GetColourFromTimeArray(AtmosphereTime[] timeArray, int hour, int minute)
		{			
			AtmosphereTime timeFrom = null;
			AtmosphereTime timeTo = null;
			int num = 0;
			foreach(var time in timeArray)
			{
				// If we're on the exact hour then we need to manually grab the next one
                if(time.hour == worldTimer.gameTime.TimeHour)
				{
					timeFrom = time;
					if(num < timeArray.Length - 1)
						timeTo = timeArray[num + 1];
					else
					{
						// We need to loop back around if at the end of the array
						timeTo = timeArray[0];
					}					
				}

				// Make sure we're in the bounds
                if(time.hour < worldTimer.gameTime.TimeHour)
					timeFrom = time;			
                if(time.hour > worldTimer.gameTime.TimeHour)
				{
					// We've gone past our allotted time so we should have two times now
					timeTo = time;
					break;
				}
				num++;
			}

			// No times found so we'll default to terrible ones
			if(timeArray.Length == 0 || (timeFrom == null && timeTo == null))
			{
				timeTo = new AtmosphereTime {
					hour = 0,
					colour = new Color(1f, 0f, 0f)
				};
				timeFrom = new AtmosphereTime {
					hour = 23,
					colour = new Color(0f, 1f, 0f)
				};
			}

			// We didn't find a to time but found a from then we loop back around
			if(timeTo == null)
				timeTo = timeArray[0];
			// And same the other way
			if(timeFrom == null)
				timeFrom = timeArray.Last();

			// Lerp between our found colours at the correct time
			var hourT = ((float)(hour - timeFrom.hour) / (float)(timeTo.hour - timeFrom.hour));
			var minT = (float)minute / ((float)Consts.NUM_MINUTES_IN_HOUR * (float)(timeTo.hour - timeFrom.hour));
			return Color.Lerp(timeFrom.colour, timeTo.colour, hourT + minT);
		}
	}
}

