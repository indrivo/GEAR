using System;
using GR.Core.Extensions;
using GR.EmailTwoFactorAuth.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GR.EmailTwoFactorAuth.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Email auth configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection EmailTwoFactorAuthConfiguration(this IServiceCollection services, Action<EmailTwoFactorConfiguration> options = null)
        {
            var config = new EmailTwoFactorConfiguration();
            options?.Invoke(config);
            services.AddGearSingleton(config);
            return services;
        }
    }
}
