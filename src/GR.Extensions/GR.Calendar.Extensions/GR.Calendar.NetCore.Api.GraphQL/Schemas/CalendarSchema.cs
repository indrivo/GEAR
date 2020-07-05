using GraphQL;
using GraphQL.Types;
using GR.Calendar.NetCore.Api.GraphQL.Queries;
using GR.Calendar.NetCore.Api.GraphQL.Schemas.Contracts;
using GR.Core.Helpers;

namespace GR.Calendar.NetCore.Api.GraphQL.Schemas
{
    public class CalendarSchema : Schema, ICalendarSchema
    {
        public CalendarSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = IoC.Resolve<CalendarQuery>();
        }

        public CalendarSchema(CalendarQuery query)
        {
            Query = query;
        }
    }
}
