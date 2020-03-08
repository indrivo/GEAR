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
        Task<ResultModel<Guid>> CreateUpdateSubscriptionAsync(SubscriptionViewModel model);

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

        /// <summary>
        /// Get valid subscriptions for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Subscription>>> GetValidSubscriptionsForUserAsync(Guid? userId);

        /// <summary>
        /// Get subscriptions that will expire after some period
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Subscription>>> GetSubscriptionsThatExpireInAsync(TimeSpan timeSpan);

        /// <summary>
        /// Get expired permissions
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Subscription>>> GetExpiredSubscriptionsAsync();

        /// <summary>
        /// Remove range
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveRangeAsync(IEnumerable<Subscription> subscriptions);

        /// <summary>
        /// Notify expired subscriptions
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        Task<ResultModel> NotifyAndRemoveExpiredSubscriptionsAsync([Required] IList<Subscription> subscriptions);

        /// <summary>
        /// Notify subscriptions that will expire soon
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        Task NotifySubscriptionsThatExpireAsync([Required] IList<Subscription> subscriptions);

        /// <summary>
        /// Get last subscription for user
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<Subscription>> GetLastSubscriptionForUserAsync(Guid? userId = null);
    }
}