using NUnit.Framework;

namespace GardenPlanet
{
    [TestFixture]
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
            Assert.That(time.Minutes, Is.EqualTo(0));
            Assert.That(time.Hours, Is.EqualTo(0));
            Assert.That(time.Days, Is.EqualTo(0));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestOneHour()
        {
            var time = new GameTime(hours: 1);
            Assert.That(time.Minutes, Is.EqualTo(anHour));
            Assert.That(time.Hours, Is.EqualTo(1));
            Assert.That(time.Days, Is.EqualTo(0));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestHalfAnHour()
        {
            var time = new GameTime(minutes: anHour/2);
            Assert.That(time.Minutes, Is.EqualTo(anHour/2));
            Assert.That(time.Hours, Is.EqualTo(0));
            Assert.That(time.Days, Is.EqualTo(0));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestAFewHours()
        {
            var time = new GameTime(hours: 3);
            Assert.That(time.Minutes, Is.EqualTo(anHour*3));
            Assert.That(time.Hours, Is.EqualTo(3));
            Assert.That(time.Days, Is.EqualTo(0));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestADay()
        {
            var time = new GameTime(days: 1);
            Assert.That(time.Minutes, Is.EqualTo(aDay));
            Assert.That(time.Hours, Is.EqualTo(Consts.NUM_HOURS_IN_DAY));
            Assert.That(time.Days, Is.EqualTo(1));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestSomeDays()
        {
            var time = new GameTime(days: 6);
            Assert.That(time.Minutes, Is.EqualTo(aDay*6));
            Assert.That(time.Hours, Is.EqualTo(Consts.NUM_HOURS_IN_DAY*6));
            Assert.That(time.Days, Is.EqualTo(6));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestASeason()
        {
            var time = new GameTime(seasons: 1);
            Assert.That(time.Minutes, Is.EqualTo(aSeason));
            Assert.That(time.Hours, Is.EqualTo(Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Days, Is.EqualTo(Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Seasons, Is.EqualTo(1));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestACoupleSeasons()
        {
            var time = new GameTime(seasons: 2);
            Assert.That(time.Minutes, Is.EqualTo(aSeason*2));
            Assert.That(time.Hours, Is.EqualTo(2*Consts.NUM_HOURS_IN_DAY*Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Days, Is.EqualTo(2*Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Seasons, Is.EqualTo(2));
            Assert.That(time.Years, Is.EqualTo(0));
        }

        [Test]
        public void TestAYear()
        {
            var time = new GameTime(years: 1);
            Assert.That(time.Minutes, Is.EqualTo(aYear));
            Assert.That(time.Hours, Is.EqualTo(Consts.SEASONS.Length*Consts.NUM_HOURS_IN_DAY*Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Days, Is.EqualTo(Consts.SEASONS.Length*Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Seasons, Is.EqualTo(Consts.SEASONS.Length));
            Assert.That(time.Years, Is.EqualTo(1));
        }

        [Test]
        public void TestSomeYears()
        {
            var time = new GameTime(years: 10);
            Assert.That(time.Minutes, Is.EqualTo(aYear * 10));
            Assert.That(time.Hours, Is.EqualTo(10*Consts.SEASONS.Length*Consts.NUM_HOURS_IN_DAY*Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Days, Is.EqualTo(10*Consts.SEASONS.Length*Consts.NUM_DAYS_IN_SEASON));
            Assert.That(time.Seasons, Is.EqualTo(10*Consts.SEASONS.Length));
            Assert.That(time.Years, Is.EqualTo(10));
        }

        [Test]
        public void TestHoursAndMins()
        {
            var time = new GameTime(hours: 6, minutes: 42);
            Assert.That(time.Minutes, Is.EqualTo(42 + (anHour * 6)));
            Assert.That(time.Hours, Is.EqualTo(6));
            Assert.That(time.Days, Is.EqualTo(0));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
            Assert.That(time.TimeHour, Is.EqualTo(6));
            Assert.That(time.TimeMinute, Is.EqualTo(42));
        }

        [Test]
        public void TestHoursMinsAndDays()
        {
            var time = new GameTime(hours: 7, minutes: 23, days: 16);
            Assert.That(time.Minutes, Is.EqualTo(23 + (anHour * 7) + (aDay * 16)));
            Assert.That(time.Hours, Is.EqualTo(7 + (Consts.NUM_HOURS_IN_DAY * 16)));
            Assert.That(time.Days, Is.EqualTo(16));
            Assert.That(time.Seasons, Is.EqualTo(0));
            Assert.That(time.Years, Is.EqualTo(0));
            Assert.That(time.TimeHour, Is.EqualTo(7));
            Assert.That(time.TimeMinute, Is.EqualTo(23));
        }

        [Test]
        public void TestHoursMinsDaysAndSeasons()
        {
            var time = new GameTime(minutes: 12, hours: 18, days: 24, seasons: 3);
            Assert.That(time.Minutes, Is.EqualTo(12 + (18 * anHour) + (24 * aDay) + (3 * aSeason)));
            Assert.That(time.Hours, Is.EqualTo(18 + (Consts.NUM_HOURS_IN_DAY * 24) + (3 * Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON)));
            Assert.That(time.Days, Is.EqualTo(24 + (3 * Consts.NUM_DAYS_IN_SEASON)));
            Assert.That(time.Seasons, Is.EqualTo(3));
            Assert.That(time.Years, Is.EqualTo(0));
            Assert.That(time.TimeHour, Is.EqualTo(18));
            Assert.That(time.TimeMinute, Is.EqualTo(12));
        }

        [Test]
        public void TestHoursMinsDaysSeasonsAndYears()
        {
            var time = new GameTime(minutes: 7, hours: 2, days: 16, seasons: 2, years: 11);
            Assert.That(time.Minutes, Is.EqualTo(7 + (2 * anHour) + (16 * aDay) + (2 * aSeason) + (11 * aYear)));
            Assert.That(time.Hours, Is.EqualTo(2 + (Consts.NUM_HOURS_IN_DAY * 16) +
                                        (2 * Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON) +
                                        (11 * Consts.NUM_HOURS_IN_DAY * Consts.NUM_DAYS_IN_SEASON * Consts.SEASONS.Length)));
            Assert.That(time.Days, Is.EqualTo(16 + (2 * Consts.NUM_DAYS_IN_SEASON) +
                                       (11 * Consts.NUM_DAYS_IN_SEASON * Consts.SEASONS.Length)));
            Assert.That(time.Seasons, Is.EqualTo(2 + (11 * Consts.SEASONS.Length)));
            Assert.That(time.Years, Is.EqualTo(11));
            Assert.That(time.TimeHour, Is.EqualTo(2));
            Assert.That(time.TimeMinute, Is.EqualTo(7));
        }

        [Test]
        public void TestMinsBecomeHours()
        {
            var time = new GameTime(minutes: Consts.NUM_MINUTES_IN_HOUR * 15);
            Assert.That(time.Hours, Is.EqualTo(15));
        }

        [Test]
        public void TestHoursBecomeDays()
        {
            var time = new GameTime(hours: Consts.NUM_HOURS_IN_DAY * 3);
            Assert.That(time.Days, Is.EqualTo(3));
        }

        [Test]
        public void TestDaysBecomeSeasons()
        {
            var time = new GameTime(days: Consts.NUM_DAYS_IN_SEASON * 3);
            Assert.That(time.Seasons, Is.EqualTo(3));
        }

        [Test]
        public void TestSeasonsBecomeYears()
        {
            var time = new GameTime(seasons: Consts.SEASONS.Length * 5);
            Assert.That(time.Years, Is.EqualTo(5));
        }

        [Test]
        public void TestWeekday()
        {
            var time = new GameTime(days: 3);
            Assert.That(time.Weekday, Is.EqualTo(4));
            Assert.That(time.WeekdayName, Is.EqualTo(Consts.WEEKDAYS[3]));
        }

        [Test]
        public void TestWeekdayAnotherWeek()
        {
            var time = new GameTime(days: Consts.WEEKDAYS.Length + 5);
            Assert.That(time.Weekday, Is.EqualTo(6));
            Assert.That(time.WeekdayName, Is.EqualTo(Consts.WEEKDAYS[5]));
        }

        [Test]
        public void TestWeekdayFutureWeeks()
        {
            var time = new GameTime(days: (Consts.WEEKDAYS.Length * 6) + 2);
            Assert.That(time.Weekday, Is.EqualTo(3));
            Assert.That(time.WeekdayName, Is.EqualTo(Consts.WEEKDAYS[2]));
        }

        [Test]
        public void TestDateSeason()
        {
            var time = new GameTime(seasons: 3);
            Assert.That(time.DateSeason, Is.EqualTo(4));
            Assert.That(time.DateSeasonName, Is.EqualTo(Consts.SEASONS[3].displayName));
        }

        [Test]
        public void TestDateSeasonNextYear()
        {
            var time = new GameTime(seasons: Consts.SEASONS.Length + 3);
            Assert.That(time.DateSeason, Is.EqualTo(4));
            Assert.That(time.DateSeasonName, Is.EqualTo(Consts.SEASONS[3].displayName));
        }

        [Test]
        public void TestDateSeasonThirdEarly()
        {
            var time = new GameTime(days: 2);
            Assert.That(time.DateSeasonNameWithThird, Does.StartWith(Consts.SEASON_THIRD_PREFIXES[0]));
        }

        [Test]
        public void TestDateSeasonThirdMid()
        {
            var time = new GameTime(seasons:3, days: 18);
            Assert.That(time.DateSeasonNameWithThird, Does.StartWith(Consts.SEASON_THIRD_PREFIXES[1]));
        }
        [Test]

        public void TestDateSeasonThirdLate()
        {
            var time = new GameTime(years:2, days: 28);
            Assert.That(time.DateSeasonNameWithThird, Does.StartWith(Consts.SEASON_THIRD_PREFIXES[2]));
        }

        [Test]
        public void TestDateSeasonFutureYear()
        {
            var time = new GameTime(seasons: (Consts.SEASONS.Length * 6) + 2);
            Assert.That(time.DateSeason, Is.EqualTo(3));
            Assert.That(time.DateSeasonName, Is.EqualTo(Consts.SEASONS[2].displayName));
        }

        [Test]
        public void TestDateDay()
        {
            var time = new GameTime(days: 4);
            Assert.That(time.DateDay, Is.EqualTo(5));
        }

        [Test]
        public void TestDateDayNextMonth()
        {
            var time = new GameTime(days: 12 + Consts.NUM_DAYS_IN_SEASON);
            Assert.That(time.DateDay, Is.EqualTo(13));
        }

        [Test]
        public void TestDateDayFutureMonth()
        {
            var time = new GameTime(days: 20 + (Consts.NUM_DAYS_IN_SEASON * 30));
            Assert.That(time.DateDay, Is.EqualTo(21));
        }

        [Test]
        public void TestTimeHour()
        {
            var time = new GameTime(hours: 1);
            Assert.That(time.TimeHour, Is.EqualTo(1));
        }

        [Test]
        public void TestTimeHourNextDay()
        {
            var time = new GameTime(hours: 3 + Consts.NUM_HOURS_IN_DAY);
            Assert.That(time.TimeHour, Is.EqualTo(3));
        }

        [Test]
        public void TestTimeHourFutureDay()
        {
            var time = new GameTime(hours: 12 + (Consts.NUM_HOURS_IN_DAY * 56));
            Assert.That(time.TimeHour, Is.EqualTo(12));
        }

        [Test]
        public void TestTimeMinute()
        {
            var time = new GameTime(minutes: 1);
            Assert.That(time.TimeMinute, Is.EqualTo(1));
        }

        [Test]
        public void TestTimeMinuteNextHour()
        {
            var time = new GameTime(minutes: 34 + Consts.NUM_MINUTES_IN_HOUR);
            Assert.That(time.TimeMinute, Is.EqualTo(34));
        }

        [Test]
        public void TestTimeHourFutureHour()
        {
            var time = new GameTime(minutes: 44 + (Consts.NUM_MINUTES_IN_HOUR * 53));
            Assert.That(time.TimeMinute, Is.EqualTo(44));
        }

        [Test]
        public void TestAddition()
        {
            var time1 = new GameTime(minutes: 3);
            var time2 = new GameTime(minutes: 24);
            Assert.That((time1 + time2).Minutes, Is.EqualTo(27));

            time1 = new GameTime(minutes: 3, hours: 4);
            time2 = new GameTime(hours: 5);
            Assert.That((time1 + time2).Hours, Is.EqualTo(9));

            time1 = new GameTime(days: 2, hours: 4);
            time2 = new GameTime(days: 4);
            Assert.That((time1 + time2).Days, Is.EqualTo(6));

            time1 = new GameTime(years: 3, seasons: 2, hours: 3);
            time2 = new GameTime(years: 5, hours: 4);
            Assert.That((time1 + time2).Years, Is.EqualTo(8));
            Assert.That((time1 + time2).TimeHour, Is.EqualTo(7));
        }

        [Test]
        public void TestSubtraction()
        {
            var time1 = new GameTime(minutes: 12);
            var time2 = new GameTime(minutes: 4);
            Assert.That((time1 - time2).Minutes, Is.EqualTo(8));

            time1 = new GameTime(minutes: 3, hours: 4);
            time2 = new GameTime(hours: 2);
            Assert.That((time1 - time2).Hours, Is.EqualTo(2));

            time1 = new GameTime(days: 12, hours: 4);
            time2 = new GameTime(days: 6);
            Assert.That((time1 - time2).Days, Is.EqualTo(6));

            time1 = new GameTime(years: 3, hours: 3);
            time2 = new GameTime(hours: 2);
            Assert.That((time1 - time2).Years, Is.EqualTo(3));
            Assert.That((time1 - time2).TimeHour, Is.EqualTo(1));
        }

        [Test]
        public void TestEquality()
        {
            Assert.That(
                new GameTime(minutes: 10),
                Is.EqualTo(new GameTime(minutes: 10))
            );

            Assert.That(
                new GameTime(minutes: 15),
                Is.Not.EqualTo(new GameTime(minutes: 2))
            );

            Assert.That(
                new GameTime(minutes: 25, hours:4),
                Is.EqualTo(new GameTime(minutes: 25, hours:4))
            );
            Assert.That(
                new GameTime(minutes: 15, hours:8),
                Is.Not.EqualTo(new GameTime(minutes: 2, hours:8))
            );

            Assert.That(
                new GameTime(minutes: 35, hours:2, days:16),
                Is.EqualTo(new GameTime(minutes: 35, hours:2, days:16))
            );
            Assert.That(
                new GameTime(minutes: 15, hours:8, days:10),
                Is.Not.EqualTo(new GameTime(minutes: 2, hours:8, days:10))
            );

            Assert.That(
                new GameTime(minutes: 25, hours:4, days:18, seasons:2),
                Is.EqualTo(new GameTime(minutes: 25, hours:4, days:18, seasons:2))
            );
            Assert.That(
                new GameTime(minutes: 15, hours:8, days:19, seasons:1),
                Is.Not.EqualTo(new GameTime(minutes: 2, hours:8, days:19, seasons:2))
            );

            Assert.That(
                new GameTime(minutes: 25, hours:4, days:18, seasons:2, years:1000),
                Is.EqualTo(new GameTime(minutes: 25, hours:4, days:18, seasons:2, years:1000))
            );
            Assert.That(
                new GameTime(minutes: 15, hours:8, days:19, seasons:1, years:2001),
                Is.Not.EqualTo(new GameTime(minutes: 2, hours:8, days:19, seasons:2, years:2002))
            );
        }

        [Test]
        public void TestGreater()
        {
            Assert.That(
                new GameTime(minutes:22),
                Is.GreaterThan(new GameTime(minutes:10))
            );
            Assert.That(
                new GameTime(minutes:2),
                Is.Not.GreaterThan(new GameTime(minutes:16))
            );

            Assert.That(
                new GameTime(minutes:22, hours:3),
                Is.GreaterThan(new GameTime(minutes:10, hours:1))
            );
            Assert.That(
                new GameTime(minutes:2,hours:1),
                Is.Not.GreaterThan(new GameTime(minutes:16, hours:12))
            );

            Assert.That(
                new GameTime(minutes:22, hours:5, days:15),
                Is.GreaterThan(new GameTime(minutes:10, hours:12, days:2))
            );
            Assert.That(
                new GameTime(minutes:2, hours:7, days:2) ,
                Is.Not.GreaterThan(new GameTime(minutes:16, hours:12, days:30))
            );

            Assert.That(
                new GameTime(minutes:22, hours:5, days:15, seasons:4),
                Is.GreaterThan(new GameTime(minutes:10, hours:12, days:2, seasons:2))
            );
            Assert.That(
                new GameTime(minutes:2, hours:7, days:10, seasons:2),
                Is.Not.GreaterThan(new GameTime(minutes:16, hours:12, days:5, seasons:4))
            );

            Assert.That(
                new GameTime(minutes:22, hours:5, days:15, seasons:2, years:3000),
                Is.GreaterThan(new GameTime(minutes:10, hours:12, days:2, seasons:4, years:2000))
            );
            Assert.That(
                new GameTime(minutes:2, hours:7, days:10, seasons:3, years:1),
                Is.Not.GreaterThan(new GameTime(minutes:16, hours:12, days:5, seasons:2, years:20))
            );
        }

        [Test]
        public void TestLess()
        {
            Assert.That(
                new GameTime(minutes:2),
                Is.LessThan(new GameTime(minutes:16))
            );
            Assert.That(
                new GameTime(minutes:22),
                Is.Not.LessThan(new GameTime(minutes:10))
            );

            Assert.That(
                new GameTime(minutes:2,hours:1),
                Is.LessThan(new GameTime(minutes:16, hours:12))
            );
            Assert.That(
                new GameTime(minutes:22, hours:3),
                Is.Not.LessThan(new GameTime(minutes:10, hours:1))
            );

            Assert.That(
                new GameTime(minutes:2, hours:7, days:2),
                Is.LessThan(new GameTime(minutes:16, hours:12, days:30))
            );
            Assert.That(
                new GameTime(minutes:22, hours:5, days:15),
                Is.Not.LessThan(new GameTime(minutes:10, hours:12, days:2))
            );

            Assert.That(
                new GameTime(minutes:2, hours:7, days:10, seasons:2),
                Is.LessThan(new GameTime(minutes:16, hours:12, days:5, seasons:4))
            );
            Assert.That(
                new GameTime(minutes:22, hours:5, days:15, seasons:4),
                Is.Not.LessThan(new GameTime(minutes:10, hours:12, days:2, seasons:2))
            );

            Assert.That(
                new GameTime(minutes:2, hours:7, days:10, seasons:3, years:1),
                Is.LessThan(new GameTime(minutes:16, hours:12, days:5, seasons:2, years:20))
            );
            Assert.That(
                new GameTime(minutes:22, hours:5, days:15, seasons:2, years:3000),
                Is.Not.LessThan(new GameTime(minutes:10, hours:12, days:2, seasons:4, years:2000))
            );
        }

    }
}