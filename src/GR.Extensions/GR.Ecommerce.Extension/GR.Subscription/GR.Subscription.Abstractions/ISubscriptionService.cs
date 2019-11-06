﻿using GR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GR.Subscriptions.Abstractions.ViewModels;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Subscriptions.Abstractions
{
    public interface ISubscriptionService
    {

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Subscription>>> GetSubscriptionByUserAsync();

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<Subscription>> GetSubscriptionByIdAsync(Guid? subscriptionId);


        /// <summary>
        /// create subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateSubscriptionAsync(SubscriptionViewModel model);

        /// <summary>
        /// Has valids subscription
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<bool>> HasValidsSubscription();
    }
}
