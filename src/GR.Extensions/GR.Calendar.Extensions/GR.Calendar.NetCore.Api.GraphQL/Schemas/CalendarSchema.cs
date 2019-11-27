using GraphQL;
using GraphQL.Types;
using GR.Calendar.NetCore.Api.GraphQL.Queries;
using GR.Calendar.NetCore.Api.GraphQL.Schemas.Contracts;

namespace GR.Calendar.NetCore.Api.GraphQL.Schemas
{
    public class CalendarSchema : Schema, ICalendarSchema
    {
        public CalendarSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<CalendarQuery>();
        }
    }
}
