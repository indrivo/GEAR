using System.Collections.Generic;
using System.Linq;
using Braintree;
using GR.Core.Helpers;

namespace GR.Braintree.Extensions
{
    public static class BraintreeValidationExtensions
    {
        /// <summary>
        /// To gear errors
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static IEnumerable<IErrorModel> ToGearErrors(this ValidationErrors errors)
        {
            return errors.DeepAll().Select(x => new ErrorModel(x.Code.ToString(), x.Message));
        }
    }
}
