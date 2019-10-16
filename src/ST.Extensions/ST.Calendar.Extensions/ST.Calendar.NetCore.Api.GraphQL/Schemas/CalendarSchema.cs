using GraphQL;
using GraphQL.Types;
using ST.Calendar.NetCore.Api.GraphQL.Queries;
using System;
using ST.Calendar.NetCore.Api.GraphQL.Schemas.Contracts;

namespace ST.Calendar.NetCore.Api.GraphQL.Schemas
{
    public class CalendarSchema : Schema, ICalendarSchema
    {
        [Obsolete]
        public CalendarSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<CalendarQuery>();
        }
    }
}
