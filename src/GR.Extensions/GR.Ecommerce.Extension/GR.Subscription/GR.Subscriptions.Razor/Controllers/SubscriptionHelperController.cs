using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Templates;
using GR.Email.Abstractions;
using GR.Email.Abstractions.Helpers;
using GR.Identity.Abstractions;
using GR.Subscriptions.Razor.ViewModels.QuotationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace GR.Subscriptions.Razor.Controllers
{
    [AllowAnonymous]
    public class SubscriptionHelperController : Controller
    {
        #region Injectable
        /// <summary>
        /// Inject email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject env service
        /// </summary>
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        public SubscriptionHelperController(IEmailSender emailSender, IUserManager<GearUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Request a quotation as an enterprise
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RequestEnterpriseQuotation([Required] EnterpriseQuotationViewModel model)
        {
            var response = new ResultModel();
            if (!ModelState.IsValid) return Json(response.AttachModelState(ModelState));
            var emails = (await _userManager.UserManager.GetUsersInRoleAsync(GlobalResources.Roles.ADMINISTRATOR))
                .Select(x => x.Email).ToList();
            const string subject = "Quotation request for Enterprise";
            var templateRequest = TemplateManager.GetTemplateBody("enterprise-quotation");
            if (!templateRequest.IsSuccess) return Json(response);
            var template = templateRequest.Result;
            var message = template.Inject(model).Inject(new Dictionary<string, string>
            {
                { "Frequency", model.FrequencyOfPayment.ToString() }
            });
            if (_hostingEnvironment.IsDevelopment())
            {
                emails = new List<string> {
                    EmailResources.SystemTestEmails.OUTLOOK_RECEIVER_EMAIL,
                    EmailResources.SystemTestEmails.GMAIL_RECEIVER_EMAIL
                };
            }
            await _emailSender.SendEmailAsync(emails, subject, message);
            response.IsSuccess = true;
            return Json(response);
        }

        /// <summary>
        /// Request a quotation for additional services
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RequestAdditionalServices([Required] AdditionalServicesViewModel model)
        {
            var response = new ResultModel();
            if (!ModelState.IsValid) return Json(response.AttachModelState(ModelState));
            var emails = (await _userManager.UserManager.GetUsersInRoleAsync(GlobalResources.Roles.ADMINISTRATOR))
                .Select(x => x.Email).ToList();
            const string subject = "Quotation request for additional services";
            var templateRequest = TemplateManager.GetTemplateBody("quotation-additional-services");
            if (!templateRequest.IsSuccess) return Json(response);
            var template = templateRequest.Result;
            var message = template.Inject(model).Inject(new Dictionary<string, string>
            {
                { "ImplementationText", model.RequestForNewImplementation
                    ? "New implementation" : "Existing implementation" }
            });

            if (_hostingEnvironment.IsDevelopment())
            {
                emails = new List<string> {
                    EmailResources.SystemTestEmails.OUTLOOK_RECEIVER_EMAIL,
                    EmailResources.SystemTestEmails.GMAIL_RECEIVER_EMAIL
                };
            }

            await _emailSender.SendEmailAsync(emails, subject, message);
            response.IsSuccess = true;
            return Json(response);
        }
    }
}