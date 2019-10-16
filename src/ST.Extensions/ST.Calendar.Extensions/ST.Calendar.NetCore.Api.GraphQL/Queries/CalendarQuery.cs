using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using ST.Calendar.Abstractions;
using ST.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes;
using System;
using System.Linq;

namespace ST.Calendar.NetCore.Api.GraphQL.Queries
{
    public class CalendarQuery : ObjectGraphType
    {
        public CalendarQuery(ICalendarDbContext dbContext)
        {
            Field<EventType>(
                "events",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "id", Description = "Category id" }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>("id");
                    return dbContext.CalendarEvents.Where(x => x.Id.Equals(id)).ToListAsync().Result;
                }
            );
        }
    }
}
