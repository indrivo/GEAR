using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Paypal;
using GR.ECommerce.Paypal.Abstractions;
using GR.ECommerce.Paypal.Models;
using GR.Paypal.Razor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
#pragma warning disable 1998

namespace GR.Paypal.Razor.Controllers
{
    public class PaypalController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaypalPaymentService _paymentManager;

        /// <summary>
        /// Inject http client factory
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Inject service options
        /// </summary>
        private readonly IOptionsSnapshot<PaypalExpressConfigForm> _payPalOptions;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="payPalOptions"></param>
        public PaypalController(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PaypalExpressConfigForm> payPalOptions)
        {
            _httpClientFactory = httpClientFactory;
            _payPalOptions = payPalOptions;
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


        [HttpPost]
        public async Task<IActionResult> CreatePayment()
        {
            var hostingDomain = Request.Host.Value;

                var response = _paymentManager.CreatePayment(hostingDomain);
                if (response.Result.IsSucces)
                {
                    string paymentId = response.Result.Message;
                    return Ok(new { PaymentId = paymentId });
                }

                return BadRequest(response.Result.Message);
        }


        public async Task<IActionResult> ExecutePayment(PaymentExecuteVm model)
        {
            //var accessToken = await GetAccessToken();
            //var currentUser = await 
            //var cart = GetActiveCart();
            //var orderCreateResult = await _orderService.CreateOrder(cart.Id, "PaypalExpress",
            //    CalculatePaymentFee(cart.OrderTotal), OrderStatus.PendingPayment);
            //if (!orderCreateResult.Success)
            //{
            //    return BadRequest(orderCreateResult.Error);
            //}

            //var order = orderCreateResult.Value;
            //var payment = new Payme()
            //{
            //    OrderId = order.Id,
            //    PaymentFee = order.PaymentFeeAmount,
            //    Amount = order.OrderTotal,
            //    PaymentMethod = "Paypal Express",
            //    CreatedOn = DateTimeOffset.UtcNow,
            //};

            //using (var httpClient = new HttpClient())
            //{
            //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //    var paymentExecuteRequest = new PaymentExecuteRequest { PayerId = model.PayerId };

            //    var response = await httpClient.PostJsonAsync(
            //        $"https://api{_payPalOptions.Value.EnvironmentUrlPart}.paypal.com/v1/payments/payment/{model.PaymentId}/execute",
            //        paymentExecuteRequest);
            //    var responseBody = await response.Content.ReadAsStringAsync();
            //    dynamic responseObject = JObject.Parse(responseBody);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        // Has to explicitly declare the type to be able to get the propery
            //        //string payPalPaymentId = responseObject.id;
            //        //payment.Status = PaymentStatus.Succeeded;
            //        //payment.GatewayTransactionId = payPalPaymentId;
            //        //_paymentRepository.Add(payment);
            //        //order.OrderStatus = OrderStatus.PaymentReceived;
            //        //await _paymentRepository.SaveChangesAsync();
            //        return Ok(new { Status = "success", OrderId = 12 });
            //    }

            //    string errorName = responseObject.name;
            //    string errorDescription = responseObject.message;
            //    return BadRequest($"{errorName} - {errorDescription}");
            //}


            var response = _paymentManager.ExecutePayment(model);

            if (response.Result.IsSucces)
            {
                return Ok(new { Status = "success", OrderId = response.Result.Message });
            }

            return BadRequest(response.Result.Message);

            //payment.Status = PaymentStatus.Failed;
            //payment.FailureMessage = responseBody;
            //_paymentRepository.Add(payment);
            //order.OrderStatus = OrderStatus.PaymentFailed;
            //await _paymentRepository.SaveChangesAsync();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Cancel()
        {
            return default;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Success()
        {
            return default;
        }
    }
}