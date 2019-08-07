using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ST.Core.Helpers;
using ST.Email.Abstractions;
using ST.Email.Abstractions.Models.EmailViewModels;

namespace ST.Email
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Email settings
        /// </summary>
        private readonly IOptions<EmailSettingsViewModel> _options;

        public EmailSender(IOptions<EmailSettingsViewModel> options)
        {
            if (options.Value == null) throw new Exception("Email settings not register in appsettings file");
            _options = options;
        }

        /// <inheritdoc />
        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendEmailAsync(string email, string subject, string message) => await SendEmailAsync(new List<string>
        {
            email
        }, subject, message);

        /// <inheritdoc />
        /// <summary>
        /// Send email message to multiple targets
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendEmailAsync(IEnumerable<string> emails, string subject, string message)
        {
            if (!_options.Value.Enabled) return;
            var settings = _options.Value;
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Port = settings.Port;
                    client.Host = settings.Host;
                    client.EnableSsl = true;
                    client.Timeout = settings.Timeout;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(settings.NetworkCredential.Email, settings.NetworkCredential.Password);

                    var mailMessage = new MailMessage
                    {
                        BodyEncoding = Encoding.UTF8,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                        From = new MailAddress(settings.NetworkCredential.Email),
                        Subject = subject,
                        Body = message
                    };

                    foreach (var emailTo in emails)
                    {
                        mailMessage.To.Add(emailTo);
                    }

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Update email settings
        /// </summary>
        /// <param name="newSettings"></param>
        /// <returns></returns>
        public virtual ResultModel ChangeSettings(EmailSettingsViewModel newSettings)
        {
            var result = new ResultModel();
            _options.Value.NetworkCredential = newSettings.NetworkCredential;
            _options.Value.Enabled = newSettings.Enabled;
            _options.Value.Host = newSettings.Host;
            _options.Value.Port = newSettings.Port;
            _options.Value.Timeout = newSettings.Timeout;
            _options.Value.EnableSsl = newSettings.EnableSsl;
            //TODO: Update email settings into appsettings.envName.json
            return result;
        }
    }
}