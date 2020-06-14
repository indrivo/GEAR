using System.Linq;
using GR.ApplePay.Abstractions;
using GR.ApplePay.Razor.ViewModels;
using GR.Core.Extensions;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.ApplePay.Razor.Controllers
{
    [AllowAnonymous]
    public class WellKnownFileController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly IApplePayPaymentMethodService _paymentMethodService;

        #endregion

        public WellKnownFileController()
        {
            _paymentMethodService = IoC.Resolve<IApplePayPaymentMethodService>("ApplePayPaymentMethodService");
        }

        /// <summary>
        /// Get app site association
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(".well-known/apple-app-site-association")]
        public ContentResult SiteAssociation()
        {
            var model = new AppleAppSiteAssociationViewModel();
            return new ContentResult
            {
                Content = model.SerializeAsJson(),
                ContentType = "text/text"
            };
        }

        /// <summary>
        /// Get domain association file
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(".well-known/apple-developer-merchantid-domain-association.txt")]
        public IActionResult DomainAssociation()
        {
            var fileRequest = _paymentMethodService.GetAssociationDomainFile();
            if (!fileRequest.IsSuccess) return new ContentResult
            {
                Content = fileRequest.Errors.FirstOrDefault()?.Message,
                ContentType = "text/text"
            };
            return File(fileRequest.Result, "text/text", "apple-developer-merchantid-domain-association.txt");
        }
    }
}