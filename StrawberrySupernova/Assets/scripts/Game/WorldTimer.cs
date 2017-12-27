﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            dayText.text = gameTime.WeekdayName;
            dateText.text = String.Format("{0:D2} {1} {2}", gameTime.DateDay, gameTime.DateSeasonName, gameTime.Years);
        }

        public void GoToNextDay(int hourToStartAt)
        {
            gameTime = new GameTime(days: gameTime.Days + 1, hours: hourToStartAt);
            DoTimerEvents();
        }

        public void DoTimerEvents()
        {
            if(eventCallbacks.Count <= 0 || eventCallbacks.First().Key >= gameTime)
                return;
            foreach(var action in eventCallbacks.First().Value)
                action(gameTime);
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