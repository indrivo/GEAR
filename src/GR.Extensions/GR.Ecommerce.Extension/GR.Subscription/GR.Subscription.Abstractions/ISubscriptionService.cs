using GR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GR.Subscription.Abstractions.ViewModels;

namespace GR.Subscription.Abstractions
{
    public interface ISubscriptionService
    {

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Models.Subscription>>> GetSubscriptionByUserAsync();

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<Models.Subscription>> GetSubscriptionByIdAsync(Guid? orderId);


        /// <summary>
        /// create subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateSubscriptionAsync(SubscriptionViewModel model);
    }
}
