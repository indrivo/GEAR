using System;
using System.Collections.Generic;
using ST.Calendar.Abstractions.Models;
using System.Threading.Tasks;
using ST.Calendar.Abstractions.Enums;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Core.Helpers;

namespace ST.Calendar.Abstractions
{
    public interface ICalendarManager
    {
        /// <summary>
        /// Get event by id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<ResultModel<CalendarEvent>> GetEventByIdAsync(Guid? eventId);

        /// <summary>
        /// Get my events
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<CalendarEvent>>> GetAllEventsOrganizedByMeAsync();

        /// <summary>
        /// Get events on interval
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<CalendarEvent>>> GetEventsAsync(Guid? userId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get my events on interval
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<CalendarEvent>>> GetMyEventsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Create new event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateEventAsync(BaseEventViewModel evt);

        /// <summary>
        /// Add or update new members to an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        Task<ResultModel> AddOrUpdateMembersToEventAsync(Guid? eventId, IList<Guid> users);


        /// <summary>
        /// Delete event permanently
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteEventPermanentlyAsync(Guid? eventId);

        /// <summary>
        /// Delete event logically
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteEventLogicallyAsync(Guid? eventId);


        /// <summary>
        /// Restore deleted event
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<ResultModel> RestoreEventLogicallyAsync(Guid? eventId);

        /// <summary>
        /// Get events
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="origin">Default is today date</param>
        /// <param name="timeLineType">Specify the interval of time</param>
        /// <param name="expandDayPrecision">Specify the expand interval in days, default is zero</param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<CalendarEvent>>> GetUserEventsByTimeLineAsync(Guid? userId, DateTime? origin,
            CalendarTimeLineType timeLineType = CalendarTimeLineType.Month, int expandDayPrecision = 0);
    }
}