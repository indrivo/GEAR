using System;
using System.Threading.Tasks;
using GR.ECommerce.Payments.Abstractions;
using MobilpayEncryptDecrypt;

namespace GR.MobilPay.Abstractions
{
    public interface IMobilPayPaymentMethod : IPaymentMethodService
    {
        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<MobilpayEncrypt> CreatePaymentAsync(string hostingDomain, Guid? orderId);
    }
}