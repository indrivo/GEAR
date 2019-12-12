using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions.Models;

namespace GR.ECommerce.Payments.Abstractions
{
    public interface IPaymentService
    {
        Task<ResultModel<IEnumerable<PaymentMethod>>> GetActivePaymentMethodsAsync();
        Task<ResultModel<IEnumerable<PaymentMethod>>> GetAllPaymentMethodsAsync();
        Task<ResultModel<IEnumerable<Payment>>> GetPaymentsForOrderAsync(Guid? orderId);
        Task<ResultModel> IsOrderPayedAsync(Guid? orderId);
        Task<ResultModel> AddPaymentAsync(Guid? orderId, Payment payment);
    }
}
