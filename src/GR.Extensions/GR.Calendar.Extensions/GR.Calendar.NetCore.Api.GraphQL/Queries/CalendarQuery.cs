using System;
using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using GR.Calendar.Abstractions;
using GR.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes;
using GR.Identity.Abstractions;

namespace GR.Calendar.NetCore.Api.GraphQL.Queries
{
    public class CalendarQuery : ObjectGraphType
    {
        public CalendarQuery(ICalendarDbContext dbContext, IUserManager<ApplicationUser> userManager)
        {
            Field<ListGraphType<EventType>>(
                "events",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "Event id" },
                    new QueryArgument<DateGraphType> { Name = "startDate" },
                    new QueryArgument<DateGraphType> { Name = "endDate" }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid?>("id");
                    if (id != null)
                    {
                        return dbContext.CalendarEvents.Where(x => x.Id.Equals(id)).ToListAsync().Result;
                    }

                    return dbContext.CalendarEvents.ToListAsync().Result;
                });

            Field<ListGraphType<UserType>>("users",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "User id" }
                ),
                resolve: ctx => userManager.UserManager.Users.ToListAsync());
        }
    }
}
