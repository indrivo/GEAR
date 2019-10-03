using System;
using System.Collections.Generic;
using ST.Calendar.Abstractions.Enums;
using ST.Calendar.Abstractions.Models;
using System.Threading.Tasks;
using ST.Core.Helpers;

namespace ST.Calendar.Abstractions
{
    public interface ICalendarManager
    {
        /// <summary>
        /// Get task by task Id
        /// </summary>
        /// <param name="timeLine"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<CalendarEvent>>> GetEventsAsync(CalendarTimeLineType timeLine, DateTime dateTime, Guid userId);
    }
}