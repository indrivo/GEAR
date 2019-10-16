using GraphQL.Types;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Models;

namespace ST.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes
{
    public class EventType : ObjectGraphType<CalendarEvent>
    {
        public EventType(ICalendarDbContext dbContext)
        {
            //Field(x => x.Id).Description("Event id");
            Field(x => x.Title).Description("Event title");
            Field(x => x.Details).Description("Event details");
            Field(x => x.StartDate).Description("Event details");
            Field(x => x.EndDate).Description("Event details");
            Field(x => x.Author).Description("Event author");
            Field(x => x.Version).Description("Event version");
            Field(x => x.ModifiedBy).Description("Event author");
            Field(x => x.Created).Description("Event author");
            Field(x => x.Changed).Description("Event author");
        }
    }
}
