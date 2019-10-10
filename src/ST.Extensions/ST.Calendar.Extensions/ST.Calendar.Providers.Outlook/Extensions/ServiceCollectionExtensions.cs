using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using ST.Calendar.Abstractions.ExternalProviders.Extensions;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Calendar.Providers.Outlook.Helpers;
using ST.Core.Helpers;

namespace ST.Calendar.Providers.Outlook.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static CalendarServiceCollection RegisterOutlookCalendarProvider(this CalendarServiceCollection serviceCollection, Action<MsAuthorizationSettings> outlookOptions)
        {
            Arg.NotNull(outlookOptions, nameof(RegisterOutlookCalendarProvider));
            serviceCollection.RegisterExternalCalendarProvider(options =>
            {
                options.ProviderName = nameof(OutlookCalendarProvider);
                options.ProviderType = typeof(OutlookCalendarProvider);
            });

            var authSettings = new MsAuthorizationSettings();
            outlookOptions(authSettings);
            OutlookAuthSettings.SetAuthSettings(authSettings);

            serviceCollection.Services.AddAuthentication()
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = authSettings.ClientId;
                    microsoftOptions.ClientSecret = authSettings.ClientSecretId;
                    microsoftOptions.SaveTokens = true;
                    microsoftOptions.Events.OnCreatingTicket = ctx =>
                    {
                        var tokens = ctx.Properties.GetTokens().ToList();

                        tokens.Add(new AuthenticationToken()
                        {
                            Name = "TicketCreated",
                            Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                        });

                        ctx.Properties.StoreTokens(tokens);

                        return Task.CompletedTask;
                    };

                    foreach (var scope in authSettings.Scopes)
                    {
                        microsoftOptions.Scope.Add(scope);
                    }
                });
            return serviceCollection;
        }
    }
}
