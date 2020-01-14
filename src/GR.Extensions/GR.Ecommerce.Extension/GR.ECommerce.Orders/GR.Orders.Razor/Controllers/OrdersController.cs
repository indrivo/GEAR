using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.BaseControllers;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;
using GR.ECommerce.Payments.Abstractions;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Orders.Razor.ViewModels.OrderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Orders.Razor.Controllers
{
    [Authorize]
    public class OrdersController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject order product service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderProductService"></param>
        /// <param name="paymentService"></param>
        public OrdersController(IOrderProductService<Order> orderProductService, IPaymentService paymentService)
        {
            _orderProductService = orderProductService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MyOrders() => View();

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public IActionResult AllOrders() => View();

        /// <summary>
        /// Get order details
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(Guid? orderId)
        {
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return NotFound();

            var paymentsRequest = await _paymentService.GetPaymentsForOrderAsync(orderId);

            return View(new OrderViewModel
            {
                Order = orderRequest.Result,
                Payments = paymentsRequest.Result?.ToList()
            });
        }

        /// <summary>
        /// Create order
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([Required]OrderCartViewModel model)
        {
            var createOrderRequest = await _orderProductService.CreateOrderAsync(model);
            return Json(createOrderRequest);
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
            var createOrderRequest = await _orderProductService.CreateOrderAsync(productId, variationId);
            return Json(createOrderRequest);
        }

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> CancelOrder(Guid? orderId)
        {
            var cancelRequest = await _orderProductService.CancelOrderAsync(orderId);
            return Json(cancelRequest);
        }

        /// <summary>
        /// Get orders with pagination
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(DTResult<Order>))]
        public virtual async Task<JsonResult> GetMyOrdersWithPagination(DTParameters param)
            => Json(await _orderProductService.GetMyOrdersWithPaginationWayAsync(param), SerializerSettings);

        /// <summary>
        /// Get orders with pagination
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]"), Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        [Produces("application/json", Type = typeof(DTResult<Order>))]
        public virtual JsonResult GetAllOrdersWithPagination(DTParameters param)
            => Json(_orderProductService.GetAllOrdersWithPaginationWay(param), SerializerSettings);

        /// <summary>
        /// Get orders count
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Dictionary<string, int>>))]
        public virtual async Task<JsonResult> GetOrdersGraphInfo() =>
            Json(await _orderProductService.GetOrdersCountForOrderStatesAsync(), SerializerSettings);


        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<Order>>))]
        public async Task<JsonResult> GetAllOrders()
        {
            var ordersRequest = await _orderProductService.GetAllOrdersAsync();
            return Json(ordersRequest, SerializerSettings);
        }
    }
}