using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.GooglePay.Abstractions.Models;
using GR.GooglePay.Abstractions.ViewModels;

namespace GR.GooglePay.Abstractions
{
    public interface IGPayPaymentMethodService : IPaymentMethodService
    {
        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<GPayTransactionInfo>> CreatePaymentAsync(Guid? orderId);

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> ExecutePaymentAsync(GPayPaymentExecuteViewModel model);
    }
}