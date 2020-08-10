using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Helpers;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GR.Subscriptions
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class SubscriptionService : ISubscriptionService<Subscription>
    {
        #region Injectable
        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject subscription db context
        /// </summary>
        private readonly ISubscriptionDbContext _subscriptionDbContext;

        /// <summary>
        /// Inject commerce context
        /// </summary>
        private readonly ICommerceContext _commerceContext;

        /// <summary>
        /// Inject order service
        /// </summary>
        private readonly IOrderProductService<Order> _orderService;

        /// <summary>
        /// Inject orders context
        /// </summary>
        private readonly IOrderDbContext _orderDbContext;

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly IAppSender _sender;

        /// <summary>
        /// Inject product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        /// <summary>
        /// inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<SubscriptionService> _logger;

        private readonly SubscriptionConfiguration _subscriptionConfiguration;

        #endregion

        public SubscriptionService(IUserManager<GearUser> userManager, ISubscriptionDbContext subscriptionDbContext, IOrderProductService<Order> orderService, IPaymentService paymentService, IAppSender sender, IProductService<Product> productService, ICommerceContext commerceContext, IMapper mapper, ILogger<SubscriptionService> logger, IOrderDbContext orderDbContext, SubscriptionConfiguration subscriptionConfiguration)
        {
            _userManager = userManager;
            _subscriptionDbContext = subscriptionDbContext;
            _orderService = orderService;
            _paymentService = paymentService;
            _sender = sender;
            _productService = productService;
            _commerceContext = commerceContext;
            _mapper = mapper;
            _logger = logger;
            _orderDbContext = orderDbContext;
            _subscriptionConfiguration = subscriptionConfiguration;
        }

        /// <summary>
        /// Get subscription by User
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<SubscriptionGetViewModel>>> GetSubscriptionsByUserAsync()
        {
            var response = new ResultModel<IEnumerable<SubscriptionGetViewModel>>();
            var user = (await _userManager.GetCurrentUserAsync()).Result;

            if (user == null)
            {
                return new NotFoundResultModel<IEnumerable<SubscriptionGetViewModel>>();
            }

            var listSubscription = await _subscriptionDbContext.Subscription
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .Include(x => x.SubscriptionPermissions)
                .Where(x => x.UserId.Equals(user.Id))
                .ToListAsync();

            response.IsSuccess = true;
            response.Result = _mapper.Map<IEnumerable<SubscriptionGetViewModel>>(listSubscription);
            return response;
        }

        /// <summary>
        /// Get valid subscriptions for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<SubscriptionGetViewModel>>> GetValidSubscriptionsForUserAsync(Guid? userId)
        {
            var response = new ResultModel<IEnumerable<SubscriptionGetViewModel>>();
            if (userId == null) return new InvalidParametersResultModel<IEnumerable<SubscriptionGetViewModel>>();
            var user = await _userManager.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return new NotFoundResultModel<IEnumerable<SubscriptionGetViewModel>>();

            var listSubscription = await _subscriptionDbContext.Subscription
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .Include(x => x.SubscriptionPermissions)
                .Where(x => x.UserId.Equals(user.Id) && (x.StartDate.AddDays(x.Availability) > DateTime.Now || x.IsFree))
                .ToListAsync();

            response.IsSuccess = true;
            response.Result = _mapper.Map<IEnumerable<SubscriptionGetViewModel>>(listSubscription);

            return response;
        }

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Subscription>> GetSubscriptionByIdAsync(Guid? subscriptionId)
        {
            var response = new ResultModel<Subscription>();

            if (subscriptionId is null)
                return new InvalidParametersResultModel<Subscription>();

            var subscription = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .Include(x => x.SubscriptionPermissions)
                .FirstOrDefaultAsync(x => x.Id == subscriptionId);

            if (subscription is null) return new NotFoundResultModel<Subscription>();

            response.IsSuccess = true;
            response.Result = subscription;
            return response;
        }

        /// <summary>
        /// Create subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddOrUpdateSubscriptionAsync(SubscriptionAddViewModel model)
        {
            if (model == null) throw new NullReferenceException();
            _logger.LogTrace($"User {model.UserId} start to update subscription, subscription: {model.Id}");
            var state = ModelValidator.IsValid<SubscriptionAddViewModel, Guid>(model);
            if (!state.IsSuccess)
            {
                _logger.LogCritical($"User {model.UserId} fail to update subscription, reason: validation error, model: {model.SerializeAsJson()}");
                return state;
            }
            var result = new ResultModel<Guid>();
            if (!model.IsFree)
            {
                var orderRequest = await _orderService.GetOrderByIdAsync(model.OrderId);
                if (!orderRequest.IsSuccess) return new InvalidParametersResultModel<Guid>();
                var order = orderRequest.Result;
                var isPayedRequest = await _paymentService.IsOrderPayedAsync(order.Id);
                if (!isPayedRequest.IsSuccess)
                {
                    _logger.LogTrace($"User {model.UserId} fail to update subscription, reason: order not payed, orderId: {model.OrderId}");
                    result.AddError("Order was not paid");
                    return result;
                }
            }

            var subscription = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .Include(x => x.SubscriptionPermissions)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            Guid subscriptionId;

            if (subscription != null)
            {
                _subscriptionDbContext.SubscriptionPermissions.RemoveRange(subscription.SubscriptionPermissions);
                var removeCurrentPermissions = await _subscriptionDbContext.PushAsync();
                if (!removeCurrentPermissions.IsSuccess)
                {
                    _logger.LogError("Fail to remove existent permissions for subscription: {Subscription}", subscription.Id);
                }
                subscription.OrderId = model.OrderId;
                subscription.Availability = model.Availability;
                subscription.Name = model.Name;
                subscription.SubscriptionPermissions = null;
                subscription.IsFree = model.IsFree;
                _subscriptionDbContext.Subscription.Update(subscription);
                var newPermissions = model.SubscriptionPermissions.ToList();
                foreach (var permission in newPermissions)
                {
                    permission.SubscriptionId = subscription.Id;
                }
                await _subscriptionDbContext.SubscriptionPermissions.AddRangeAsync(newPermissions);
                subscriptionId = subscription.Id;
            }
            else
            {
                var user = await _userManager.UserManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    _logger.LogCritical($"Subscription {model.Name} with id: {model.Id} fail to update, error: user not found, userId: {model.UserId}");
                    result.AddError("User not exist");
                    return result;
                }

                subscription = model;
                await _subscriptionDbContext.Subscription.AddAsync(subscription);
                subscriptionId = subscription.Id;
            }

            var dbRequest = await _subscriptionDbContext.PushAsync();
            if (!dbRequest.IsSuccess)
            {
                _logger.LogCritical($"Subscription {model.Name} with id: {model.Id} fail to update, error: {dbRequest.Errors.FirstOrDefault()?.Message}");
            }

            return dbRequest.Map(subscriptionId);
        }

        /// <summary>
        /// Has valid subscriptions
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<bool>> HasValidSubscription()
        {
            var toReturn = new ResultModel<bool>();
            var user = (await _userManager.GetCurrentUserAsync()).Result;

            if (user is null) return new NotFoundResultModel<bool>();

            var listSubscription = (await GetSubscriptionsByUserAsync()).Result;

            toReturn.IsSuccess = true;
            toReturn.Result = listSubscription.Any(x => x.IsValid);

            return toReturn;
        }

        /// <summary>
        /// Get subscriptions that will expire after some period
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Subscription>>> GetSubscriptionsThatExpireInAsync(TimeSpan timeSpan)
        {
            var days = timeSpan.TotalDays;
            var data = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Where(x => !x.IsFree && (x.StartDate.AddDays(x.Availability) > DateTime.Now || x.IsFree))
                .ToListAsync();
            //TODO: TimeSpan issue on EF Core 3.1
            return new SuccessResultModel<IEnumerable<Subscription>>(data.Where(x => (x.StartDate.AddDays(x.Availability) - DateTime.Now).Days < days).ToList());
        }

        /// <summary>
        /// Get expired subscriptions
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Subscription>>> GetExpiredSubscriptionsAsync()
        {
            var data = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Where(x => !(x.StartDate.AddDays(x.Availability) > DateTime.Now || x.IsFree)).ToListAsync();
            return new SuccessResultModel<IEnumerable<Subscription>>(data);
        }

        /// <summary>
        /// Get subscription duration
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        public virtual int GetSubscriptionDuration([Required] ProductVariation variation)
        {
            if (variation == null) return 0;
            var periodString = variation.ProductVariationDetails.FirstOrDefault(x => x.ProductOption.Name.Equals("Period"))?.Value;
            var measureUnit = variation.ProductVariationDetails.FirstOrDefault(x => x.ProductOption.Name.Equals("Unit"))?.Value;
            var days = 30;
            var period = Convert.ToInt32(periodString);
            var today = DateTime.Today;
            switch (measureUnit)
            {
                case "day": days = period; break;
                case "month": days = (today.AddMonths(period) - today).Days; break;
                case "year": days = (today.AddYears(period) - today).Days; break;
            }

            return days;
        }

        /// <summary>
        /// Remove range
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveRangeAsync(IEnumerable<Subscription> subscriptions)
        {
            _subscriptionDbContext.Subscription.RemoveRange(subscriptions);
            return await _subscriptionDbContext.PushAsync();
        }

        /// <summary>
        /// Notify expired subscriptions
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> NotifyAndRemoveExpiredSubscriptionsAsync([Required] IList<Subscription> subscriptions)
        {
            if (subscriptions == null) return new NullReferenceResultModel<object>().ToBase();
            var removeRequest = await RemoveRangeAsync(subscriptions);
            if (!removeRequest.IsSuccess) return removeRequest;
            foreach (var subscription in subscriptions)
            {
                var userReq = await _userManager.FindUserByIdAsync(subscription.UserId);
                if (!userReq.IsSuccess) continue;
                var message = $"Subscription {subscription.Name}, valid from {subscription.StartDate} " +
                              $"to {subscription.ExpirationDate}, has expired";
                await _sender.SendAsync(userReq.Result, $"Subscription {subscription.Name} expired", message, _subscriptionConfiguration.NotificationProviders.ToArray());
            }

            return removeRequest;
        }

        /// <summary>
        /// Notify subscriptions that will expire
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        public virtual async Task NotifySubscriptionsThatExpireAsync([Required] IList<Subscription> subscriptions)
        {
            if (subscriptions == null) throw new NullReferenceException();
            foreach (var subscription in subscriptions)
            {
                var userReq = await _userManager.FindUserByIdAsync(subscription.UserId);
                if (!userReq.IsSuccess) continue;
                var message = $"Subscription {subscription.Name}, valid from {subscription.StartDate} " +
                              $"to {subscription.ExpirationDate}, expires in {subscription.RemainingDays} days";
                await _sender.SendAsync(userReq.Result, "The subscription expires soon", message, _subscriptionConfiguration.NotificationProviders.ToArray());
            }
        }

        /// <summary>
        /// Get las subscription for user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<Subscription>> GetLastSubscriptionForUserAsync(Guid? userId = null)
        {
            var result = new ResultModel<Subscription>();
            var user = userId != null
                ? (await _userManager.FindUserByIdAsync(userId)).Result
                : (await _userManager.GetCurrentUserAsync()).Result;

            if (user == null) return UserNotFoundResult<Subscription>.Instance;

            var userSubscription = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .Include(x => x.SubscriptionPermissions)
                .Where(x => x.UserId.Equals(user.Id))
                .ToListAsync();

            if (!userSubscription.Any())
            {
                result.AddError("Subscription not found");

                if (_subscriptionConfiguration.MissingSubscriptionCreateFreeDefault)
                {
                    var response = await AddDefaultFreeSubscriptionAsync(user.Id);
                    if (response.IsSuccess) return await GetLastSubscriptionForUserAsync(user.Id);
                }

                return result;
            }

            result.IsSuccess = true;
            var orderedSubscriptions = userSubscription.OrderByDescending(o => o.Created).ToList();
            result.Result = orderedSubscriptions.FirstOrDefault(x => !x.IsFree && x.IsValid)
                            ?? orderedSubscriptions.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Get last subscription
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<SubscriptionGetViewModel>> GetLastSubscriptionAsync()
        {
            var lastSubscription = await GetLastSubscriptionForUserAsync();
            if (!lastSubscription.IsSuccess || lastSubscription.Result == null)
                return lastSubscription.Map<SubscriptionGetViewModel>();
            var subscription = _mapper.Map<SubscriptionGetViewModel>(lastSubscription.Result);
            return new SuccessResultModel<SubscriptionGetViewModel>(subscription);
        }

        /// <summary>
        /// Get subscription plans
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<SubscriptionPlanViewModel>>> GetSubscriptionPlansAsync()
        {
            var userRequest = _userManager.FindUserIdInClaims();
            var subscriptionsRequest = await GetLastSubscriptionForUserAsync(userRequest.Result);
            var products = await _commerceContext.Products
                .NonDeleted()
                .Include(x => x.ProductAttributes)
                .ThenInclude(x => x.ProductAttribute)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductVariations)
                .ThenInclude(x => x.ProductVariationDetails)
                .ThenInclude(x => x.ProductOption)
                .Where(x => x.ProductTypeId.Equals(SubscriptionResources.SubscriptionPlanProductType)
                            && x.IsPublished)
                .ToListAsync();

            var currency = (await _productService.GetGlobalCurrencyAsync()).Result;
            var response = new ResultModel<IEnumerable<SubscriptionPlanViewModel>>
            {
                IsSuccess = true,
                Result = products.Select(product => SubscriptionMapper.Map(product, currency, subscriptionsRequest.Result))
            };

            return response;
        }

        /// <summary>
        /// Get users in subscription by name. Subscription must be valid
        /// </summary>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<GearUser>>> GetUsersInSubscriptionAsync(string subscriptionName)
        {
            var userIds = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Where(x => (x.StartDate.AddDays(x.Availability) > DateTime.Now || x.IsFree) && x.Name.Equals(subscriptionName)).Select(x => x.UserId).ToListAsync();

            var users = await _userManager.UserManager.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();
            return new SuccessResultModel<IEnumerable<GearUser>>(users);
        }

        /// <summary>
        /// Get users in subscription by name. Subscription must be valid
        /// </summary>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Guid>>> GetUsersIdInSubscriptionAsync(string subscriptionName)
        {
            var userIds = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Where(x => (x.StartDate.AddDays(x.Availability) > DateTime.Now || x.IsFree) && x.Name.Equals(subscriptionName)).Select(x => x.UserId).ToListAsync();
            return new SuccessResultModel<IEnumerable<Guid>>(userIds);
        }

        /// <summary>
        /// Get total incoming
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<SubscriptionsTotalViewModel>> GetTotalIncomeResourcesAsync()
        {
            var productType = SubscriptionResources.SubscriptionPlanProductType;
            var productOrders = await _orderDbContext.ProductOrders
                .Include(x => x.Product)
                .ThenInclude(x => x.ProductPrices)
                .Include(x => x.Product)
                .ThenInclude(x => x.ProductDiscount)
                .Include(x => x.Product)
                .ThenInclude(x => x.ProductVariations)
                .AsNoTracking()
                .Where(x => x.Product.ProductTypeId.Equals(productType) &&
                            x.Order.OrderState == OrderState.PaymentReceived)
                .Select(x => x.FinalPrice)
                .ToListAsync();

            var total = productOrders.Sum();

            var data = new SubscriptionsTotalViewModel
            {
                Total = total,
                Currency = (await _productService.GetGlobalCurrencyAsync()).Result
            };

            return new SuccessResultModel<SubscriptionsTotalViewModel>(data);
        }

        /// <summary>
        /// Get user subscription info with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<SubscriptionUserInfoViewModel>> GetUsersSubscriptionInfoWithPaginationAsync(DTParameters parameters)
        {
            var subscriptionsPaged = await _subscriptionDbContext.Subscription
                .Include(x => x.Order)
                .ThenInclude(x => x.ProductOrders)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductVariations)
                .ThenInclude(x => x.ProductVariationDetails)
                .ThenInclude(x => x.ProductOption)
                .AsNoTracking()
                .GetPagedAsDtResultAsync(parameters);
            var currency = (await _productService.GetGlobalCurrencyAsync()).Result;
            var data = new List<SubscriptionUserInfoViewModel>();
            foreach (var subscription in subscriptionsPaged.Data)
            {
                var userRequest = await _userManager.FindUserByIdAsync(subscription.UserId);
                if (!userRequest.IsSuccess) continue;

                var subscriptionUser = _mapper.Map<SubscriptionUserInfoViewModel>(userRequest.Result);
                subscriptionUser.Currency = currency;
                subscriptionUser.CanExpire = !subscription.IsFree;
                subscriptionUser.ExpirationDate = subscription.ExpirationDate;
                subscriptionUser.DatePaid = subscription.Order?.Changed;
                subscriptionUser.Amount = subscription.Order?.Total ?? 0;
                subscriptionUser.Status = subscription.IsValid ? "Active" : "Expired";
                var orderDetails = subscription.Order?.ProductOrders.FirstOrDefault();
                if (orderDetails?.ProductVariation?.ProductVariationDetails != null)
                {
                    foreach (var variation in orderDetails.ProductVariation.ProductVariationDetails)
                    {
                        subscriptionUser.Period += variation.Value + " ";
                    }
                }
                data.Add(subscriptionUser);
            }

            return new DTResult<SubscriptionUserInfoViewModel>
            {
                Data = data,
                Draw = subscriptionsPaged.Draw,
                RecordsFiltered = subscriptionsPaged.RecordsFiltered,
                RecordsTotal = subscriptionsPaged.RecordsTotal
            };
        }

        /// <summary>
        /// Extend user subscription
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ExtendUserSubscriptionAsync(Guid orderId, Guid userId)
        {
            var orderRequest = await _orderService.GetOrderByIdAsync(orderId);
            if (orderRequest.IsSuccess.Negate()) return orderRequest.ToBase();
            var order = orderRequest.Result;
            var checkIfProductIsSubscription = order.ProductOrders
                .FirstOrDefault(x => x.Product?.ProductTypeId == SubscriptionResources.SubscriptionPlanProductType)?.ProductId;
            if (checkIfProductIsSubscription == null) return new NotFoundResultModel();
            var planRequest = await _productService.GetProductByIdAsync(checkIfProductIsSubscription);
            if (planRequest.IsSuccess.Negate()) return planRequest.ToBase();
            var plan = planRequest.Result;
            var permissions = SubscriptionMapper.Map(plan.ProductAttributes).ToList();
            var variationId = order.ProductOrders.FirstOrDefault(x => x.ProductId == checkIfProductIsSubscription)?.ProductVariationId;
            var variation = plan.ProductVariations.FirstOrDefault(x => x.Id.Equals(variationId));

            var userSubscriptionRequest = await GetLastSubscriptionForUserAsync(userId);

            var newSubscription = new SubscriptionAddViewModel
            {
                Name = plan.Name,
                OrderId = orderId,
                StartDate = DateTime.Now,
                Availability = GetSubscriptionDuration(variation),
                UserId = order.UserId,
                SubscriptionPermissions = permissions,
            };

            if (!userSubscriptionRequest.IsSuccess || userSubscriptionRequest.Result.IsFree)
                return (await AddOrUpdateSubscriptionAsync(newSubscription)).ToBase();

            var userSubscription = userSubscriptionRequest.Result;
            newSubscription.Id = userSubscription.Id;
            newSubscription.Availability += userSubscription.Availability;
            newSubscription.IsFree = false;

            return (await AddOrUpdateSubscriptionAsync(newSubscription)).ToBase();
        }

        /// <summary>
        ///     Replace subscription
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="period"></param>
        /// <param name="unit"></param>
        /// <param name="multiplyIndex"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddSubscriptionAsync(Guid userId, Guid productId, string period,
            string unit, int multiplyIndex = 1)
        {
            var planRequest = await _productService.GetProductByIdAsync(productId);
            if (planRequest.IsSuccess.Negate()) return planRequest.ToBase();
            var plan = planRequest.Result;
            var permissions = SubscriptionMapper.Map(plan.ProductAttributes).ToList();
            var variation = plan.ProductVariations.FirstOrDefault(
                x => x.ProductVariationDetails.Any(c => c.ProductOption.Name.Equals("Unit") && c.Value == unit)
                && x.ProductVariationDetails.Any(c => c.ProductOption.Name.Equals("Period") && c.Value == period));

            var result = new ResultModel();
            if (variation == null)
            {
                _logger.LogError($"Variation not found for productId: {productId}, period: {period}, unit: {unit}");
                result.AddError("Variation not found");
                return result;
            }

            var subscription = new Subscription
            {
                Name = plan.Name,
                StartDate = DateTime.Now,
                Availability = GetSubscriptionDuration(variation) * multiplyIndex,
                UserId = userId,
                SubscriptionPermissions = permissions,
                IsFree = false
            };
            await _subscriptionDbContext.Subscription.AddAsync(subscription);
            return await _subscriptionDbContext.PushAsync();
        }

        /// <summary>
        /// Add default free subscription
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddDefaultFreeSubscriptionAsync(Guid userId)
        {
            var planRequest = await _productService.GetProductByIdAsync(SubscriptionResources.DefaultSubscriptionPlan);

            if (!planRequest.IsSuccess) return planRequest.ToBase();
            var plan = planRequest.Result;
            var permissions = SubscriptionMapper.Map(plan.ProductAttributes).ToList();
            return (await AddOrUpdateSubscriptionAsync(new SubscriptionAddViewModel
            {
                Name = plan.DisplayName,
                StartDate = DateTime.Now,
                Availability = 0,
                UserId = userId,
                IsFree = true,
                SubscriptionPermissions = permissions
            })).ToBase();
        }
    }
}