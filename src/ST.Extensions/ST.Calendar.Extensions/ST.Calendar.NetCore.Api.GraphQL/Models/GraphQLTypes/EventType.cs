using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Models;
using ST.Core.Extensions;
using ST.Identity.Abstractions;

namespace ST.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes
{
    public class EventType : ObjectGraphType<CalendarEvent>
    {
        public EventType(ICalendarDbContext dbContext, IUserManager<ApplicationUser> userManager)
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
            Field(x => x.Organizer, type: typeof(IdGraphType), nullable: false).Description("Organizer id");
            Field<UserType>("organizerInfo", resolve: ctx => userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.ToGuid().Equals(ctx.Source.Organizer)).Result);
            Field<ListGraphType<EventMemberType>>("InvitedUsers",
                resolve: context => dbContext.EventMembers.Where(x => x.EventId.Equals(context.Source.Id)).ToListAsync().Result);
        }
    }
}
