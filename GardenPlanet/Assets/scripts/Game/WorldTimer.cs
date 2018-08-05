using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using StompyBlondie;
using TMPro;
using UnityEngine.AI;

namespace GardenPlanet
{
    public class WorldTimer: MonoBehaviour
    {
        public GameTime gameTime;

        [Header("Object references")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI dayText;
        public TextMeshProUGUI dateText;

        [Header("Clock setup")]
        public Image clockImage;
        public float clockHandAngle = 45f;

        private SortedDictionary<GameTime, List<Action<GameTime>>> eventCallbacks;
        private bool doTimer;
        private float lastMinuteTime;

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
            eventCallbacks = new SortedDictionary<GameTime, List<Action<GameTime>>>();
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
                    gameTime += new GameTime(minutes: 1);
                    lastMinuteTime = Time.time;
                }

                // Trigger any events, not done in minute check because time could change later
                DoTimerEvents();

                UpdateDisplay();
                yield return new WaitForFixedUpdate();
            }

        }

        public void UpdateDisplay()
        {
            timerText.text = String.Format("{0:D2}:{1:D2}", gameTime.TimeHour, gameTime.TimeMinute);
            dayText.text = String.Format("{0} {1:D2}",gameTime.WeekdayName, gameTime.DateDay);
            dateText.text = String.Format("{0} Yr {1}", gameTime.DateSeasonNameWithThird, gameTime.Years);

            // Work out rotation for the clock
            const float minsInDay = (float)(Consts.NUM_MINUTES_IN_HOUR * Consts.NUM_HOURS_IN_DAY);
            const float degreesPerMin = 360 / minsInDay;
            float minsAt = (gameTime.TimeHour * Consts.NUM_MINUTES_IN_HOUR) + gameTime.TimeMinute;

            var angleToSet = (minsAt * degreesPerMin) + clockHandAngle;

            clockImage.transform.localRotation = Quaternion.Euler(
                0f,
                0f,
                -angleToSet
            );
        }

        public void GoToNextDay(int hourToStartAt)
        {
            gameTime = new GameTime(days: gameTime.Days + 1, hours: hourToStartAt);
            DoTimerEvents();
        }

        public void GoToHour(int hourToStartAt)
        {
            gameTime = new GameTime(days: gameTime.Days, hours: hourToStartAt);
            DoTimerEvents();
        }

        public void DoTimerEvents()
        {
            if(eventCallbacks.Count <= 0 || eventCallbacks.First().Key >= gameTime)
                return;
            var actionList = new List<Action<GameTime>>(eventCallbacks.First().Value);
            foreach(var action in actionList)
                action(gameTime);
            if(eventCallbacks.Count > 0)
                eventCallbacks.Remove(eventCallbacks.First().Key);
        }

        public void RemindMe(GameTime atTime, Action<GameTime> callback)
        {
            if(!eventCallbacks.ContainsKey(atTime))
                eventCallbacks[atTime] = new List<Action<GameTime>>();
            eventCallbacks[atTime].Add(callback);
        }

        public void DontRemindMe(Action<GameTime> callback)
        {
            if(eventCallbacks.Count == 0)
                return;

            var deleteKeys = new List<GameTime>();

            foreach(var kvp in eventCallbacks)
            {
                foreach(var action in new List<Action<GameTime>>(kvp.Value))
                    if(action == callback)
                        eventCallbacks[kvp.Key].Remove(callback);
                if(eventCallbacks[kvp.Key].Count == 0)
                    deleteKeys.Add(kvp.Key);
            }

            foreach(var delMe in deleteKeys)
                eventCallbacks.Remove(delMe);
        }

    }
}