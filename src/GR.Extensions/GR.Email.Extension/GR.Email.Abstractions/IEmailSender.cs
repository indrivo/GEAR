using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Email.Abstractions.Helpers;
using GR.Email.Abstractions.Models.EmailViewModels;

namespace GR.Email.Abstractions
{
    public interface IEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        /// <summary>
        /// Send message to multiple users
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        Task SendEmailAsync(IEnumerable<string> emails, string subject, string message, bool isBodyHtml = true);

        /// <summary>
        /// Send mail
        /// </summary>
        /// <typeparam name="TConfigurator"></typeparam>
        /// <param name="conf"></param>
        /// <returns></returns>
        Task SendEmailAsync<TConfigurator>(Action<TConfigurator> conf = null) where TConfigurator : BaseMailTemplateGenerator;

        /// <summary>
        /// Update email settings
        /// </summary>
        /// <param name="newSettings"></param>
        /// <returns></returns>
        ResultModel ChangeSettings(EmailSettingsViewModel newSettings);

        /// <summary>
        /// Check if is valid email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        Task<ResultModel> IsValidEmailAsync(string emailAddress);
    }
}