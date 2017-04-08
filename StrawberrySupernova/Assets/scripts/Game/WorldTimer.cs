using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using StompyBlondie;

namespace StrawberryNova
{
	public class WorldTimer: MonoBehaviour
	{
        public GameTime gameTime;

		public Text timerText;
		public Text dayText;
		public Text dateText;

		bool doTimer;
		float lastMinuteTime;

		public void Reset()
		{
            gameTime = new GameTime(years: Consts.GAME_START_YEAR);
		}

		public void StartTimer()
		{
			lastMinuteTime = Time.time;
			doTimer = true;	
		}

		public void StopTimer()
		{
			doTimer = false;
		}

		public void Awake()
		{
			Reset();
		}

		public void Start()
		{
			StartCoroutine(DoTimer());
		}

		public IEnumerator DoTimer()
		{
			while(true)
			{
				if(!doTimer)
				{
					UpdateDisplay();
					yield return new WaitForFixedUpdate();
					continue;
				}

				// Go to next minute
				if(Time.time > lastMinuteTime + Consts.SECONDS_PER_GAME_MINUTE)
				{
                    var previousGameTime = new GameTime(gameTime);
                    gameTime += new GameTime(minutes: 1);
					lastMinuteTime = Time.time;

                    // Trigger any events
                    if(gameTime.Hours > previousGameTime.Hours)
                        FindObjectOfType<App>().events.NewHourEvent.Invoke(gameTime.TimeHour);
				}					

				UpdateDisplay();
				yield return new WaitForFixedUpdate();
			}
				
		}

		public void UpdateDisplay()
		{
            timerText.text = String.Format("{0:D2}:{1:D2}", gameTime.TimeHour, gameTime.TimeMinute);
            dayText.text = gameTime.WeekdayName;
            dateText.text = String.Format("{0:D2} {1} {2}", gameTime.DateDay, gameTime.DateSeasonName, gameTime.Years);
		}

        public void GoToNextDay(int hourToStartAt)
        {
            gameTime = new GameTime(days: gameTime.Days + 1, seasons: gameTime.Seasons, years: gameTime.Years) +
                new GameTime(hours: hourToStartAt);
        }
						
	}
}

