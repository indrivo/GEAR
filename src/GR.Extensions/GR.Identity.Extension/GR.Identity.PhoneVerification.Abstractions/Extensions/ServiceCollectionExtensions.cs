using GR.Core.Extensions;
using GR.Identity.PhoneVerification.Abstractions.Events;
using GR.Identity.PhoneVerification.Abstractions.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.PhoneVerification.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add phone verification module
        /// </summary>
        /// <typeparam name="TAuthy"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPhoneVerificationModule<TAuthy>(this IServiceCollection services, IConfiguration configuration)
            where TAuthy : class, IAuthy
        {
            services.AddGearScoped<IAuthy, TAuthy>();
            services.ConfigureWritable<TwilioSettings>(configuration.GetSection("TwilioSettings"));
            return services;
        }

        /// <summary>
        /// Bind events
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection BindPhoneVerificationEvents(this IServiceCollection services)
        {
            PhoneVerificationEvents.RegisterEvents();
            return services;
        }
    }
}