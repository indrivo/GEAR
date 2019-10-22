using ST.Core.Helpers;
using System.Threading.Tasks;

namespace ST.ECommerce.Payments.Abstractions
{
    public interface IPaymentManager
    {
        ResultModel Pay();
        Task<ResultModel> PayAsync();

    }
}