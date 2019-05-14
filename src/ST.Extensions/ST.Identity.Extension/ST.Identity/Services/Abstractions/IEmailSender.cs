using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.Identity.Models.EmailViewModels;

namespace ST.Identity.Services.Abstractions
{
    public interface IEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        /// <summary>
        /// Send message to multiple users
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendEmailAsync(IEnumerable<string> emails, string subject, string message);

        /// <summary>
        /// Update email settings
        /// </summary>
        /// <param name="newSettings"></param>
        /// <returns></returns>
        ResultModel ChangeSettings(EmailSettingsViewModel newSettings);
    }
}