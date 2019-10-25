using GR.ECommerce.Payments.Abstractions;
using GR.Paypal.Abstractions.ViewModels;
using GR.Paypal.Razor.ViewModels;
using System.Threading.Tasks;

namespace GR.ECommerce.Paypal.Abstractions
{
    public interface IPaypalPaymentService : IPaymentService
    {
        Task<ResponsePaypal> CreatePayment(string hostingDomain);
        Task<ResponsePaypal> ExecutePayment(PaymentExecuteVm model);
    }
}