﻿using System.Linq;
using Microsoft.Graph;
using GR.Calendar.Abstractions.Models.ViewModels;

namespace GR.Calendar.Providers.Outlook.Mappers
{
    public static class OutlookMapper
    {
        /// <summary>
        /// Map
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Event Map(GetEventViewModel model)
        {
            if (model == null) return new Event();

            return new Event
            {
                Subject = model.Title,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = model.Details
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = model.StartDate.ToString("MM/dd/yyyy HH:mm:ss.fff"),
                    TimeZone = "GTB Standard Time"
                },
                End = new DateTimeTimeZone
                {
                    DateTime = model.EndDate.ToString("MM/dd/yyyy HH:mm:ss.fff"),
                    TimeZone = "GTB Standard Time"
                },
                Location = new Location
                {
                    DisplayName = model.Location
                },
                Attendees = model.InvitedUsers.Select(x => new Attendee
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = x.Email,
                        Name = x.FirstName + x.LastName,
                    },
                    Type = AttendeeType.Required
                })
            };
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Event Map(Event source, GetEventViewModel target)
        {
            if (source == null || target == null) return source;
            var newEv = Map(target);
            newEv.Id = source.Id;
            return newEv;
        }
    }
}
