using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Subscriptions.Abstractions
{
    public interface ISubscriptionExpirationService
    {
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
    }
}