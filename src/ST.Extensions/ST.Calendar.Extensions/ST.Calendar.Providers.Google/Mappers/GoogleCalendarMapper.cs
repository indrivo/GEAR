using System;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using ST.Calendar.Abstractions.Models.ViewModels;

namespace ST.Calendar.Providers.Google.Mappers
{
    public static class GoogleCalendarMapper
    {
        /// <summary>
        /// Map
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Event Map(GetEventViewModel model)
        {
            if (model == null) return new Event();
            var evt = new Event
            {
                Summary = model.Title,
                Description = model.Details,
                Start = new EventDateTime
                {
                    DateTime = model.StartDate,
                    //TimeZone = TimeZoneInfo.Local.StandardName
                },
                End = new EventDateTime
                {
                    DateTime = model.EndDate,
                    //TimeZone = TimeZoneInfo.Local.StandardName
                },
                Creator = new Event.CreatorData
                {
                    DisplayName = "ISODMS Creator",
                    Email = "admin@admin.com"
                },
                Organizer = new Event.OrganizerData
                {
                    DisplayName = model.OrganizerInfo.FirstName + model.OrganizerInfo.LastName,
                    Email = model.OrganizerInfo.Email
                },
                Location = model.Location,
                Created = model.Created,
                Attendees = model.InvitedUsers?.Select(x => new EventAttendee
                {
                    Email = x.Email
                }).ToList(),
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new[]
                    {
                        new EventReminder {Method = "email", Minutes = 10},
                        new EventReminder {Method = "sms", Minutes = 10},
                    }
                }
            };

            return evt;
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Event Map(Event target, GetEventViewModel source)
        {
            if (target == null || source == null) return target;
            target.Summary = source.Title;
            target.Description = source.Details;

            target.Start = new EventDateTime
            {
                DateTime = source.StartDate
            };

            target.End = new EventDateTime
            {
                DateTime = source.EndDate
            };

            target.Location = source.Location;

            target.Attendees = source.InvitedUsers?.Select(x => new EventAttendee
            {
                Email = x.Email
            }).ToList();

            return target;
        }
    }
}
