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
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Helpers;
using GR.Orders.Abstractions.Models;
using GR.Orders.Abstractions.ViewModels.OrderViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.Orders
{
    public class OrderProductService : IOrderProductService<Order>
    {
        #region Injectable

        /// <summary>
        /// Inject product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly ICommerceContext _commerceContext;

        /// <summary>
        /// Inject orders db context
        /// </summary>
        private readonly IOrderDbContext _orderDbContext;

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

        public OrderProductService(ICommerceContext commerceContext, IDataFilter dataFilter, IUserManager<ApplicationUser> userManager, ICartService cartService, IOrderDbContext orderDbContext, IProductService<Product> productService)
        {
            _commerceContext = commerceContext;
            _dataFilter = dataFilter;
            _userManager = userManager;
            _cartService = cartService;
            _orderDbContext = orderDbContext;
            _productService = productService;
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
            var order = await _orderDbContext.Orders
                .Include(x => x.ProductOrders)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductPrices)
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
            var orders = await _orderDbContext.Orders
                .Include(x => x.ProductOrders)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductPrices)
                .Where(x => x.UserId.Equals(userRequest.Result.Id.ToGuid()))
                .ToListAsync();
            response.IsSuccess = true;
            response.Result = orders;
            return response;
        }

        /// <summary>
        /// Get orders count
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<Dictionary<string, int>>> GetOrdersCountForOrderStatesAsync()
        {
            var response = new Dictionary<string, int>();
            var statuses = Enum.GetNames(typeof(OrderState)).ToList();
            foreach (var state in statuses)
            {
                var count = await _orderDbContext.Orders.CountAsync(x => x.OrderState.ToString().Equals(state));
                response.Add(state, count);
            }
            return new ResultModel<Dictionary<string, int>> { IsSuccess = true, Result = response };
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Order>>> GetAllOrdersAsync()
        {
            var response = new ResultModel<IEnumerable<Order>>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<IEnumerable<Order>>();
            var orders = await _orderDbContext.Orders
                .Include(x => x.ProductOrders)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductPrices)
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
            await _orderDbContext.Orders.AddAsync(order);
            var dbRequest = await _orderDbContext.PushAsync();
            if (dbRequest.IsSuccess)
            {
                CommerceEvents.Orders.OrderCreated(new AddOrderEventArgs
                {
                    Id = order.Id,
                    OrderStatus = order.OrderState.ToString()
                });
            }

            _commerceContext.CartItems.RemoveRange(cart.CartItems);
            await _commerceContext.PushAsync();

            return dbRequest.Map(order.Id);
        }

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> CreateOrderAsync(Guid? productId)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return userRequest.Map(Guid.Empty);
            var productRequest = await _productService.GetProductByIdAsync(productId);
            if (!productRequest.IsSuccess) return productRequest.Map(Guid.Empty);
            var product = productRequest.Result;
            var order = OrderMapper.Map(product);
            order.UserId = userRequest.Result.Id.ToGuid();
            await _orderDbContext.Orders.AddAsync(order);
            var dbRequest = await _orderDbContext.PushAsync();
            if (dbRequest.IsSuccess)
            {
                CommerceEvents.Orders.OrderCreated(new AddOrderEventArgs
                {
                    Id = order.Id,
                    OrderStatus = order.OrderState.ToString()
                });
            }

            await _commerceContext.PushAsync();

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
            var response = new ResultModel();
            var orderRequest = await GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return orderRequest.ToBase();
            var order = orderRequest.Result;
            if (order.OrderState == orderState)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Same status"));
                return response;
            }

            var oldState = order.OrderState;
            order.OrderState = orderState;
            _orderDbContext.Orders.Update(order);
            var dbRequest = await _orderDbContext.PushAsync();
            if (dbRequest.IsSuccess)
                await _orderDbContext.OrderHistories.AddAsync(new OrderHistory
                {
                    Notes = string.Empty,
                    OrderState = oldState,
                    OrderId = order.Id
                });
            return dbRequest;
        }

        /// <summary>
        /// Set billing
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shipmentAddress"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetOrderBillingAddressAndShipmentAsync(Guid? orderId, Guid shipmentAddress, Guid billingAddress)
        {
            var orderRequest = await GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return new NotFoundResultModel<object>().ToBase();
            var order = orderRequest.Result;
            order.BillingAddress = billingAddress;
            order.ShipmentAddress = shipmentAddress;
            _orderDbContext.Orders.Update(order);
            return await _orderDbContext.PushAsync();
        }

        /// <summary>
        /// Get order history
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<OrderHistory>>> GetOrderHistoryAsync(Guid? orderId)
        {
            var history = await _orderDbContext.OrderHistories.Where(x => x.OrderId.Equals(orderId)).ToListAsync();
            return new ResultModel<IEnumerable<OrderHistory>> { IsSuccess = true, Result = history };
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
                map.ProductOrders = _orderDbContext.ProductOrders.Where(t => t.OrderId.Equals(map.Id));
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
