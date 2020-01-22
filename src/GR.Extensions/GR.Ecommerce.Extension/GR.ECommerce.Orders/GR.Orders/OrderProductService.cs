using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.OrderViewModels;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Events;
using GR.Orders.Abstractions.Events.EventArgs.OrderEventArgs;
using GR.Orders.Abstractions.Helpers;
using GR.Orders.Abstractions.Models;
using GR.Orders.Abstractions.ViewModels.OrderViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.Orders
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Documentation("Basic implementation of product ordering service")]
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
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Cart service
        /// </summary>
        private readonly ICartService _cartService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commerceContext"></param>
        /// <param name="dataFilter"></param>
        /// <param name="userManager"></param>
        /// <param name="cartService"></param>
        /// <param name="orderDbContext"></param>
        /// <param name="productService"></param>
        public OrderProductService(ICommerceContext commerceContext, IDataFilter dataFilter, IUserManager<GearUser> userManager, ICartService cartService, IOrderDbContext orderDbContext, IProductService<Product> productService)
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
        public virtual async Task<ResultModel<Order>> GetOrderByIdAsync(Guid? orderId)
        {
            var response = new ResultModel<Order>();
            if (orderId == null) return new InvalidParametersResultModel<Order>();
            var order = await _orderDbContext.Orders
                .Include(x => x.Currency)
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
        public virtual async Task<ResultModel<IEnumerable<Order>>> GetMyOrdersAsync()
        {
            var response = new ResultModel<IEnumerable<Order>>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<IEnumerable<Order>>();
            var orders = await _orderDbContext.Orders
                .Include(x => x.Currency)
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
        public virtual async Task<ResultModel<Dictionary<string, int>>> GetOrdersCountForOrderStatesAsync()
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
        public virtual async Task<ResultModel<IEnumerable<Order>>> GetAllOrdersAsync()
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
        public virtual async Task<ResultModel<Guid>> CreateOrderAsync(OrderCartViewModel model)
        {
            if (model == null) throw new NullReferenceException();
            if (model.CartId == null) return new NotFoundResultModel<Guid>();
            var cartRequest = await _cartService.GetCartByIdAsync(model.CartId);
            if (!cartRequest.IsSuccess) return cartRequest.Map(Guid.Empty);
            var cart = cartRequest.Result;
            var order = OrderMapper.Map(cart, model.Notes);
            var currency = (await _productService.GetGlobalCurrencyAsync()).Result;
            order.CurrencyId = currency.Code;
            await _orderDbContext.Orders.AddAsync(order);
            var dbRequest = await _orderDbContext.PushAsync();
            if (dbRequest.IsSuccess)
            {
                OrderEvents.Orders.OrderCreated(new AddOrderEventArgs
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
        public virtual async Task<ResultModel<Guid>> CreateOrderAsync(Guid? productId)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return userRequest.Map(Guid.Empty);
            var productRequest = await _productService.GetProductByIdAsync(productId);
            if (!productRequest.IsSuccess) return productRequest.Map(Guid.Empty);
            var product = productRequest.Result;
            var order = OrderMapper.Map(product);
            var currency = (await _productService.GetGlobalCurrencyAsync()).Result;
            order.CurrencyId = currency.Code;
            order.UserId = userRequest.Result.Id.ToGuid();
            await _orderDbContext.Orders.AddAsync(order);
            var dbRequest = await _orderDbContext.PushAsync();
            if (dbRequest.IsSuccess)
            {
                OrderEvents.Orders.OrderCreated(new AddOrderEventArgs
                {
                    Id = order.Id,
                    OrderStatus = order.OrderState.ToString()
                });
            }

            await _commerceContext.PushAsync();

            return dbRequest.Map(order.Id);
        }

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> CreateOrderAsync(Guid? productId, Guid? variationId)
        {
            if (productId == null || variationId == null) return new InvalidParametersResultModel<Guid>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return userRequest.Map(Guid.Empty);
            var productRequest = await _productService.GetProductByIdAsync(productId);
            if (!productRequest.IsSuccess) return productRequest.Map(Guid.Empty);
            var product = productRequest.Result;
            var variation = product.ProductVariations.FirstOrDefault(x => x.Id.Equals(variationId));
            var order = OrderMapper.Map(product, variation);
            order.UserId = userRequest.Result.Id.ToGuid();
            var currency = (await _productService.GetGlobalCurrencyAsync()).Result;
            order.CurrencyId = currency.Code;
            await _orderDbContext.Orders.AddAsync(order);
            var dbRequest = await _orderDbContext.PushAsync();
            if (dbRequest.IsSuccess)
            {
                OrderEvents.Orders.OrderCreated(new AddOrderEventArgs
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
        /// Get all orders
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual DTResult<GetOrdersViewModel> GetAllOrdersWithPaginationWay(DTParameters param)
        {
            if (param == null) return new DTResult<GetOrdersViewModel>();
            var filtered = _dataFilter.FilterAbstractEntity<Order, ICommerceContext>(_commerceContext, param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount).ToList();

            var list = filtered.Select(async x =>
            {
                var map = x.Adapt<GetOrdersViewModel>();
                map.ProductOrders =
                    await _orderDbContext.ProductOrders.Where(t => t.OrderId.Equals(map.Id)).ToListAsync();
                map.User = _userManager.UserManager.Users.FirstOrDefault(y => y.Id.ToGuid().Equals(x.UserId));
                return map;
            }).Select(x => x.Result);

            return new DTResult<GetOrdersViewModel>
            {
                Draw = param.Draw,
                Data = list.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
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
        /// <param name="notes"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeOrderStateAsync(Guid? orderId, OrderState orderState, string notes = null)
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

            //Save history
            await _orderDbContext.OrderHistories.AddAsync(new OrderHistory
            {
                Notes = notes,
                OrderState = oldState,
                OrderId = order.Id
            });

            var dbRequest = await _orderDbContext.PushAsync();
            if (!dbRequest.IsSuccess) return dbRequest;
            switch (order.OrderState)
            {
                case OrderState.New:
                    break;
                case OrderState.OnHold:
                    break;
                case OrderState.PendingPayment:
                    break;
                case OrderState.PaymentReceived:
                    OrderEvents.Orders.PaymentReceived(new PaymentReceivedEventArgs
                    {
                        OrderId = order.Id,
                        UserId = order.UserId
                    });
                    break;
                case OrderState.PaymentFailed:
                    break;
                case OrderState.Invoiced:
                    break;
                case OrderState.Shipping:
                    break;
                case OrderState.Shipped:
                    break;
                case OrderState.Complete:
                    break;
                case OrderState.Canceled:
                    break;
                case OrderState.Refunded:
                    break;
                case OrderState.Closed:
                    break;
            }
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
            if (shipmentAddress == Guid.Empty || billingAddress == Guid.Empty) return new InvalidParametersResultModel("The address was not specified");
            var orderRequest = await GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return new NotFoundResultModel<object>().ToBase();
            var order = orderRequest.Result;
            order.BillingAddress = billingAddress;
            order.ShipmentAddress = shipmentAddress;
            _orderDbContext.Orders.Update(order);
            var dbResult = await _orderDbContext.PushAsync();
            if (dbResult.IsSuccess) await ChangeOrderStateAsync(orderId, OrderState.Invoiced);
            return dbResult;
        }

        /// <summary>
        /// Check if the order was in the x state
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderState"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<bool>> ItWasInTheStateAsync(Guid? orderId, OrderState orderState)
        {
            if (orderId == null) return new InvalidParametersResultModel<bool>();
            var check = await _orderDbContext.OrderHistories
                .Include(x => x.Order)
                .AnyAsync(x => x.OrderId.Equals(orderId)
                               && (x.OrderState.Equals(orderState) || x.Order.OrderState.Equals(orderState)));
            return new SuccessResultModel<bool>(check);
        }

        /// <summary>
        /// Get order history
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<OrderHistory>>> GetOrderHistoryAsync(Guid? orderId)
        {
            var history = await _orderDbContext.OrderHistories.Where(x => x.OrderId.Equals(orderId)).ToListAsync();
            return new SuccessResultModel<IEnumerable<OrderHistory>>(history);
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
                x.Currency = _commerceContext.Currencies.FirstOrDefault(y => y.Code.Equals(x.CurrencyId));
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
