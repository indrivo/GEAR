using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Card.Abstractions;
using GR.Card.Abstractions.Enums;
using GR.Card.Abstractions.Helpers;
using GR.Card.Abstractions.Models;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Authorization;
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
        /// Pay order with existent credit card
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> PayOrderWithExistentCard([Required] OrderWithSavedCreditCardPayViewModel model)
            => await JsonAsync(_paymentMethodManager.PayOrderAsyncWithExistentCardAsync(model));

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

        /// <summary>
        /// Get hidden credit cards
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(IEnumerable<HiddenCreditCardPayViewModel>))]
        public async Task<JsonResult> GetUserHiddenCards() => await JsonAsync(_paymentMethodManager.GetHiddenCardsAsync());

        /// <summary>
        /// Add new card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPut]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> AddNewCard([Required] CreditCardPayViewModel card)
            => await JsonAsync(_paymentMethodManager.AddNewCardAsync(card));

        /// <summary>
        /// Remove credit card
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> SetDefaultCard([Required] Guid cardId)
            => await JsonAsync(_paymentMethodManager.SetDefaultCardAsync(cardId));

        /// <summary>
        /// Remove credit card
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveCreditCard([Required] Guid cardId)
            => await JsonAsync(_paymentMethodManager.RemoveCreditCardAsync(cardId));

        /// <summary>
        /// Get card types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult CardTypes()
        {
            var types = Enum.GetNames(typeof(CreditCardType));
            return Json(types);
        }
    }
}