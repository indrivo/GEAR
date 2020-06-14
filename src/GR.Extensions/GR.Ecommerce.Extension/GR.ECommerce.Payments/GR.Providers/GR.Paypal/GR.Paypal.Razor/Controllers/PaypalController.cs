using System;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Paypal.Abstractions;
using GR.Paypal.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GR.Paypal.Razor.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [Author(Authors.LUPEI_NICOLAE)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    public sealed class PaypalController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaypalPaymentMethodService _paymentMethodManager;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PaypalController(IPaypalPaymentMethodService paymentMethodManager)
        {
            _paymentMethodManager = paymentMethodManager;
        }

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResponsePaypal))]
        public async Task<JsonResult> CreatePayment(Guid? orderId)
        {
            var hostingDomain = Request.HttpContext.GetAppBaseUrl();
            var response = await _paymentMethodManager.CreatePaymentAsync(hostingDomain, orderId);
            return Json(response);
        }

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResponsePaypal))]
        public async Task<JsonResult> ExecutePayment(PaymentExecuteVm model)
        {
            var response = await _paymentMethodManager.ExecutePaymentAsync(model);
            return Json(response);
        }
    }
}