using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;
using GR.Orders.Abstractions.Models;
using GR.Orders.Abstractions.ViewModels.OrderViewModels;

namespace GR.Orders.Abstractions
{
    public interface IOrderProductService<TOrderEntity> where TOrderEntity : Order
    {
        /// <summary>
        /// Get my orders
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<TOrderEntity>>> GetMyOrdersAsync();

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<Order>> GetOrderByIdAsync(Guid? orderId);

        /// <summary>
        /// Create order from cart
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateOrderAsync(OrderCartViewModel model);

        /// <summary>
        /// Get paginated orders by user id
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        DTResult<GetOrdersViewModel> GetPaginatedOrdersByUserId(DTParameters param, Guid? userId);

        /// <summary>
        /// Get my orders
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<DTResult<GetOrdersViewModel>> GetMyOrdersWithPaginationWayAsync(DTParameters param);

        /// <summary>
        /// Change order state
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderState"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeOrderStateAsync(Guid? orderId, OrderState orderState);

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel> CancelOrderAsync(Guid? orderId);

        /// <summary>
        /// Set up addresses
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shipmentAddress"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        Task<ResultModel> SetOrderBillingAddressAndShipmentAsync(Guid? orderId, Guid shipmentAddress, Guid billingAddress);

        /// <summary>
        /// Get order history
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<OrderHistory>>> GetOrderHistoryAsync(Guid? orderId);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Order>>> GetAllOrdersAsync();

        /// <summary>
        /// Get orders count
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<Dictionary<string, int>>> GetOrdersCountForOrderStatesAsync();

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateOrderAsync(Guid? productId);
    }
}