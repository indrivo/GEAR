using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Email.Abstractions;
using GR.Email.Abstractions.Helpers;
using GR.Email.Abstractions.Models.EmailViewModels;
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable 162

namespace GR.Email
{
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
        public virtual async Task<ResultModel> SendEmailAsync(IEnumerable<string> emails, string subject, string message, bool isBodyHtml = true)
        {
            var result = new ResultModel();
            var mails = emails?.ToList() ?? new List<string>();
            if (!_options.Value.Enabled || !mails.Any())
            {
                result.AddError("Invalid parameters or service is disabled");
                return result;
            }
            var settings = _options.Value;
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Port = settings.Port;
                    client.Host = settings.Host;
                    client.EnableSsl = settings.EnableSsl;
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

                    foreach (var emailTo in mails)
                    {
                        if (!emailTo.IsValidEmail()) continue;
                        mailMessage.To.Add(emailTo);
                    }

                    await client.SendMailAsync(mailMessage);
                }

                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.AddError(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Send mail 
        /// </summary>
        /// <typeparam name="TConfigurator"></typeparam>
        /// <param name="conf"></param>
        /// <returns></returns>
        public virtual async Task SendEmailAsync<TConfigurator>(Action<TConfigurator> conf = null) where TConfigurator : BaseMailTemplateGenerator
        {
            var configuration = Activator.CreateInstance<TConfigurator>();
            conf?.Invoke(configuration);
            await SendEmailAsync(configuration.Emails, configuration.Subject, await configuration.GenerateAsync());
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

        /// <summary>
        /// Is valid email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public async Task<ResultModel> IsValidEmailAsync(string emailAddress)
        {
            //TODO: Need for all smtp servers
            var response = new ResultModel
            {
                IsSuccess = true
            };
            return response;

            if (!emailAddress.IsValidEmail())
            {
                response.AddError("Invalid email");
                return response;
            }

            var host = emailAddress.Split('@');
            var hostname = host[1];
            var addresses = ResolveDns($"smtp.{hostname}") ?? ResolveDns(hostname);

            if (addresses == null)
            {

            }

            try
            {
                var endPt = new IPEndPoint(addresses[0], 25);
                var s = new Socket(endPt.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                await s.ConnectAsync(endPt);
                if (s.Connected)
                {
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            response.AddError("Invalid host for email");
            return response;
        }

        private static IPAddress[] ResolveDns(string host)
        {
            try
            {
                return Dns.GetHostAddresses(host);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        /// Get provider
        /// </summary>
        /// <returns></returns>
        public virtual object GetProvider() => this;

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SendAsync(string subject, string message, string to)
            => await SendEmailAsync(new List<string>
        {
            to
        }, subject, message);
    }
}