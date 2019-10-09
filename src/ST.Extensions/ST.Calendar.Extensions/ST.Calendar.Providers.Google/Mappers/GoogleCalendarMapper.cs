using System;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using ST.Calendar.Abstractions.Models.ViewModels;

namespace ST.Calendar.Providers.Google.Mappers
{
    public static class GoogleCalendarMapper
    {
        public static Event Map(GetEventViewModel model)
        {
            var evt = new Event
            {
                Summary = model.Title,
                Description = model.Details,
                Start = new EventDateTime
                {
                    DateTime = model.StartDate,
                    TimeZone = TimeZoneInfo.Local.StandardName
                },
                End = new EventDateTime
                {
                    DateTime = model.EndDate,
                    TimeZone = TimeZoneInfo.Local.StandardName
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

        public static Event Map(Event target, GetEventViewModel source)
        {


            return target;
        }
    }
}
