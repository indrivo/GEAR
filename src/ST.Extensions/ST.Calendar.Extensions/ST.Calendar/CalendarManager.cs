using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Enums;
using ST.Calendar.Abstractions.Models;
using ST.Core.Helpers;
using ST.TaskManager.Abstractions;
using Task = ST.TaskManager.Abstractions.Models.Task;

namespace ST.Calendar
{
    public class CalendarManager : ICalendarManager
    {
        private readonly ITaskManagerContext _taskManagerContext;

        public CalendarManager(ITaskManagerContext taskManagerContext)
        {
            _taskManagerContext = taskManagerContext;
        }

        public async Task<ResultModel<IEnumerable<CalendarEvent>>> GetEventsAsync(CalendarTimeLineType timeLine, DateTime dateTime, Guid userId)
        {
            var eventsList = new List<CalendarEvent>();

            eventsList.AddRange(await GetEventsFromTasks(timeLine, dateTime, userId));
            //TODO        : if another events provider appears just add them to eventsList object;
            //TODO EXAMPLE: ventsList.AddRange(await GetEventsFromOutLook(timeLine, dateTime));

            return new ResultModel<IEnumerable<CalendarEvent>>
            {
                IsSuccess = true,
                Result = eventsList
            };
        }

        private async Task<IEnumerable<CalendarEvent>> GetEventsFromTasks(CalendarTimeLineType timeLine, DateTime dateTime, Guid userId)
        {
            List<Task> tasks;

            switch (timeLine)
            {
                case CalendarTimeLineType.Day:
                {
                    tasks = await _taskManagerContext.Tasks.Where(x => x.StartDate.Date == dateTime.Date && x.UserId == userId).ToListAsync();
                    break;
                }
                case CalendarTimeLineType.Week:
                {
                    tasks = await _taskManagerContext.Tasks.Where(x => x.StartDate.DayOfWeek == dateTime.DayOfWeek && x.UserId == userId).ToListAsync();
                    break;
                }
                case CalendarTimeLineType.Month:
                {
                    tasks = await _taskManagerContext.Tasks.Where(x => x.StartDate.Month == dateTime.Month && x.UserId == userId).ToListAsync();
                    break;
                }
                case CalendarTimeLineType.Year:
                {
                    tasks = await _taskManagerContext.Tasks.Where(x => x.StartDate.Year == dateTime.Year && x.UserId == userId).ToListAsync();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeLine), timeLine, null);
            }

            return tasks.Select(item => new CalendarEvent
            {
                Name = $"Task: #{item.TaskNumber} {item.Name}", Description = item.Description, StartDate = item.StartDate, EndDate = item.EndDate
            }).ToList();
        }
    }
}
