using System;
using System.Threading.Tasks;
using GR.ECommerce.Payments.Abstractions;
using GR.Paypal.Abstractions.ViewModels;

namespace GR.Paypal.Abstractions
{
    public interface IPaypalPaymentMethodService : IPaymentMethodService
    {
        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResponsePaypal> CreatePaymentAsync(string hostingDomain, Guid? orderId);

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponsePaypal> ExecutePaymentAsync(PaymentExecuteVm model);

        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns></returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// Create experience profile
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<string> CreateExperienceProfileAsync(string accessToken);

        /// <summary>
        /// Calculate payment fee
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        decimal CalculatePaymentFee(decimal total);
    }
}