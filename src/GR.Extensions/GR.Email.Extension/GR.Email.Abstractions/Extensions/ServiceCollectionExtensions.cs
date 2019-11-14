using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Extensions;
using GR.Email.Abstractions.Models.EmailViewModels;

namespace GR.Email.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register email module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEmailModule<TEmailSender>(this IServiceCollection services)
            where TEmailSender : class, IEmailSender
        {
            services.AddTransient<IEmailSender, TEmailSender>();
            return services;
        }

        /// <summary>
        /// Bind Email settings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection BindEmailSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureWritable<EmailSettingsViewModel>(configuration.GetSection("EmailSettings"));
            return services;
        }
    }
}
