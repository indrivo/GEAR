using GR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.ECommerce.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Subscriptions.Abstractions.ViewModels;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Subscriptions.Abstractions
{
    public interface ISubscriptionService<TSubscriptionEntity> where TSubscriptionEntity : Subscription
    {
        /// <summary>
        /// Get subscription plans
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<SubscriptionPlanViewModel>>> GetSubscriptionPlansAsync();

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<SubscriptionGetViewModel>>> GetSubscriptionsByUserAsync();

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
        Task<ResultModel<Guid>> AddOrUpdateSubscriptionAsync(SubscriptionAddViewModel model);

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
        Task<ResultModel<IEnumerable<SubscriptionGetViewModel>>> GetValidSubscriptionsForUserAsync(Guid? userId);

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

        /// <summary>
        /// Get last subscription
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<SubscriptionGetViewModel>> GetLastSubscriptionAsync();

        /// <summary>
        /// Get users in subscription by name. Subscription must be valid
        /// </summary>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GearUser>>> GetUsersInSubscriptionAsync(string subscriptionName);

        /// <summary>
        /// Get users in subscription by name. Subscription must be valid
        /// </summary>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Guid>>> GetUsersIdInSubscriptionAsync(string subscriptionName);

        /// <summary>
        /// Get total incoming
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<SubscriptionsTotalViewModel>> GetTotalIncomeResourcesAsync();

        /// <summary>
        /// Get user subscription info
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<SubscriptionUserInfoViewModel>> GetUsersSubscriptionInfoWithPaginationAsync(DTParameters parameters);

        /// <summary>
        /// Extend user subscription
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> ExtendUserSubscriptionAsync(Guid orderId, Guid userId);

        /// <summary>
        ///     Replace subscription
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="period"></param>
        /// <param name="unit"></param>
        /// <param name="multiplyIndex"></param>
        /// <returns></returns>
        Task<ResultModel> AddSubscriptionAsync(Guid userId, Guid productId, string period,
            string unit, int multiplyIndex = 1);

        /// <summary>
        /// Add default free subscription
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> AddDefaultFreeSubscriptionAsync(Guid userId);
    }
}