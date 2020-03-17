using GR.Core.Extensions;
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
        /// <returns></returns>
        public static IServiceCollection AddPhoneVerificationModule<TAuthy>(this IServiceCollection services)
            where TAuthy : class, IAuthy
        {
            services.AddGearSingleton<IAuthy, TAuthy>();
            return services;
        }
    }
}