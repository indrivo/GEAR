using System;
using System.Threading.Tasks;
using GR.ECommerce.Payments.Abstractions;
using GR.MobilPay.Abstractions.Models;
using MobilpayEncryptDecrypt;

namespace GR.MobilPay.Abstractions
{
    public interface IMobilPayPaymentMethod : IPaymentMethodService
    {
        /// <summary>
        /// Request invoice
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task RequestInvoicePaymentAsync(string hostingDomain, Guid? orderId);

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<MobilpayEncrypt> CreatePaymentAsync(string hostingDomain, Guid? orderId);

        /// <summary>
        /// Confirm payment
        /// </summary>
        /// <param name="textXml"></param>
        /// <param name="envKey"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<MobilPayPaymentResponse> ConfirmPaymentAsync(string textXml, string envKey, Guid? orderId);
    }
}