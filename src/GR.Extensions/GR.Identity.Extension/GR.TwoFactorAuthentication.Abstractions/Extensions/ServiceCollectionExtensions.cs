using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.TwoFactorAuthentication.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.TwoFactorAuthentication.Abstractions.Extensions
{
    /// <summary>
    /// This class has the authority for register Two factor auth providers
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register two factor auth provider 
        /// </summary>
        /// <typeparam name="TAuthenticator"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterTwoFactorAuthenticatorProvider<TAuthenticator>(this IServiceCollection services)
            where TAuthenticator : class, ITwoFactorAuthService
        {
            services.AddGearScoped<ITwoFactorAuthService, TAuthenticator>();
            TwoFactorAuthEvents.RegisterEvents();
            return services;
        }
    }
}