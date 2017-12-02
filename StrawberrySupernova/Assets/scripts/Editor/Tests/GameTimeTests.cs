using System;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace StrawberryNova
{
    [TestFixture]
//    [Category("GameTimeTests")]
    public class GameTimeTest
    {

        int anHour = Consts.NUM_MINUTES_IN_HOUR;
        int aDay = Consts.NUM_HOURS_IN_DAY * Consts.NUM_MINUTES_IN_HOUR;
        int aSeason = Consts.NUM_DAYS_IN_SEASON * Consts.NUM_HOURS_IN_DAY * Consts.NUM_MINUTES_IN_HOUR;
        int aYear = Consts.SEASONS.Length * Consts.NUM_DAYS_IN_SEASON * Consts.NUM_HOURS_IN_DAY * Consts.NUM_MINUTES_IN_HOUR;

        [Test]
        public void TestEmptyConstructor()
        {
            var time = new GameTime();
            Assert.AreEqual(time.Minutes, 0);
            Assert.AreEqual(time.Hours, 0);
            Assert.AreEqual(time.Days, 0);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestOneHour()
        {
            var time = new GameTime(hours: 1);
            Assert.AreEqual(time.Minutes, anHour);
            Assert.AreEqual(time.Hours, 1);
            Assert.AreEqual(time.Days, 0);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestHalfAnHour()
        {
            var time = new GameTime(minutes: anHour/2);
            Assert.AreEqual(time.Minutes, anHour/2);
            Assert.AreEqual(time.Hours, 0);
            Assert.AreEqual(time.Days, 0);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestAFewHours()
        {
            var time = new GameTime(hours: 3);
            Assert.AreEqual(time.Minutes, anHour*3);
            Assert.AreEqual(time.Hours, 3);
            Assert.AreEqual(time.Days, 0);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestADay()
        {
            var time = new GameTime(days: 1);
            Assert.AreEqual(time.Minutes, aDay);
            Assert.AreEqual(time.Hours, Consts.NUM_HOURS_IN_DAY);
            Assert.AreEqual(time.Days, 1);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestSomeDays()
        {
            var time = new GameTime(days: 6);
            Assert.AreEqual(time.Minutes, aDay*6);
            Assert.AreEqual(time.Hours, Consts.NUM_HOURS_IN_DAY*6);
            Assert.AreEqual(time.Days, 6);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestASeason()
        {
            var time = new GameTime(seasons: 1);
            Assert.AreEqual(time.Minutes, aSeason);
            Assert.AreEqual(time.Hours, Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Days, Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Seasons, 1);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestACoupleSeasons()
        {
            var time = new GameTime(seasons: 2);
            Assert.AreEqual(time.Minutes, aSeason*2);
            Assert.AreEqual(time.Hours, 2*Consts.NUM_HOURS_IN_DAY*Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Days, 2*Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Seasons, 2);
            Assert.AreEqual(time.Years, 0);
        }

        [Test]
        public void TestAYear()
        {
            var time = new GameTime(years: 1);
            Assert.AreEqual(time.Minutes, aYear);
            Assert.AreEqual(time.Hours, Consts.SEASONS.Length*Consts.NUM_HOURS_IN_DAY*Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Days, Consts.SEASONS.Length*Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Seasons, Consts.SEASONS.Length);
            Assert.AreEqual(time.Years, 1);
        }

        [Test]
        public void TestSomeYears()
        {
            var time = new GameTime(years: 10);
            Assert.AreEqual(time.Minutes, aYear * 10);
            Assert.AreEqual(time.Hours, 10*Consts.SEASONS.Length*Consts.NUM_HOURS_IN_DAY*Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Days, 10*Consts.SEASONS.Length*Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.Seasons, 10*Consts.SEASONS.Length);
            Assert.AreEqual(time.Years, 10);
        }

        [Test]
        public void TestHoursAndMins()
        {
            var time = new GameTime(hours: 6, minutes: 42);
            Assert.AreEqual(time.Minutes, 42 + (anHour * 6));
            Assert.AreEqual(time.Hours, 6);
            Assert.AreEqual(time.Days, 0);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
            Assert.AreEqual(time.TimeHour, 6);
            Assert.AreEqual(time.TimeMinute, 42);
        }

        [Test]
        public void TestHoursMinsAndDays()
        {
            var time = new GameTime(hours: 7, minutes: 23, days: 16);
            Assert.AreEqual(time.Minutes, 23 + (anHour * 7) + (aDay * 16));
            Assert.AreEqual(time.Hours, 7 + (Consts.NUM_HOURS_IN_DAY * 16));
            Assert.AreEqual(time.Days, 16);
            Assert.AreEqual(time.Seasons, 0);
            Assert.AreEqual(time.Years, 0);
            Assert.AreEqual(time.TimeHour, 7);
            Assert.AreEqual(time.TimeMinute, 23);
        }

        [Test]
        public void TestHoursMinsDaysAndSeasons()
        {
            var time = new GameTime(minutes: 12, hours: 18, days: 24, seasons: 3);
            Assert.AreEqual(time.Minutes, 12 + (18 * anHour) + (24 * aDay) + (3 * aSeason));
            Assert.AreEqual(time.Hours, 18 + (Consts.NUM_HOURS_IN_DAY * 24) + (3 * Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON));
            Assert.AreEqual(time.Days, 24 + (3 * Consts.NUM_DAYS_IN_SEASON));
            Assert.AreEqual(time.Seasons, 3);
            Assert.AreEqual(time.Years, 0);
            Assert.AreEqual(time.TimeHour, 18);
            Assert.AreEqual(time.TimeMinute, 12);
        }

        [Test]
        public void TestHoursMinsDaysSeasonsAndYears()
        {
            var time = new GameTime(minutes: 7, hours: 2, days: 16, seasons: 2, years: 11);
            Assert.AreEqual(time.Minutes, 7 + (2 * anHour) + (16 * aDay) + (2 * aSeason) + (11 * aYear));
            Assert.AreEqual(time.Hours, 2 + (Consts.NUM_HOURS_IN_DAY * 16) +
                (2 * Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON) +
                (11 * Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON * Consts.SEASONS.Length));
            Assert.AreEqual(time.Days, 16 + (2 * Consts.NUM_DAYS_IN_SEASON) +
                (11 * Consts.NUM_DAYS_IN_SEASON * Consts.SEASONS.Length));
            Assert.AreEqual(time.Seasons, 2 + (11 * Consts.SEASONS.Length));
            Assert.AreEqual(time.Years, 11);
            Assert.AreEqual(time.TimeHour, 2);
            Assert.AreEqual(time.TimeMinute, 7);
        }

        [Test]
        public void TestMinsBecomeHours()
        {
            var time = new GameTime(minutes: Consts.NUM_MINUTES_IN_HOUR * 15);
            Assert.AreEqual(time.Hours, 15);
        }

        [Test]
        public void TestHoursBecomeDays()
        {
            var time = new GameTime(hours: Consts.NUM_HOURS_IN_DAY * 3);
            Assert.AreEqual(time.Days, 3);
        }

        [Test]
        public void TestDaysBecomeSeasons()
        {
            var time = new GameTime(days: Consts.NUM_DAYS_IN_SEASON * 3);
            Assert.AreEqual(time.Seasons, 3);
        }

        [Test]
        public void TestSeasonsBecomeYears()
        {
            var time = new GameTime(seasons: Consts.SEASONS.Length * 5);
            Assert.AreEqual(time.Years, 5);
        }

        [Test]
        public void TestWeekday()
        {
            var time = new GameTime(days: 3);
            Assert.AreEqual(time.Weekday, 4);
            Assert.AreEqual(time.WeekdayName, Consts.WEEKDAYS[3]);
        }

        [Test]
        public void TestWeekdayAnotherWeek()
        {
            var time = new GameTime(days: Consts.WEEKDAYS.Length + 5);
            Assert.AreEqual(time.Weekday, 6);
            Assert.AreEqual(time.WeekdayName, Consts.WEEKDAYS[5]);
        }

        [Test]
        public void TestWeekdayFutureWeeks()
        {
            var time = new GameTime(days: (Consts.WEEKDAYS.Length * 6) + 2);
            Assert.AreEqual(time.Weekday, 3);
            Assert.AreEqual(time.WeekdayName, Consts.WEEKDAYS[2]);
        }

        [Test]
        public void TestDateSeason()
        {
            var time = new GameTime(seasons: 3);
            Assert.AreEqual(time.DateSeason, 4);
            Assert.AreEqual(time.DateSeasonName, Consts.SEASONS[3]);
        }

        [Test]
        public void TestDateSeasonNextYear()
        {
            var time = new GameTime(seasons: Consts.SEASONS.Length + 3);
            Assert.AreEqual(time.DateSeason, 4);
            Assert.AreEqual(time.DateSeasonName, Consts.SEASONS[3]);
        }

        [Test]
        public void TestDateSeasonFutureYear()
        {
            var time = new GameTime(seasons: (Consts.SEASONS.Length * 6) + 2);
            Assert.AreEqual(time.DateSeason, 3);
            Assert.AreEqual(time.DateSeasonName, Consts.SEASONS[2]);
        }

        [Test]
        public void TestDateDay()
        {
            var time = new GameTime(days: 4);
            Assert.AreEqual(time.DateDay, 5);
        }

        [Test]
        public void TestDateDayNextMonth()
        {
            var time = new GameTime(days: 12 + Consts.NUM_DAYS_IN_SEASON);
            Assert.AreEqual(time.DateDay, 13);
        }

        [Test]
        public void TestDateDayFutureMonth()
        {
            var time = new GameTime(days: 20 + (Consts.NUM_DAYS_IN_SEASON * 30));
            Assert.AreEqual(time.DateDay, 21);
        }

        [Test]
        public void TestTimeHour()
        {
            var time = new GameTime(hours: 1);
            Assert.AreEqual(time.TimeHour, 1);
        }

        [Test]
        public void TestTimeHourNextDay()
        {
            var time = new GameTime(hours: 3 + Consts.NUM_HOURS_IN_DAY);
            Assert.AreEqual(time.TimeHour, 3);
        }

        [Test]
        public void TestTimeHourFutureDay()
        {
            var time = new GameTime(hours: 12 + (Consts.NUM_HOURS_IN_DAY * 56));
            Assert.AreEqual(time.TimeHour, 12);
        }

        [Test]
        public void TestTimeMinute()
        {
            var time = new GameTime(minutes: 1);
            Assert.AreEqual(time.TimeMinute, 1);
        }

        [Test]
        public void TestTimeMinuteNextHour()
        {
            var time = new GameTime(minutes: 34 + Consts.NUM_MINUTES_IN_HOUR);
            Assert.AreEqual(time.TimeMinute, 34);
        }

        [Test]
        public void TestTimeHourFutureHour()
        {
            var time = new GameTime(minutes: 44 + (Consts.NUM_MINUTES_IN_HOUR * 53));
            Assert.AreEqual(time.TimeMinute, 44);
        }

        [Test]
        public void TestAddition()
        {
            var time1 = new GameTime(minutes: 3);
            var time2 = new GameTime(minutes: 24);
            Assert.AreEqual((time1 + time2).Minutes, 27);

            time1 = new GameTime(minutes: 3, hours: 4);
            time2 = new GameTime(hours: 5);
            Assert.AreEqual((time1 + time2).Hours, 9);

            time1 = new GameTime(days: 2, hours: 4);
            time2 = new GameTime(days: 4);
            Assert.AreEqual((time1 + time2).Days, 6);

            time1 = new GameTime(years: 3, seasons: 2, hours: 3);
            time2 = new GameTime(years: 5, hours: 4);
            Assert.AreEqual((time1 + time2).Years, 8);
            Assert.AreEqual((time1 + time2).TimeHour, 7);
        }

        [Test]
        public void TestSubtraction()
        {
            var time1 = new GameTime(minutes: 12);
            var time2 = new GameTime(minutes: 4);
            Assert.AreEqual((time1 - time2).Minutes, 8);

            time1 = new GameTime(minutes: 3, hours: 4);
            time2 = new GameTime(hours: 2);
            Assert.AreEqual((time1 - time2).Hours, 2);

            time1 = new GameTime(days: 12, hours: 4);
            time2 = new GameTime(days: 6);
            Assert.AreEqual((time1 - time2).Days, 6);

            time1 = new GameTime(years: 3, hours: 3);
            time2 = new GameTime(hours: 2);
            Assert.AreEqual((time1 - time2).Years, 3);
            Assert.AreEqual((time1 - time2).TimeHour, 1);
        }

    }
}

