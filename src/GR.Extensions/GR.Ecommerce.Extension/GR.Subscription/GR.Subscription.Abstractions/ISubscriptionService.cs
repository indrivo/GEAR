using GR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.ECommerce.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Subscriptions.Abstractions
{
    public interface ISubscriptionService<TSubscriptionEntity> where TSubscriptionEntity : Subscription
    {

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<TSubscriptionEntity>>> GetSubscriptionsByUserAsync();

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        Task<ResultModel<TSubscriptionEntity>> GetSubscriptionByIdAsync(Guid? subscriptionId);


        /// <summary>
        /// create subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateSubscriptionAsync(SubscriptionViewModel model);

        /// <summary>
        /// Has valid subscription
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<bool>> HasValidSubscription();

        /// <summary>
        /// Get duration in days
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        int GetSubscriptionDuration([Required] ProductVariation variation);
    }
}
