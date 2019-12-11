using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Subscriptions.Razor.Controllers
{
    [Authorize]
    public sealed class SubscriptionController : Controller
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


        public SubscriptionController(ISubscriptionService<Subscription> subscriptionService, 
            IOrderProductService<Order> orderProductService, IProductService<Product> productService)
        {
            _subscriptionService = subscriptionService;
            _orderProductService = orderProductService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<JsonResult> GetSubscription(Guid? id)
        {
            return Json(await _subscriptionService.GetSubscriptionByIdAsync(id));
        }

        /// <summary>
        /// Get subscription by user
        /// </summary>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<JsonResult> GetSubscriptions()
        {
            return Json(await _subscriptionService.GetSubscriptionsByUserAsync());
        }

        /// <summary>
        /// Has valid subscriptions
        /// </summary>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<JsonResult> HasValidSubscriptions()
        {
            return Json(await _subscriptionService.HasValidSubscription());
        }


        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<IActionResult> CreateOrderForPlanSubscription([Required]Guid? productId, [Required]Guid? variationId)
        {
            //TODO: Muta la subscriptions si anunta-l pe Girbu sa schimbe la el in ui, plus aici verificari la subscriptions
            //TODO: Sa adaugi mesaje la errors in ResultModel<Guid> de ce nu poate crea o noua subscriere ca sa stie si NIcu ce sa afiseze in pagina

            var product = await _productService.GetProductByIdAsync(productId);
            var lastSubscriptionForUser = await _subscriptionService.GetLastSubscriptionForUserAsync();

            if (lastSubscriptionForUser.IsSuccess && product.IsSuccess)
            {
                if (lastSubscriptionForUser.Result != null && product.Result.Name != lastSubscriptionForUser.Result?.Name && !lastSubscriptionForUser.Result.IsFree)
                {
                    return Json(new ResultModel
                    {
                        IsSuccess = false, 
                        Errors = new List<IErrorModel> { new ErrorModel() {Message = "Select plan not correspond exist plan" } }
                    });
                }
            }

            var createOrderRequest = await _orderProductService.CreateOrderAsync(productId, variationId);
            return Json(createOrderRequest);
        }
    }
}
