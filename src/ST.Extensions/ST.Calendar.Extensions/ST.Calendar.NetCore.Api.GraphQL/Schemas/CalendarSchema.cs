using GraphQL;
using GraphQL.Types;
using ST.Calendar.NetCore.Api.GraphQL.Queries;
using System;

namespace ST.Calendar.NetCore.Api.GraphQL.Schemas
{
    public class CalendarSchema : Schema
    {
        [Obsolete]
        public CalendarSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<CalendarQuery>();
        }
    }
}
