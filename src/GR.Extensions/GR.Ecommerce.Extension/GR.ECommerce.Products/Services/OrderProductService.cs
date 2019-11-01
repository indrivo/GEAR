using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Events;
using GR.ECommerce.Abstractions.Events.EventArgs.OrderEventArgs;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Responses;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Products.Services
{
    public class OrderProductService : IOrderProductService<Order>
    {
        #region Injectable

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly ICommerceContext _commerceContext;

        /// <summary>
        /// Inject data filter
        /// </summary>
        private readonly IDataFilter _dataFilter;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Cart service
        /// </summary>
        private readonly ICartService _cartService;

        #endregion

        public OrderProductService(ICommerceContext commerceContext, IDataFilter dataFilter, IUserManager<ApplicationUser> userManager, ICartService cartService)
        {
            _commerceContext = commerceContext;
            _dataFilter = dataFilter;
            _userManager = userManager;
            _cartService = cartService;
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Order>> GetOrderByIdAsync(Guid? orderId)
        {
            var response = new ResultModel<Order>();
            if (orderId == null) return new InvalidParametersResultModel<Order>();
            var order = await _commerceContext.Orders
                .Include(x => x.ProductOrders)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id.Equals(orderId));
            if (order == null) return new NotFoundResultModel<Order>();
            response.IsSuccess = true;
            response.Result = order;
            return response;
        }

        /// <summary>
        /// Get my orders
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Order>>> GetMyOrdersAsync()
        {
            var response = new ResultModel<IEnumerable<Order>>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<IEnumerable<Order>>();
            var orders = await _commerceContext.Orders
                .Include(x => x.ProductOrders)
                .ToListAsync();
            response.IsSuccess = true;
            response.Result = orders;
            return response;
        }

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> CreateOrderAsync(OrderCartViewModel model)
        {
            if (model == null) throw new NullReferenceException();
            if (model.CartId == null) return new NotFoundResultModel<Guid>();
            var cartRequest = await _cartService.GetCartByIdAsync(model.CartId);
            if (!cartRequest.IsSuccess) return cartRequest.Map(Guid.Empty);
            var cart = cartRequest.Result;
            var order = OrderMapper.Map(cart, model.Notes);
            await _commerceContext.Orders.AddAsync(order);
            _commerceContext.Carts.Remove(cart);
            var dbRequest = await _commerceContext.PushAsync();
            if (dbRequest.IsSuccess)
            {
                CommerceEvents.Orders.OrderCreated(new AddOrderEventArgs
                {
                    Id = order.Id,
                    OrderStatus = order.OrderState.ToString()
                });
            }

            return dbRequest.Map(order.Id);
        }

        /// <summary>
        /// Get my orders
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<GetOrdersViewModel>> GetMyOrdersWithPaginationWayAsync(DTParameters param)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            return !userRequest.IsSuccess ? new DTResult<GetOrdersViewModel>() : GetPaginatedOrdersByUserId(param, userRequest.Result.Id.ToGuid());
        }

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> CancelOrderAsync(Guid? orderId)
        {
            var response = new ResultModel();
            var orderRequest = await GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return orderRequest.ToBase();
            var order = orderRequest.Result;
            if (order.OrderState == OrderState.New) return await ChangeOrderStateAsync(orderId, OrderState.Canceled);
            response.Errors.Add(new ErrorModel(string.Empty, "The order can only be canceled in the new order status"));
            return response;

        }

        /// <summary>
        /// Change order state
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderState"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeOrderStateAsync(Guid? orderId, OrderState orderState)
        {
            var orderRequest = await GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return orderRequest.ToBase();
            var order = orderRequest.Result;
            order.OrderState = orderState;
            _commerceContext.Update(order);
            return await _commerceContext.PushAsync();
        }

        /// <summary>
        /// Get filtered list of organization
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual DTResult<GetOrdersViewModel> GetPaginatedOrdersByUserId(DTParameters param, Guid? userId)
        {
            if (param == null || userId == null) return new DTResult<GetOrdersViewModel>();
            var filtered = _dataFilter.FilterAbstractEntity<Order, ICommerceContext>(_commerceContext, param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount, x => x.UserId.Equals(userId)).ToList();

            var list = filtered.Select(x =>
            {
                var map = x.Adapt<GetOrdersViewModel>();
                return map;
            });

            return new DTResult<GetOrdersViewModel>
            {
                Draw = param.Draw,
                Data = list.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
        }
    }
}
