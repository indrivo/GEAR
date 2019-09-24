using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ST.Core.Helpers;
using ST.ECommerce.Payments.Abstractions;
using ST.ECommerce.Paypal.Impl;
using ST.ECommerce.Paypal.Impl.Extensions;
using ST.ECommerce.Paypal.Razor.ViewModels;

namespace ST.ECommerce.Paypal.Razor.Controllers
{
    public class PaypalController : Controller
    {
        private readonly IPaymentManager _paymentManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaypalController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _paymentManager = IoC.Resolve<IPaymentManager>(nameof(PaypalPaymentManager));
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
            var accessToken = await GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("No access token");
            }

            using (var httpClient = new HttpClient())
            {
                //var regionInfo = new RegionInfo(_currencyService.CurrencyCulture.LCID)
                var experienceProfileId = await CreateExperienceProfile(accessToken);
                var cart = GetActiveCartDetails(Guid.NewGuid());
                if (cart == null)
                {
                    return NotFound();
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var paypalAcceptedNumericFormatCulture = CultureInfo.CreateSpecificCulture("en-US");

                var paymentCreateRequest = new PaymentCreateRequest
                {
                    experience_profile_id = experienceProfileId,
                    intent = "sale",
                    payer = new Payer
                    {
                        payment_method = "paypal",
                    },
                    transactions = new[]
                    {
                        new Transaction
                        {
                            amount = new Amount
                            {
                                total = (cart.OrderTotal + CalculatePaymentFee(cart.OrderTotal)).ToString("N2",
                                    paypalAcceptedNumericFormatCulture),
                                currency = "USD",
                                details = new Details
                                {
                                    handling_fee = CalculatePaymentFee(cart.OrderTotal)
                                        .ToString("N2", paypalAcceptedNumericFormatCulture),
                                    subtotal = cart.SubTotalWithDiscountWithoutTax.ToString("N2",
                                        paypalAcceptedNumericFormatCulture),
                                    tax = cart.TaxAmount?.ToString("N2", paypalAcceptedNumericFormatCulture) ?? "0",
                                    shipping =
                                        cart.ShippingAmount?.ToString("N2", paypalAcceptedNumericFormatCulture) ?? "0"
                                }
                            }
                        }
                    },
                    redirect_urls = new Redirect_Urls()
                    {
                        cancel_url = $"http://{hostingDomain}/Paypal/Cancel",
                        return_url = $"http://{hostingDomain}/Paypal/Success"
                    }
                };

                var response = await httpClient.PostJsonAsync(
                    $"https://api{_payPalOptions.Value.EnvironmentUrlPart}.paypal.com/v1/payments/payment",
                    paymentCreateRequest);

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic payment = JObject.Parse(responseBody);
                if (response.IsSuccessStatusCode)
                {
                    string paymentId = payment.id;
                    return Ok(new {PaymentId = paymentId});
                }

                return BadRequest(responseBody);
            }
        }


        public async Task<IActionResult> ExecutePayment(PaymentExecuteVm model)
        {
            var accessToken = await GetAccessToken();
            //var currentUser = await 
            var cart = GetActiveCart();
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

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var paymentExecuteRequest = new PaymentExecuteRequest {PayerId = model.PayerId};

                var response = await httpClient.PostJsonAsync(
                    $"https://api{_payPalOptions.Value.EnvironmentUrlPart}.paypal.com/v1/payments/payment/{model.PaymentId}/execute",
                    paymentExecuteRequest);
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JObject.Parse(responseBody);
                if (response.IsSuccessStatusCode)
                {
                    // Has to explicitly declare the type to be able to get the propery
                    //string payPalPaymentId = responseObject.id;
                    //payment.Status = PaymentStatus.Succeeded;
                    //payment.GatewayTransactionId = payPalPaymentId;
                    //_paymentRepository.Add(payment);
                    //order.OrderStatus = OrderStatus.PaymentReceived;
                    //await _paymentRepository.SaveChangesAsync();
                    return Ok(new {Status = "success", OrderId = 12});
                }

                string errorName = responseObject.name;
                string errorDescription = responseObject.message;
                return BadRequest($"{errorName} - {errorDescription}");
            }


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

        private async Task<string> GetAccessToken()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes(
                        $"{_payPalOptions.Value.ClientId}:{_payPalOptions.Value.ClientSecret}")));
            var requestBody = new StringContent("grant_type=client_credentials");
            requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync(
                $"https://api{_payPalOptions.Value.EnvironmentUrlPart}.paypal.com/v1/oauth2/token", requestBody);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic token = JObject.Parse(responseBody);
            string accessToken = token.access_token;
            return accessToken;
        }


        private decimal CalculatePaymentFee(decimal total)
        {
            var percent = _payPalOptions.Value.PaymentFee;
            return total / 100 * percent;
        }

        private async Task<string> CreateExperienceProfile(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var experienceRequest = new ExperienceProfile
                {
                    Name = $"simpl_{Guid.NewGuid()}",
                    InputFields = new InputFields
                    {
                        NoShipping = 1
                    },
                    Temporary = true
                };
                var response = await httpClient.PostJsonAsync(
                    $"https://api{_payPalOptions.Value.EnvironmentUrlPart}.paypal.com/v1/payment-experience/web-profiles",
                    experienceRequest);
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic experience = JObject.Parse(responseBody);
                string profileId = experience.id;
                return profileId;
            }
        }
    }
}