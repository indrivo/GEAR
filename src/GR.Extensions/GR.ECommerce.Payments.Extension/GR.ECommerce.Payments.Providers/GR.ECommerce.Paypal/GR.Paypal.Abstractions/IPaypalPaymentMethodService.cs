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
        Task<ResponsePaypal> CreatePayment(string hostingDomain, Guid? orderId);

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponsePaypal> ExecutePayment(PaymentExecuteVm model);
    }
}