using System.Collections.Generic;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.ViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Payments.Api.Controllers
{
    /// <summary>
    /// This class represent an api for payment service
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [Route(DefaultApiRouteTemplate)]
    [JsonApiExceptionFilter]
    public class PaymentsMethodsApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject payment method service
        /// </summary>
        private readonly IPaymentService _service;

        #endregion

        public PaymentsMethodsApiController(IPaymentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all payment methods
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<PaymentMethodViewModel>>))]
        public async Task<JsonResult> GetPaymentMethods()
            => await JsonAsync(_service.GetAllPaymentMethodsAsync());

        /// <summary>
        /// Activate payment method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Admin]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> ActivatePaymentMethod(string id)
            => await JsonAsync(_service.ActivatePaymentMethodAsync(id));

        /// <summary>
        /// Disable payment method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Admin]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> DisablePaymentMethod(string id)
            => await JsonAsync(_service.DisablePaymentMethodAsync(id));
    }
}