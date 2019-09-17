using System;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.ECommerce.Payments.Abstractions;

namespace ST.ECommerce.Paypal.Impl
{
    public class PaypalPaymentManager : IPaymentManager
    {
        public ResultModel Pay()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> PayAsync()
        {
            throw new NotImplementedException();
        }
    }
}
