using System;
using System.Threading.Tasks;
using GR.Braintree.Abstractions;
using GR.Core.Razor.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Braintree.Razor.Controllers
{
    [Authorize]
    public class BraintreeController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject Braintree method
        /// </summary>
        private readonly IBraintreePaymentMethod _braintreePaymentMethod;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="braintreePaymentMethod"></param>
        public BraintreeController(IBraintreePaymentMethod braintreePaymentMethod)
        {
            _braintreePaymentMethod = braintreePaymentMethod;
        }

        /// <summary>
        /// Pay with Braintree
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Charge(Guid? orderId, string nonce)
        {
            var payResult = await _braintreePaymentMethod.ChargeAsync(orderId, nonce);
            if (payResult.IsSuccess) return Ok(payResult);

            var errorMessages = "";
            foreach (var error in payResult.Errors)
            {
                errorMessages += "Error: " + error.Key + " - " + error.Message + "\n";
            }

            return BadRequest(errorMessages);
        }
    }
}