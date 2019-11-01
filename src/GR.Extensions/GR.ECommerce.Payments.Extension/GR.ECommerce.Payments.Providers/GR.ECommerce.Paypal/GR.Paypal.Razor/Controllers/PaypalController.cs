using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Paypal.Abstractions;
using GR.Paypal.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GR.Paypal.Razor.Controllers
{
    public class PaypalController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaypalPaymentService _paymentManager;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PaypalController()
        {
            _paymentManager = IoC.Resolve<IPaypalPaymentService>(nameof(PaypalPaymentService));
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePayment(Guid? orderId)
        {
            var hostingDomain = Request.Host.Value;

            var response = await _paymentManager.CreatePayment(hostingDomain, orderId);
            if (response.IsSucces)
            {
                var paymentId = response.Message;
                return Ok(new { PaymentId = paymentId });
            }

            return BadRequest(response.Message);
        }


        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExecutePayment(PaymentExecuteVm model)
        {
            var response = await _paymentManager.ExecutePayment(model);

            if (response.IsSucces)
            {
                return Ok(new { Status = "success", OrderId = response.Message });
            }

            return BadRequest(response.Message);
        }
    }
}