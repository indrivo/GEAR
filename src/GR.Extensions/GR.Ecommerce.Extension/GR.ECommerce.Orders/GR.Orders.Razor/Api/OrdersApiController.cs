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
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;

namespace GR.Orders.Razor.Api
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/orders/[action]")]
    public sealed class OrdersApiController : BaseGearController
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
        public OrdersApiController(IOrderProductService<Order> orderProductService)
        {
            _orderProductService = orderProductService;
        }

        /// <summary>
        /// Create order
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel<Guid>))]
        public async Task<JsonResult> CreateOrder([Required] OrderCartViewModel model)
        {
            var createOrderRequest = await _orderProductService.CreateOrderAsync(model);
            return Json(createOrderRequest);
        }

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
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
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(DTResult<Order>))]
        public async Task<JsonResult> GetMyOrdersWithPagination(DTParameters param)
            => await JsonAsync(_orderProductService.GetMyOrdersWithPaginationWayAsync(param), DateFormatWithTimeSerializerSettings);

        /// <summary>
        /// Get orders with pagination
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Admin]
        [Produces(ContentType.ApplicationJson, Type = typeof(DTResult<Order>))]
        public async Task<JsonResult> GetAllOrdersWithPagination(DTParameters param)
            => await JsonAsync(_orderProductService.GetAllOrdersWithPaginationWayAsync(param), DateFormatWithTimeSerializerSettings);

        /// <summary>
        /// Get orders count
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Dictionary<string, int>>))]
        public async Task<JsonResult> GetOrdersGraphInfo() =>
            Json(await _orderProductService.GetOrdersCountForOrderStatesAsync(), SerializerSettings);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<Order>>))]
        public async Task<JsonResult> GetAllOrders()
        {
            var ordersRequest = await _orderProductService.GetAllOrdersAsync();
            return Json(ordersRequest, SerializerSettings);
        }
    }
}