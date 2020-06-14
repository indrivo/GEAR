using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Card.Abstractions;
using GR.Card.Abstractions.Helpers;
using GR.Card.Abstractions.Models;
using GR.Core.Attributes.Documentation;
using GR.Core.Attributes.Validation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.Card.Razor.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [Author(Authors.LUPEI_NICOLAE)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    public sealed class CardPayApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly ICardPayPaymentMethodService _paymentMethodManager;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public CardPayApiController(ICardPayPaymentMethodService paymentMethodManager)
        {
            _paymentMethodManager = paymentMethodManager;
        }

        /// <summary>
        /// Pay order with credit card
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> PayOrder([Required] OrderCreditCardPayViewModel model)
            => await JsonAsync(_paymentMethodManager.PayOrderAsync(model));

        /// <summary>
        /// Get credit card type
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(string))]
        public JsonResult GetCardType([Required] string cardNumber)
        {
            var type = CreditCardValidator.GetCardType(cardNumber);
            return Json(type.ToString());
        }
    }
}