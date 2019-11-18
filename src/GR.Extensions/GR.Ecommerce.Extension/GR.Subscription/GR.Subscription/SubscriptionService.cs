using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions;
using GR.Identity.Abstractions;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.Subscriptions
{
    public class SubscriptionService : ISubscriptionService<Subscription>
    {
        #region Injectable
        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly ICommerceContext _commerceContext;


        /// <summary>
        /// Inject subscription db context
        /// </summary>
        private readonly ISubscriptionDbContext _subscriptionDbContext;

        /// <summary>
        /// Inject order service
        /// </summary>
        private readonly IOrderProductService<Order> _orderService;

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        #endregion

        public SubscriptionService(ICommerceContext commerceContext, IUserManager<ApplicationUser> userManager, ISubscriptionDbContext subscriptionDbContext, IOrderProductService<Order> orderService, IPaymentService paymentService)
        {
            _commerceContext = commerceContext;
            _userManager = userManager;
            _subscriptionDbContext = subscriptionDbContext;
            _orderService = orderService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Get subscription by User
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Subscription>>> GetSubscriptionsByUserAsync()
        {
            var response = new ResultModel<IEnumerable<Subscription>>();
            var user = (await _userManager.GetCurrentUserAsync()).Result;

            if (user is null)
            {
                return new NotFoundResultModel<IEnumerable<Subscription>>();
            }

            var listSubscription = await _subscriptionDbContext.Subscription
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders).ToListAsync();

            response.IsSuccess = true;
            response.Result = listSubscription;

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
        public async Task<ResultModel<Guid>> CreateSubscriptionAsync(SubscriptionViewModel model)
        {
            if (model == null) throw new NullReferenceException();
            var orderRequest = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (!orderRequest.IsSuccess) return new InvalidParametersResultModel<Guid>();
            var order = orderRequest.Result;
            var isPayedRequest = await _paymentService.IsOrderPayedAsync(order.Id);
            if (!isPayedRequest.IsSuccess) return new ResultModel<Guid>
            {
                Errors = new List<IErrorModel> { new ErrorModel(string.Empty, "Order was not paid") }
            };
            var subscription = new Subscription
            {
                Id = model.Id,
                UserId = order.UserId,
                StartDate = model.StartDate,
                Availability = model.Availability,
                OrderId = model.OrderId,
                Name = model.Name,
                SubscriptionPermissions = model.SubscriptionPermissions
            };

            await _subscriptionDbContext.Subscription.AddAsync(subscription);
            var dbRequest = await _subscriptionDbContext.PushAsync();

            return dbRequest.Map(subscription.Id);
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
        /// Get subscription duration
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        public int GetSubscriptionDuration([Required]ProductVariation variation)
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
    }
}
