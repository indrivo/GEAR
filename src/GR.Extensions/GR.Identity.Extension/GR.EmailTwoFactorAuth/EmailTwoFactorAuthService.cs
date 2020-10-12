using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.ErrorCodes;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Templates;
using GR.Email.Abstractions;
using GR.Email.Abstractions.Events;
using GR.Email.Abstractions.Events.EventArgs;
using GR.EmailTwoFactorAuth.Helpers;
using GR.EmailTwoFactorAuth.Models;
using GR.Identity.Abstractions;
using GR.TwoFactorAuthentication.Abstractions;
using GR.TwoFactorAuthentication.Abstractions.Events;
using GR.TwoFactorAuthentication.Abstractions.Events.EventArgs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace GR.EmailTwoFactorAuth
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class EmailTwoFactorAuthService : ITwoFactorAuthService
    {
        #region Injectable

        /// <summary>
        /// Inject email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Inject accessor
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Inject configuration
        /// </summary>
        private readonly EmailTwoFactorConfiguration _configuration;

        /// <summary>
        /// Inject localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;

        #endregion

        public EmailTwoFactorAuthService(IEmailSender emailSender, IHttpContextAccessor accessor, EmailTwoFactorConfiguration configuration, IStringLocalizer localizer)
        {
            _emailSender = emailSender;
            _accessor = accessor;
            _configuration = configuration;
            _localizer = localizer;
        }

        /// <summary>
        /// Send verification code
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> SendCodeAsync(GearUser user)
        {
            var result = new ResultModel<string>();
            if (!user.EmailConfirmed)
            {
                EmailEvents.Events.TriggerSendConfirmEmail(new SendConfirmEmailEventArgs
                {
                    HttpContext = _accessor.HttpContext,
                    Email = user.Email
                });
                result.AddError(ResultModelCodes.EmailNotConfirmed, "Email not confirmed, a message to confirm has been sent");
                return result;
            }

            var code = CodeGenerator.GenerateCode(_configuration.CodeLength);
            var subject = $"{GearApplication.ApplicationName} Verification Code";
            var message = new StringBuilder();
            const string lineDelimiter = "<br/>";
            message.AppendLine("It looks like you tried to log in to the application, please enter the code below");
            message.AppendLine(lineDelimiter + lineDelimiter);
            message.AppendLine($"Date: {DateTime.Now}");
            message.AppendLine(lineDelimiter);
            message.AppendLine($"Account: {user.Email}");
            message.AppendLine(lineDelimiter + lineDelimiter);
            message.AppendLine("Enter this 6 digit code on the sign in page to confirm your identity:");
            message.AppendLine(lineDelimiter + lineDelimiter);
            message.AppendLine($"{code}");
            message.AppendLine(lineDelimiter + lineDelimiter);
            message.AppendLine($"Yours securely, Team {GearApplication.ApplicationName}");

            if (_configuration.UseHtmlTemplate)
            {
                var templateResponse = TemplateManager.GetTemplateBody(_configuration.HtmlTemplateName);
                if (templateResponse.IsSuccess)
                {
                    var args = new Dictionary<string, string>
                    {
                        { "Title", subject },
                        { "Content", message.ToString() }
                    };

                    message = new StringBuilder(templateResponse.Result.Inject(args));
                }
            }


            await _emailSender.SendEmailAsync(user.Email, subject, message.ToString());
            result.IsSuccess = true;
            CodeProcessor.Push(user.Id, code);
            result.Result = "Verification code sent";
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> ValidateReceivedCodeAsync(GearUser user, string code)
        {
            var result = new ResultModel<string>();
            var match = CodeProcessor.Match(user.Id, code);
            if (match)
            {
                result.IsSuccess = true;
                result.Result = "Code passed";
                TwoFactorAuthEvents.Events.TwoFactorVerified(new SecondFactorVerifiedEventArgs
                {
                    HttpContext = _accessor.HttpContext,
                    Data = new Dictionary<string, string>
                    {
                        { "code", code }
                    },
                    UserId = user.Id,
                    AuthMethod = "Email"
                });
                return result;
            }

            result.Result = "The code is invalid, please try again";
            result.AddError(result.Result);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Action text
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<string> GetActionMessageAsync(GearUser user)
        {
            var message = _localizer.GetString("enter_email_verification_code", user?.Email);
            return await Task.FromResult(message);
        }
    }
}