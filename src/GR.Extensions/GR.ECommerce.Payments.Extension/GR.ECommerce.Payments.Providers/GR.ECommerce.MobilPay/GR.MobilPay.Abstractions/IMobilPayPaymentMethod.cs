using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
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
        Task<ResultModel<Dictionary<string, string>>> RequestInvoicePaymentAsync(string hostingDomain, Guid? orderId);

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<MobilpayEncrypt>> CreatePaymentAsync(string hostingDomain, Guid? orderId);

        /// <summary>
        /// Confirm payment
        /// </summary>
        /// <param name="textXml"></param>
        /// <param name="envKey"></param>
        /// <returns></returns>
        Task<MobilPayPaymentResponse> ConfirmPaymentAsync(string textXml, string envKey);
    }
}