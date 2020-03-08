using GraphiQl;
using GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using GR.Calendar.Abstractions.Helpers.ServiceBuilders;
using GR.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes;
using GR.Calendar.NetCore.Api.GraphQL.Queries;
using GR.Calendar.NetCore.Api.GraphQL.Schemas;
using GR.Calendar.NetCore.Api.GraphQL.Schemas.Contracts;

namespace GR.Calendar.NetCore.Api.GraphQL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add calendar graph
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddCalendarGraphQlApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            serviceCollection.AddSingleton<CalendarQuery>();

            //Register types
            serviceCollection.AddTransient<EventType>();
            serviceCollection.AddTransient<UserType>();
            serviceCollection.AddTransient<EventMemberType>();

            var sp = serviceCollection.BuildServiceProvider();
            serviceCollection.AddSingleton<ICalendarSchema>(new CalendarSchema(new FuncDependencyResolver(type => sp.GetService(type))));
            return serviceCollection;
        }

        /// <summary>
        /// Use graphQL
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCalendarGraphQl(this IApplicationBuilder app)
        {
            app.UseGraphiQl("/api/CalendarGraphQL");
            return app;
        }
    }
}
