using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.GooglePay.Abstractions;
using GR.GooglePay.Abstractions.Models;
using GR.GooglePay.Abstractions.ViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.GooglePay.Razor.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [Author(Authors.LUPEI_NICOLAE)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    public sealed class GPayController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IGPayPaymentMethodService _paymentMethodManager;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GPayController(IGPayPaymentMethodService paymentMethodManager)
        {
            _paymentMethodManager = paymentMethodManager;
        }

        /// <summary>
        /// Create payment with GPay
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<GPayTransactionInfo>))]
        public async Task<JsonResult> CreatePayment([Required] Guid orderId)
            => await JsonAsync(_paymentMethodManager.CreatePaymentAsync(orderId));

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<GPayTransactionInfo>))]
        public async Task<JsonResult> ExecutePayment([Required] GPayPaymentExecuteViewModel model)
            => await JsonAsync(_paymentMethodManager.ExecutePaymentAsync(model));
    }
}