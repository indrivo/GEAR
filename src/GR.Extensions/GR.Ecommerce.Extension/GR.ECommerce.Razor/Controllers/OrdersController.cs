using System;
using System.Threading.Tasks;
using GR.Core;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
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
        public async Task<IActionResult> CreateOrder()
        {
            var createOrderRequest = await _orderProductService.CreateOrderAsync(Guid.Empty);
            if (createOrderRequest.IsSuccess)
            {
                return RedirectToAction("MyOrders");
            }

            return RedirectToAction("MyOrders");
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