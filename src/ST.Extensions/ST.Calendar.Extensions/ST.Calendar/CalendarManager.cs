using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Enums;
using ST.Calendar.Abstractions.Models;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.MultiTenant.Abstractions;

namespace ST.Calendar
{
    public class CalendarManager : ICalendarManager
    {
        #region Injectable
        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly ICalendarDbContext _context;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;
        #endregion


        public CalendarManager(ICalendarDbContext context, IUserManager<ApplicationUser> userManager, IOrganizationService<Tenant> organizationService)
        {
            _context = context;
            _userManager = userManager;
            _organizationService = organizationService;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get event by id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<ResultModel<CalendarEvent>> GetEventByIdAsync(Guid? eventId)
        {
            var response = new ResultModel<CalendarEvent>();
            if (!eventId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            var evt = await _context.CalendarEvents
                .Include(x => x.EventMembers)
                .FirstOrDefaultAsync(x => x.Id.Equals(eventId));
            if (evt == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            response.IsSuccess = true;
            response.Result = evt;
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all events organized by current user
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<CalendarEvent>>> GetAllEventsOrganizedByMeAsync()
        {
            var response = new ResultModel<IEnumerable<CalendarEvent>>();
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "You are not authorized"));
                return response;
            }

            var user = currentUserRequest.Result;

            var events = await _context.CalendarEvents
                .Include(x => x.EventMembers)
                .Where(x => x.Organizer.Equals(user.Id.ToGuid()) && !x.IsDeleted)
                .ToListAsync();

            response.IsSuccess = true;
            response.Result = events;
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get events
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<CalendarEvent>>> GetEventsAsync(Guid? userId, DateTime startDate, DateTime endDate)
        {
            var response = new ResultModel<IEnumerable<CalendarEvent>>();
            var events = await _context.CalendarEvents
                .Include(x => x.EventMembers)
                .Where(x => x.EventMembers.Any(member => member.UserId.Equals(userId))
                            || x.Organizer.Equals(userId)
                            && !x.IsDeleted && x.StartDate >= startDate && x.EndDate <= endDate)
                .ToListAsync();

            response.IsSuccess = true;
            response.Result = events;
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get my events
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<CalendarEvent>>> GetMyEventsAsync(DateTime startDate, DateTime endDate)
        {
            var response = new ResultModel<IEnumerable<CalendarEvent>>();
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "You are not authorized"));
                return response;
            }

            var user = currentUserRequest.Result;
            return await GetEventsAsync(user.Id.ToGuid(), startDate, endDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="origin">Default is today date</param>
        /// <param name="timeLineType">Specify the interval of time</param>
        /// <param name="expandDayPrecision">Specify the expand interval in days, default is zero</param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<CalendarEvent>>> GetUserEventsByTimeLineAsync(Guid? userId, DateTime? origin, CalendarTimeLineType timeLineType = CalendarTimeLineType.Month, int expandDayPrecision = 0)
        {
            var response = new ResultModel<IEnumerable<CalendarEvent>>();
            if (!userId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "User not specified"));
                return response;
            }
            var today = origin ?? DateTime.Today;

            switch (timeLineType)
            {
                case CalendarTimeLineType.Day:
                    response = await GetEventsAsync(userId, today, today);
                    break;
                case CalendarTimeLineType.Week:
                    var weekStart = today.AddDays(-((int)today.DayOfWeek + expandDayPrecision));
                    var weekEnd = weekStart.AddDays(7 + expandDayPrecision).AddSeconds(-1);
                    response = await GetEventsAsync(userId, weekStart, weekEnd);
                    break;
                case CalendarTimeLineType.Month:
                    var monthStart = today.AddDays(1 - today.Day + expandDayPrecision);
                    var monthEnd = monthStart.AddMonths(1).AddDays(expandDayPrecision).AddSeconds(-1);
                    response = await GetEventsAsync(userId, monthStart, monthEnd);
                    break;
                default:
                    response.Errors.Add(new ErrorModel(string.Empty, "No line type indicated"));
                    break;
            }

            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> CreateEventAsync(BaseEventViewModel evt)
        {
            var response = new ResultModel<Guid>();
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "You are not authorized"));
                return response;
            }

            var user = currentUserRequest.Result;
            evt.Organizer = user.Id.ToGuid();
            await _context.CalendarEvents.AddAsync(evt);
            var dbResult = await _context.PushAsync();
            if (!dbResult.IsSuccess)
            {
                response.Errors = dbResult.Errors;
                return response;
            }

            response.IsSuccess = true;
            response.Result = evt.Id;
            return response;
        }


        /// <inheritdoc />
        /// <summary>
        /// Add or update event members
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddOrUpdateMembersToEventAsync(Guid? eventId, IList<Guid> users)
        {
            var eventRequest = await GetEventByIdAsync(eventId);
            if (eventRequest.IsSuccess) return eventRequest.ToBase();
            var evt = eventRequest.Result;

            var oldMembers = evt.EventMembers.Select(x => x.UserId).ToList();
            var newMembers = users.Except(oldMembers)
                .Where(x => _organizationService.IsUserPartOfOrganizationAsync(x, evt.TenantId).ExecuteAsync())
                .Select(x => new EventMember
                {
                    UserId = x,
                    Event = evt
                }).ToList();

            var removeMembers = oldMembers.Except(users)
                .Select(x => evt.EventMembers
                    .FirstOrDefault(c => c.UserId.Equals(x))).ToList();

            if (newMembers.Any()) await _context.EventMembers.AddRangeAsync(newMembers);
            if (removeMembers.Any()) _context.EventMembers.RemoveRange(removeMembers);

            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete event permanent
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteEventPermanentlyAsync(Guid? eventId)
        {
            var response = new ResultModel();
            if (!eventId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            var evt = await _context.CalendarEvents.FirstOrDefaultAsync(x => x.Id.Equals(eventId));
            if (evt == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            _context.CalendarEvents.Remove(evt);
            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete event permanent
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteEventLogicallyAsync(Guid? eventId)
        {
            var response = new ResultModel();
            if (!eventId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            var evt = await _context.CalendarEvents.FirstOrDefaultAsync(x => x.Id.Equals(eventId));
            if (evt == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            evt.IsDeleted = true;
            _context.Update(evt);

            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete event permanent
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<ResultModel> RestoreEventLogicallyAsync(Guid? eventId)
        {
            var response = new ResultModel();
            if (!eventId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            var evt = await _context.CalendarEvents.FirstOrDefaultAsync(x => x.Id.Equals(eventId));
            if (evt == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Event not found"));
                return response;
            }

            evt.IsDeleted = false;
            _context.Update(evt);

            return await _context.PushAsync();
        }
    }
}