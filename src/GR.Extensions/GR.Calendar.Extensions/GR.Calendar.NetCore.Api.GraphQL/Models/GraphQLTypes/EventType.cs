using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using GR.Calendar.Abstractions;
using GR.Calendar.Abstractions.Models;
using GR.Identity.Abstractions;

namespace GR.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes
{
    public class EventType : ObjectGraphType<CalendarEvent>
    {
        public EventType(ICalendarDbContext dbContext, IUserManager<GearUser> userManager)
        {
            Field(x => x.Id, type: typeof(IdGraphType));
            Field(x => x.Title).Description("Event title");
            Field(x => x.Details).Description("Event details");
            Field(x => x.StartDate).Description("Event details");
            Field(x => x.EndDate).Description("Event details");
            Field(x => x.Author).Description("Event author");
            Field(x => x.Version).Description("Event version");
            Field(x => x.ModifiedBy).Description("Event author");
            Field(x => x.Created).Description("Event author");
            Field(x => x.Changed).Description("Event author");
            Field(x => x.Organizer, type: typeof(IdGraphType)).Description("Organizer id");
            Field<UserType>("organizerInfo", resolve: ctx => userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(ctx.Source.Organizer)).Result);
            Field<ListGraphType<EventMemberType>>("InvitedUsers",
                resolve: context => dbContext.EventMembers.Where(x => x.EventId.Equals(context.Source.Id)).ToListAsync().Result);
        }
    }
}
