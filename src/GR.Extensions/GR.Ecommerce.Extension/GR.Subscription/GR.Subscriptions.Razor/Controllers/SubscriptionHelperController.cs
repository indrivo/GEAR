using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Email.Abstractions;
using GR.Identity.Abstractions;
using GR.Subscriptions.Razor.ViewModels.QuotationViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserManager<ApplicationUser> _userManager;
        #endregion

        public SubscriptionHelperController(IEmailSender emailSender, IUserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
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
            var subject = "Quotation request for Enterprise";
            var message = new StringBuilder("Need template");
            await _emailSender.SendEmailAsync(emails, subject, message.ToString());
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
            var subject = "Quotation request for additional services";
            var message = new StringBuilder("Need template");
            await _emailSender.SendEmailAsync(emails, subject, message.ToString());
            response.IsSuccess = true;
            return Json(response);
        }
    }
}