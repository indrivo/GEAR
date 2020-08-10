using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Subscriptions.Razor.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/subscription/[action]")]
    public sealed class SubscriptionApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Subscription service
        /// </summary>
        private readonly ISubscriptionService<Subscription> _subscriptionService;

        /// <summary>
        /// Inject order product service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        /// <summary>
        /// Inject order product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        #endregion

        public SubscriptionApiController(ISubscriptionService<Subscription> subscriptionService,
            IOrderProductService<Order> orderProductService, IProductService<Product> productService)
        {
            _subscriptionService = subscriptionService;
            _orderProductService = orderProductService;
            _productService = productService;
        }

        /// <summary>
        /// Get subscription plans
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [ResponseCache(Duration = 15 * 60 /* 15 minutes */, Location = ResponseCacheLocation.Any, NoStore = false)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<SubscriptionPlanViewModel>>))]
        public async Task<JsonResult> GetSubscriptionPlans() =>
            await JsonAsync(_subscriptionService.GetSubscriptionPlansAsync());

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson)]
        public async Task<JsonResult> GetSubscription(Guid? id)
        => await JsonAsync(_subscriptionService.GetSubscriptionByIdAsync(id));

        /// <summary>
        /// Get last subscription
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<SubscriptionGetViewModel>))]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<JsonResult> GetLastSubscription()
            => await JsonAsync(_subscriptionService.GetLastSubscriptionAsync());

        /// <summary>
        /// Get subscription by user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson)]
        public async Task<JsonResult> GetSubscriptions()
            => await JsonAsync(_subscriptionService.GetSubscriptionsByUserAsync());

        /// <summary>
        /// Has valid subscriptions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson)]
        public async Task<JsonResult> HasValidSubscriptions()
            => Json(await _subscriptionService.HasValidSubscription());

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Guid>))]
        public async Task<IActionResult> CreateOrderForPlanSubscription([Required] Guid? productId, [Required] Guid? variationId)
        {
            var productRequest = await _productService.GetProductByIdAsync(productId);
            var lastSubscriptionForUser = await _subscriptionService.GetLastSubscriptionForUserAsync();

            if (lastSubscriptionForUser.Result != null && !lastSubscriptionForUser.Result.IsFree)
            {
                //TODO: Abort if is current subscription
            }

            var createOrderRequest = await _orderProductService.CreateOrderAsync(productId, variationId);
            return Json(createOrderRequest);
        }

        /// <summary>
        /// Get total resources
        /// </summary>
        /// <returns></returns>
        [Admin, HttpGet]
        [JsonProduces(typeof(ResultModel<SubscriptionsTotalViewModel>))]
        public async Task<JsonResult> GetTotalIncomeResources()
            => await JsonAsync(_subscriptionService.GetTotalIncomeResourcesAsync());

        /// <summary>
        /// Get user subscription info
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Admin, HttpPost]
        [JsonProduces(typeof(DTResult<SubscriptionUserInfoViewModel>))]
        public async Task<JsonResult> GetUsersSubscriptionInfoWithPagination(DTParameters parameters)
            => await JsonAsync(_subscriptionService.GetUsersSubscriptionInfoWithPaginationAsync(parameters));
    }
}
