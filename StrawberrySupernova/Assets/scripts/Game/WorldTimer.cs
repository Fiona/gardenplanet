using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
	public class WorldTimer: MonoBehaviour
	{
		public Text timerText;
		public Text dayText;
		public Text dateText;

		[HideInInspector]
		public int hour;
		[HideInInspector]
		public int minute;
		[HideInInspector]
		public int day;
		[HideInInspector]
		public int weekDay;
		[HideInInspector]
		public int season;
		[HideInInspector]
		public int year = Consts.GAME_START_YEAR;

		bool doTimer;
		float lastMinuteTime;

		public void Reset()
		{
			hour = 0;
			minute = 0;
			day = 1;
			season = 1;
			CalculateWeekDay();
			year = Consts.GAME_START_YEAR;
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
					GoToNextMinute();
					lastMinuteTime = Time.time;
				}					

				UpdateDisplay();
				yield return new WaitForFixedUpdate();
			}
				
		}

		public void UpdateDisplay()
		{
			timerText.text = String.Format("{0:D2}:{1:D2}", hour, minute);
			dayText.text = Consts.WEEKDAYS[weekDay - 1];
			dateText.text = String.Format("{0:D2} {1} {2}", day, Consts.SEASONS[season - 1], year);
		}

		public void GoToNextMinute()
		{
			minute++;
			if(minute >= Consts.NUM_MINUTES_IN_HOUR)
			{
				minute = 0;
				GoToNextHour();
			}
		}

		public void GoToNextHour()
		{
			hour++;
			minute = 0;
			if(hour >= Consts.NUM_HOURS_IN_DAY)
			{
				hour = 0;
				GoToNextDay();
			}
		}

		public void GoToNextDay()
		{
			day++;
			hour = 0;
			minute = 0;
			CalculateWeekDay();
			if(day > Consts.NUM_DAYS_IN_SEASON)
			{
				day = 1;
				GoToNextSeason();
			}
		}

		public void GoToNextSeason()
		{
			season++;
			day = 1;
			hour = 0;
			minute = 0;
			CalculateWeekDay();
			if(season > Consts.SEASONS.Length)
			{
				season = 1;
				GoToNextYear();
			}
		}

		public void GoToNextYear()
		{
			season = 1;
			day = 1;
			hour = 0;
			minute = 0;
			var curYear = year;
			Reset();
			year = curYear+1;
			CalculateWeekDay();
		}

		void CalculateWeekDay()
		{
			var numDaysInCurYear = ((Consts.NUM_DAYS_IN_SEASON * (season - 1)) + (day -1));
			var numDaysAllYears = (year - Consts.GAME_START_YEAR) * (Consts.NUM_DAYS_IN_SEASON * Consts.SEASONS.Length);
			var numDays = numDaysInCurYear + numDaysAllYears;
			weekDay = (numDays % (Consts.WEEKDAYS.Length)) + 1;
		}
						
	}
}

