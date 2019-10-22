using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Paypal.Abstractions;

namespace GR.ECommerce.Paypal
{
    public class PaypalPaymentService : IPaypalPaymentService
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
