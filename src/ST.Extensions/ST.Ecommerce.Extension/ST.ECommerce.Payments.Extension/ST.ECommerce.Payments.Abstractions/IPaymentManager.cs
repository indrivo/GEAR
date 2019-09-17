using ST.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ST.ECommerce.Payments.Abstractions
{
    public interface IPaymentManager
    {
        ResultModel Pay();
        Task<ResultModel> PayAsync();
    }
}
