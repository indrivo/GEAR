using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using GR.Calendar.Abstractions;
using GR.Identity.Abstractions;

namespace GR.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes
{
    public class UserType : ObjectGraphType<GearUser>
    {
        public UserType(ICalendarDbContext dbContext)
        {
            Field(x => x.Id, type: typeof(IdGraphType), nullable: false).Description("User id");
            Field(x => x.Email).Description("User email");
            Field("FirstName", x => x.FirstName);
            Field("LastName", x => x.LastName);
            Field(x => x.UserName);
            Field<ListGraphType<EventType>>("events", resolve: ctx => dbContext.CalendarEvents.Where(x => x.Organizer.Equals(ctx.Source.Id)).ToListAsync().Result);
        }
    }
}
