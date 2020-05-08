using System;
using System.Linq;
using GR.Core.Abstractions;

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

        /// <summary>
        /// Intersects dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="intersectingStartDate"></param>
        /// <param name="intersectingEndDate"></param>
        /// <returns></returns>
        public static bool Intersects(this DateTime startDate, DateTime endDate, DateTime intersectingStartDate, DateTime intersectingEndDate)
        {
            return (intersectingEndDate >= startDate && intersectingStartDate <= endDate);
        }

        /// <summary>
        /// Check if is weekend
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime value)
        {
            return (value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday);
        }

        /// <summary>
        /// Get the age of person
        /// </summary>
        /// <param name="dateOfBirth"></param>
        /// <returns></returns>
        public static int Age(this DateTime dateOfBirth)
        {
            if (DateTime.Today.Month < dateOfBirth.Month ||
                DateTime.Today.Month == dateOfBirth.Month &&
                DateTime.Today.Day < dateOfBirth.Day)
            {
                return DateTime.Today.Year - dateOfBirth.Year - 1;
            }

            return DateTime.Today.Year - dateOfBirth.Year;
        }

        /// <summary>
        /// Returns whether or not a DateTime is during a leap year.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLeapYear(this DateTime value)
        {
            return (DateTime.DaysInMonth(value.Year, 2) == 29);
        }

        /// <summary>
        /// Display text date
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DisplayTextDate(this DateTime source, string format = null)
        {
            if (format == null)
            {
                format = GearSettings.Date.DateFormatWithTime;
            }

            string text;
            var duration = DateTime.Now - source;

            if (duration.TotalMinutes < 1)
            {
                text = "less than a minute ago";
            }
            else if (duration.TotalMinutes < 60)
            {
                text = $"about {(int)duration.TotalMinutes} minutes ago";
            }
            else if (duration.TotalDays < 1)
            {
                text = $"about {(int)duration.TotalHours} hours ago";
            }
            else if (duration.TotalDays < 30)
            {
                text = $"about {(int)duration.TotalDays} days ago";
            }
            else
            {
                text = source.ToString(format);
            }

            return text;
        }

        /// <summary>
        /// Created this week
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<TModel> CreatedThisWeek<TModel>(this IQueryable<TModel> query)
            where TModel : BaseModel
        {
            if (query == null) return null;
            var today = DateTime.Now;
            var weekStart = today.AddDays(-today.DayIndex());
            query = query.Where(x => x.Created >= weekStart);
            return query;
        }

        /// <summary>
        /// Created this month
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<TModel> CreatedThisMonth<TModel>(this IQueryable<TModel> query)
            where TModel : BaseModel
        {
            if (query == null) return null;
            var today = DateTime.Now;
            var monthStart = today.AddDays(-today.Day);
            query = query.Where(x => x.Created >= monthStart);
            return query;
        }

        /// <summary>
        /// Created today
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<TModel> CreatedToday<TModel>(this IQueryable<TModel> query)
            where TModel : BaseModel
        {
            if (query == null) return null;
            var today = DateTime.Now;
            query = query.Where(x => x.Created >= today.StartOfDay() && x.Created <= today.EndOfDay());
            return query;
        }

        /// <summary>
        /// Created in 1h
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<TModel> CreatedOneHour<TModel>(this IQueryable<TModel> query)
            where TModel : BaseModel
        {
            if (query == null) return null;
            var today = DateTime.Now;
            query = query.Where(x => x.Created <= today && x.Created >= today.AddHours(-1));
            return query;
        }

        /// <summary>
        /// Created this year
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<TModel> CreatedThisYear<TModel>(this IQueryable<TModel> query)
            where TModel : BaseModel
        {
            if (query == null) return null;
            var today = DateTime.Now;
            query = query.Where(x => x.Created.Year == today.Year);
            return query;
        }
    }
}
