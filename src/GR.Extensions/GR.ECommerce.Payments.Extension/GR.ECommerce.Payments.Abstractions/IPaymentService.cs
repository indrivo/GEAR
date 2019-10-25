using System.Threading.Tasks;
using GR.Core.Helpers;

namespace GR.ECommerce.Payments.Abstractions
{
    public interface IPaymentService
    {
        ResultModel Pay();
        Task<ResultModel> PayAsync();
    }
}