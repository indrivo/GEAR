using GraphiQl;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Calendar.NetCore.Api.GraphQL.Models.GraphQLTypes;
using ST.Calendar.NetCore.Api.GraphQL.Queries;
using ST.Calendar.NetCore.Api.GraphQL.Schemas;
using ST.Calendar.NetCore.Api.GraphQL.Schemas.Contracts;
using System;

namespace ST.Calendar.NetCore.Api.GraphQL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add calendar graph
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarGraphQLApi(this CalendarServiceCollection serviceCollection)
        {
            serviceCollection.Services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            serviceCollection.Services.AddSingleton<CalendarQuery>();

            //Register types
            serviceCollection.Services.AddTransient<EventType>();
            serviceCollection.Services.AddTransient<UserType>();
            serviceCollection.Services.AddTransient<EventMemberType>();

            var sp = serviceCollection.Services.BuildServiceProvider();
            serviceCollection.Services.AddSingleton<ICalendarSchema>(new CalendarSchema(new FuncDependencyResolver(type => sp.GetService(type))));
            return serviceCollection;
        }

        /// <summary>
        /// Use graphQL
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCalendarGrapHQL(this IApplicationBuilder app)
        {
            app.UseGraphiQl("/api/CalendarGraphQL");
            return app;
        }
    }
}
