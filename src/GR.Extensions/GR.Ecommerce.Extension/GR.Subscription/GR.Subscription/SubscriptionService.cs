using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.Identity.Abstractions;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.Subscriptions
{
   public class SubscriptionService : ISubscriptionService
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

        #endregion

        public SubscriptionService(ICommerceContext commerceContext,  IUserManager<ApplicationUser> userManager, ISubscriptionDbContext subscriptionDbContext)
        {
            _commerceContext = commerceContext;
            _userManager = userManager;
            _subscriptionDbContext = subscriptionDbContext;


        }
        
        /// <summary>
        /// Get subscription by User
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Subscription>>> GetSubscriptionByUserAsync()
        {
            var response = new ResultModel<IEnumerable<Subscription>>();
            var user = (await _userManager.GetCurrentUserAsync()).Result;

            if (user is null)
            {
                return new NotFoundResultModel<IEnumerable<Subscription>>();
            }

            var listSubscription = await _subscriptionDbContext.Subscription
                .Include(i => i.Order)
                .ThenInclude(i=>i.ProductOrders).ToListAsync();

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

            if(subscriptionId is null)
                return new InvalidParametersResultModel<Subscription>();

            var subscription = await _subscriptionDbContext.Subscription
                .Include(i => i.Order)
                .ThenInclude(i => i.ProductOrders)
                .FirstOrDefaultAsync(x=> x.Id == subscriptionId);

            if(subscription is null) new NotFoundResultModel<Subscription>();

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
            var user = (await _userManager.GetCurrentUserAsync()).Result;
            if (user == null) return new NotFoundResultModel<Guid>();

            var subscription = new Subscription
            {
                UserId = user.Id.ToGuid(),
                StartDate = model.StartDate,
                Valability = model.Valability,
                OrderId = model.OrderId
            };
            
            await _subscriptionDbContext.Subscription.AddAsync(subscription);
            var dbRequest =  await _commerceContext.PushAsync();

            return dbRequest.Map(subscription.Id);
        }

        /// <summary>
        /// Has valids subscription
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<bool>> HasValidsSubscription()
        {
            var toReturn = new ResultModel<bool>();
            var user = (await _userManager.GetCurrentUserAsync()).Result;

            if (user is null) return new NotFoundResultModel<bool>();

            var listSubscription = (await GetSubscriptionByUserAsync()).Result;

            toReturn.IsSuccess = true;
            toReturn.Result =  listSubscription.Any(x => x.IsValid);

            return toReturn;
        }
    }
}
