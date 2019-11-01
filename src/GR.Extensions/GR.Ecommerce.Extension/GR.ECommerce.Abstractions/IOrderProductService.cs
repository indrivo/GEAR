﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;

namespace GR.ECommerce.Abstractions
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
    }
}