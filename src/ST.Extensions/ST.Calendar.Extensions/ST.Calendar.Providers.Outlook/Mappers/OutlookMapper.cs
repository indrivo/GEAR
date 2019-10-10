using System.Linq;
using Microsoft.Graph;
using ST.Calendar.Abstractions.Models.ViewModels;

namespace ST.Calendar.Providers.Outlook.Mappers
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
                    DateTime = model.StartDate.ToLongDateString(),
                    TimeZone = "Pacific Standard Time"
                },
                End = new DateTimeTimeZone
                {
                    DateTime = model.EndDate.ToLongDateString(),
                    TimeZone = "Pacific Standard Time"
                },
                Location = new Location
                {
                    DisplayName = model.Location
                },
                Attendees = model?.InvitedUsers.Select(x => new Attendee
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
    }
}
