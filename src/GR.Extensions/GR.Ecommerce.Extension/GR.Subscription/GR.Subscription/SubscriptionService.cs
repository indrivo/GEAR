
using GR.Subscription.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Orders.Abstractions;
using GR.Subscription.Abstractions.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.Subscription
{
    class SubscriptionService : ISubscriptionService
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


        public async Task<ResultModel<IEnumerable<Abstractions.Models.Subscription>>> GetSubscriptionByUserAsync()
        {
            var response = new ResultModel<IEnumerable<Abstractions.Models.Subscription>>();
            var user = (await _userManager.GetCurrentUserAsync()).Result;

            if (user is null)
            {
                return new InvalidParametersResultModel<IEnumerable<Abstractions.Models.Subscription>>();
            }

            var listSubscription = _subscriptionDbContext.Subscription.Include(i => i.Order).ThenInclude(i=>i.ProductOrders);

            response.IsSuccess = true;
            response.Result = listSubscription;

            return response;
        }

        public async Task<ResultModel<Abstractions.Models.Subscription>> GetSubscriptionByIdAsync(Guid? orderId)
        {
            var response = new ResultModel<Abstractions.Models.Subscription>();

            return response;
        }

        public async Task<ResultModel<Guid>> CreateSubscriptionAsync(SubscriptionViewModel model)
        {
            var response = new ResultModel<Guid>();

            return response;
        }
    }
}
