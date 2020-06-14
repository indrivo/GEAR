using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Helpers;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;
using Microsoft.EntityFrameworkCore;

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
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<GearRole> _notify;

        /// <summary>
        /// Inject product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        /// <summary>
        /// inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        public SubscriptionService(IUserManager<GearUser> userManager, ISubscriptionDbContext subscriptionDbContext, IOrderProductService<Order> orderService, IPaymentService paymentService, INotify<GearRole> notify, IProductService<Product> productService, ICommerceContext commerceContext, IMapper mapper)
        {
            _userManager = userManager;
            _subscriptionDbContext = subscriptionDbContext;
            _orderService = orderService;
            _paymentService = paymentService;
            _notify = notify;
            _productService = productService;
            _commerceContext = commerceContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Get subscription by User
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<SubscriptionGetViewModel>>> GetSubscriptionsByUserAsync()
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
                .Where(x => SubscriptionResources.Configuration.UserSubscriptionQuery(x, user))
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
        public async Task<ResultModel<IEnumerable<SubscriptionGetViewModel>>> GetValidSubscriptionsForUserAsync(Guid? userId)
        {
            var response = new ResultModel<IEnumerable<SubscriptionGetViewModel>>();
            if (userId == null) return new InvalidParametersResultModel<IEnumerable<SubscriptionGetViewModel>>();
            var user = await _userManager.UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return new NotFoundResultModel<IEnumerable<SubscriptionGetViewModel>>();

            var listSubscription = await _subscriptionDbContext.Subscription
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .Include(x => x.SubscriptionPermissions)
                .Where(x => SubscriptionResources.Configuration.UserSubscriptionQuery(x, user) && x.IsValid)
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
        public async Task<ResultModel<Subscription>> GetSubscriptionByIdAsync(Guid? subscriptionId)
        {
            var response = new ResultModel<Subscription>();

            if (subscriptionId is null)
                return new InvalidParametersResultModel<Subscription>();

            var subscription = await _subscriptionDbContext.Subscription
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
        public async Task<ResultModel<Guid>> UpdateSubscriptionAsync(SubscriptionAddViewModel model)
        {
            if (model == null) throw new NullReferenceException();
            var result = new ResultModel<Guid>();
            if (!model.IsFree)
            {
                var orderRequest = await _orderService.GetOrderByIdAsync(model.OrderId);
                if (!orderRequest.IsSuccess) return new InvalidParametersResultModel<Guid>();
                var order = orderRequest.Result;
                var isPayedRequest = await _paymentService.IsOrderPayedAsync(order.Id);
                if (!isPayedRequest.IsSuccess)
                {
                    result.AddError("Order was not paid");
                    return result;
                }
            }

            var existSubscription = await GetSubscriptionByIdAsync(model.Id);

            Guid subscriptionId;

            if (existSubscription.IsSuccess)
            {
                var subscription = existSubscription.Result;
                subscription.OrderId = model.OrderId;
                subscription.Availability = model.Availability;
                subscription.Name = model.Name;
                subscription.SubscriptionPermissions = model.SubscriptionPermissions;
                subscription.IsFree = model.IsFree;

                _subscriptionDbContext.Subscription.Update(subscription);
                subscriptionId = subscription.Id;
            }
            else
            {
                var user = await _userManager.UserManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    result.AddError("User not exist");
                    return result;
                }

                var subscription = new Subscription
                {
                    Id = model.Id,
                    UserId = user.Id,
                    TenantId = user.TenantId,
                    StartDate = model.StartDate,
                    Availability = model.Availability,
                    OrderId = model.OrderId,
                    Name = model.Name,
                    IsFree = model.IsFree,
                    SubscriptionPermissions = model.SubscriptionPermissions
                };
                await _subscriptionDbContext.Subscription.AddAsync(subscription);
                subscriptionId = subscription.Id;
            }

            var dbRequest = await _subscriptionDbContext.PushAsync();

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
            var data = await _subscriptionDbContext.Subscription
                .AsNoTracking()
                .Where(x => !x.IsFree && x.IsValid && TimeSpan.FromDays(x.RemainingDays) < timeSpan).ToListAsync();
            return new SuccessResultModel<IEnumerable<Subscription>>(data);
        }

        /// <summary>
        /// Get expired subscriptions
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Subscription>>> GetExpiredSubscriptionsAsync()
        {
            var data = await _subscriptionDbContext.Subscription
                .Where(x => !x.IsValid).ToListAsync();
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
                await _notify.SendNotificationAsync(new List<Guid> { subscription.UserId }, new Notification
                {
                    Subject = $"Subscription {subscription.Name} expired",
                    SendLocal = true,
                    SendEmail = true,
                    Content = $"Subscription {subscription.Name}, valid from {subscription.StartDate} " +
                              $"to {subscription.ExpirationDate}, has expired"
                });
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
                await _notify.SendNotificationAsync(new List<Guid> { subscription.UserId }, new Notification
                {
                    Subject = "The subscription expires soon",
                    SendLocal = true,
                    SendEmail = true,
                    Content = $"Subscription {subscription.Name}, valid from {subscription.StartDate} " +
                              $"to {subscription.ExpirationDate}, expires in {subscription.RemainingDays} days"
                });
            }
        }

        /// <summary>
        /// Get las subscription for user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<Subscription>> GetLastSubscriptionForUserAsync(Guid? userId = null)
        {
            var user = (await _userManager.GetCurrentUserAsync()).Result;

            if (user == null && userId.HasValue)
                user = await _userManager.UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return new NotFoundResultModel<Subscription>();
            }

            var userSubscription = await _subscriptionDbContext.Subscription
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .Include(x => x.SubscriptionPermissions)
                .Where(x => SubscriptionResources.Configuration.UserSubscriptionQuery(x, user))
                .ToListAsync();

            if (!userSubscription.Any())
            {
                return new ResultModel<Subscription>
                {
                    Errors = new List<IErrorModel> { new ErrorModel { Message = "Subscription not found" } },
                    IsSuccess = false
                };
            }

            return new ResultModel<Subscription>
            {
                IsSuccess = true,
                Result = userSubscription.OrderByDescending(o => o.Created).FirstOrDefault()
            };
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
    }
}