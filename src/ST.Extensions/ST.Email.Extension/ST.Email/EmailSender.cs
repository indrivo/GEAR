using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ST.Core.Abstractions;
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
        private readonly IWritableOptions<EmailSettingsViewModel> _options;

        public EmailSender(IWritableOptions<EmailSettingsViewModel> options)
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
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public virtual async Task SendEmailAsync(IEnumerable<string> emails, string subject, string message, bool isBodyHtml = true)
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
                        Body = message,
                        Priority = MailPriority.High,
                        IsBodyHtml = isBodyHtml
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
            _options.Update(options =>
            {
                options.NetworkCredential = newSettings.NetworkCredential;
                options.Enabled = newSettings.Enabled;
                options.Host = newSettings.Host;
                options.Port = newSettings.Port;
                options.Timeout = newSettings.Timeout;
                options.EnableSsl = newSettings.EnableSsl;
            });
            result.IsSuccess = true;
            return result;
        }
    }
}