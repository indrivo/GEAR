using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using GR.Calendar.Abstractions.ExternalProviders.Extensions;
using GR.Calendar.Providers.Outlook.Helpers;
using GR.Core.Helpers;

namespace GR.Calendar.Providers.Outlook.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register outlook provider 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="outlookOptions"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterOutlookCalendarProvider(this IServiceCollection serviceCollection, Action<MsAuthorizationSettings> outlookOptions)
        {
            Arg.NotNull(outlookOptions, nameof(RegisterOutlookCalendarProvider));
            serviceCollection.RegisterExternalCalendarProvider(options =>
            {
                options.ProviderName = nameof(OutlookCalendarProvider);
                options.ProviderType = typeof(OutlookCalendarProvider);
                options.DisplayName = "Outlook";
                options.FontAwesomeIcon = "microsoft";
            });

            var authSettings = new MsAuthorizationSettings();
            outlookOptions(authSettings);
            OutlookAuthSettings.SetAuthSettings(authSettings);

            serviceCollection.AddAuthentication()
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
