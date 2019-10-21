using GraphQL;
using GraphQL.Types;
using GR.Calendar.NetCore.Api.GraphQL.Queries;
using System;
using GR.Calendar.NetCore.Api.GraphQL.Schemas.Contracts;

namespace GR.Calendar.NetCore.Api.GraphQL.Schemas
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
