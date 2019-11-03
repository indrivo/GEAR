using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;
using GR.Orders.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Razor.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject order product service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderProductService"></param>
        public OrdersController(IOrderProductService<Order> orderProductService)
        {
            _orderProductService = orderProductService;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MyOrders() => View();

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
            => Json(await _orderProductService.GetMyOrdersWithPaginationWayAsync(param));
    }
}