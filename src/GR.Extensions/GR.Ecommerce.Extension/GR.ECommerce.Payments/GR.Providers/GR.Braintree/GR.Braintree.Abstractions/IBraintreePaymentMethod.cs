using System;
using System.Threading.Tasks;
using Braintree;
using GR.Braintree.Abstractions.Models;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;

namespace GR.Braintree.Abstractions
{
    public interface IBraintreePaymentMethod : IPaymentMethodService
    {
        /// <summary>
        /// Get client token
        /// </summary>
        /// <returns></returns>
        Task<string> GetClientToken();

        /// <summary>
        /// Create new gateway
        /// </summary>
        /// <returns></returns>
        IBraintreeGateway CreateGateway();

        /// <summary>
        /// Make a braintree payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        Task<ResultModel<BraintreeSuccessPaymentResult>> ChargeAsync(Guid? orderId, string nonce);
    }
}