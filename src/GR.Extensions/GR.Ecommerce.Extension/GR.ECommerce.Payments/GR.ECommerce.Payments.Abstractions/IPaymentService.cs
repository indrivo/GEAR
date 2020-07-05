using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions.ViewModels;

namespace GR.ECommerce.Payments.Abstractions
{
    public interface IPaymentService
    {
        /// <summary>
        /// Get active payment methods
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<PaymentMethodViewModel>>> GetActivePaymentMethodsAsync();

        /// <summary>
        /// Get all payment methods
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<PaymentMethodViewModel>>> GetAllPaymentMethodsAsync();

        /// <summary>
        /// Get payments for order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Payment>>> GetPaymentsForOrderAsync(Guid? orderId);
        /// <summary>
        /// Is order payed
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel> IsOrderPayedAsync(Guid? orderId);
        /// <summary>
        /// Add payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        Task<ResultModel> AddPaymentAsync(Guid? orderId, Payment payment);
        /// <summary>
        /// Activate payment method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> ActivatePaymentMethodAsync(string id);
        /// <summary>
        /// Disable payment method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> DisablePaymentMethodAsync(string id);

        /// <summary>
        /// Check if payment method is supported
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<ResultModel> IsPaymentMethodSupportedAsync(string method);
    }
}