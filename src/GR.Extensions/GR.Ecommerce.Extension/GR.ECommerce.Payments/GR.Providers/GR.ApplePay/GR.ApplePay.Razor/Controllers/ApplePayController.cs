using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using GR.ApplePay.Abstractions;
using GR.ApplePay.Abstractions.Models;
using GR.ApplePay.Abstractions.ViewModels;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.ApplePay.Razor.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [Author(Authors.LUPEI_NICOLAE)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    public sealed class ApplePayController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly IApplePayPaymentMethodService _paymentMethodService;

        #endregion

        public ApplePayController(IApplePayPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        /// <summary>
        /// Get merchant identifier
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson)]
        public JsonResult GetMerchantIdentifier()
            => Json(_paymentMethodService.GetMerchantIdentifier());

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson)]
        public async Task<JsonResult> CreatePayment([Required] Guid orderId)
            => await JsonAsync(_paymentMethodService.CreatePaymentAsync(orderId));

        /// <summary>
        /// Approve transaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson)]
        public async Task<JsonResult> ApproveTransactionRequest(ApproveTransactionRequestViewModel model)
            => await JsonAsync(_paymentMethodService.ApproveCompleteTransactionAsync(model));

        /// <summary>
        /// Validate merchant
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson)]
        public async Task<JsonResult> Validate([FromBody] ValidateMerchantSessionModel model, CancellationToken cancellationToken = default)
        {
            var merchantSession = await _paymentMethodService.GetMerchantSessionAsync(model, HttpContext, cancellationToken);
            return Json(merchantSession);
        }
    }
}