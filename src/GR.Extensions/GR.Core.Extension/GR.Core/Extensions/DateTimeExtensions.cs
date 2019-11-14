using System;

namespace GR.Core.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Get end of day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        /// <summary>
        /// Start of day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Day of week
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int DayIndex(this DateTime date)
        {
            int index;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    index = 7;
                    break;
                default:
                    index = (int)date.DayOfWeek;
                    break;
            }

            return index;
        }
    }
}
