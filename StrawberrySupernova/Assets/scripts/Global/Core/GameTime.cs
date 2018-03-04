using System;
using UnityEngine;
using Object = System.Object;

namespace StrawberryNova
{
    /*
     * Representation of times in the game. Can use addition and subtraction
     * on other GameTime objects to manipulate it.
     * It can be constructed with minutes, hours, days, seasons and years.
     * Available members are Minutes, Hours, Days, Seasons, Years, WeekDay,
     * WeekDayName.
     *
     * DateSeason, DateSeasonName members are the current sesason the time
     * is currently going through.
     *
     * DateDay is the current number of the day in the season (starting at 1).
     *
     * TimeHour and TimeMinute will return the current hour and minute in the
     * day that the time is at.
     */
    public class GameTime: IComparable
    {

        int _minutes;

        // How many minutes the time represents
        public int Minutes{
            get{
                return Math.Max(_minutes, 0);
            }
        }

        // How many complete hours the time represents
        public int Hours{
            get{
                return (int)Math.Floor((float)_minutes / (float)Consts.NUM_MINUTES_IN_HOUR);
            }
        }

        // How many complete days the time represents
        public int Days{
            get{
                return (int)((float)Hours / (float)Consts.NUM_HOURS_IN_DAY);
            }
        }

        // How many complete seasons the time represents
        public int Seasons{
            get{
                return (int)((float)Days / (float)Consts.NUM_DAYS_IN_SEASON);
            }
        }

        // How many complete years the time represents
        public int Years{
            get{
                return (int)((float)Seasons / (float)Consts.SEASONS.Length);
            }
        }

        // Current day of the week as a number starting from 1
        public int Weekday{
            get{
                return (Days % Consts.WEEKDAYS.Length) + 1;
            }
        }

        // A displayable string for the day of the week
        public string WeekdayName{
            get{
                return Consts.WEEKDAYS[Weekday-1];
            }
        }

        // The current season as a number starting from 1
        public int DateSeason{
            get{
                return (Seasons % Consts.SEASONS.Length) + 1;
            }
        }

        // The current season as a displayable string
        public string DateSeasonName{
            get{
                return Consts.SEASONS[DateSeason-1].displayName;
            }
        }

        // The current day of the season starting at 1
        public int DateDay{
            get{
                return (Days - (Consts.NUM_DAYS_IN_SEASON * Seasons)) + 1;
            }
        }

        // The current hour that the time is in
        public int TimeHour{
            get{
                return (Hours - (Consts.NUM_HOURS_IN_DAY * Days));
            }
        }

        // The current minute that the time is in
        public int TimeMinute{
            get{
                return (Minutes - (Consts.NUM_MINUTES_IN_HOUR * Hours));
            }
        }

        public GameTime(int minutes = 0, int hours = 0, int days = 0, int seasons = 0, int years = 0)
        {
            // Initialise based on passed time
            _minutes = minutes;
            _minutes += hours * Consts.NUM_MINUTES_IN_HOUR;
            _minutes += days * Consts.NUM_HOURS_IN_DAY * Consts.NUM_MINUTES_IN_HOUR;
            _minutes += seasons * Consts.NUM_DAYS_IN_SEASON * Consts.NUM_HOURS_IN_DAY * Consts.NUM_MINUTES_IN_HOUR;
            _minutes += years * Consts.SEASONS.Length * Consts.NUM_DAYS_IN_SEASON  * Consts.NUM_HOURS_IN_DAY * Consts.NUM_MINUTES_IN_HOUR;
        }

        public GameTime(GameTime existingGameTime)
        {
            _minutes = existingGameTime.Minutes;
        }

        // Operator overloads
        public static GameTime operator +(GameTime left, GameTime right)
        {
            return new GameTime(left.Minutes + right.Minutes);
        }

        public static GameTime operator -(GameTime left, GameTime right)
        {
            return new GameTime(left.Minutes - right.Minutes);
        }

        public override bool Equals(Object obj)
        {
            var item = obj as GameTime;

            if(item == null)
                return false;

            return item.Minutes == Minutes;
        }

        public override int GetHashCode()
        {
            return Minutes;
        }

        public int CompareTo(Object obj)
        {
            var item = obj as GameTime;
            if(item == null)
                return 1;
            if(Minutes < item.Minutes)
                return -1;
            return Minutes > item.Minutes ? 1 : 0;
        }

        public static bool operator ==(GameTime left, GameTime right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }

        public static bool operator !=(GameTime left, GameTime right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : !left.Equals(right);
        }

        public static bool operator <=(GameTime left, GameTime right)
        {
            return left.Minutes <= right.Minutes;
        }

        public static bool operator <(GameTime left, GameTime right)
        {
            return left.Minutes < right.Minutes;
        }

        public static bool operator >=(GameTime left, GameTime right)
        {
            return left.Minutes >= right.Minutes;
        }

        public static bool operator >(GameTime left, GameTime right)
        {
            return left.Minutes > right.Minutes;
        }

        public override string ToString()
        {
            return String.Format("GameTime. Min:{0} Hour:{1} Day:{2} Seas:{3} Year:{4}", TimeMinute, TimeHour, DateDay,
                DateSeason, Years);
        }

    }
}

