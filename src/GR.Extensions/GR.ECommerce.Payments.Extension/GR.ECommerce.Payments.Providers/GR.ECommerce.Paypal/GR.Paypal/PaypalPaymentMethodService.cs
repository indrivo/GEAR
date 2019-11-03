using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Paypal.Abstractions;
using GR.Paypal.Abstractions.ViewModels;
using GR.Paypal.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GR.Paypal
{
    public class PaypalPaymentMethodService : IPaypalPaymentMethodService
    {

        #region Injectable       

        /// <summary>
        /// Inject http client factory
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Inject service options
        /// </summary>
        private readonly IOptionsSnapshot<PaypalExpressConfigForm> _payPalOptions;

        /// <summary>
        /// Inject order service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        #endregion


        public PaypalPaymentMethodService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PaypalExpressConfigForm> payPalOptions, IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<ApplicationUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _payPalOptions = payPalOptions;
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResponsePaypal> CreatePayment(string hostingDomain, Guid? orderId)
        {
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {
                return new ResponsePaypal { Message = "Order not Found", IsSuccess = false };
            }

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess)
            {
                return new ResponsePaypal { Message = "Order was payed before, Check your orders", IsSuccess = false };
            }

            var order = orderRequest.Result;

            var accessToken = await GetAccessToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                return new ResponsePaypal { Message = "No access token", IsSuccess = false };
            }

            using (var httpClient = new HttpClient())
            {
                var experienceProfileId = await CreateExperienceProfile(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var paymentCreateRequest = new PaymentCreateRequest
                {
                    experience_profile_id = experienceProfileId,
                    intent = "sale",
                    payer = new Payer
                    {
                        payment_method = "paypal"
                    },
                    transactions = new[]
                    {
                        new Transaction
                        {
                            amount = new Amount
                            {
                                total = (order.Total + 0.11m).ToString("N2"),
                                currency = "USD",
                                details = new Details
                                {
                                    subtotal =  order.Total.ToString("N2"),
                                    tax = "0.07",
                                    shipping = "0.03",
                                    handling_fee = "1.00",
                                    shipping_discount = "-1.00",
                                    insurance = "0.01"
                                }
                            }
                        }
                    },
                    redirect_urls = new Redirect_Urls
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
                    await _orderProductService.ChangeOrderStateAsync(orderId, OrderState.PendingPayment);
                    string paymentId = payment.id;
                    return new ResponsePaypal { Message = paymentId, IsSuccess = true };
                }
                await _orderProductService.ChangeOrderStateAsync(orderId, OrderState.PaymentFailed);
                return new ResponsePaypal { Message = responseBody, IsSuccess = false };
            }
        }

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponsePaypal> ExecutePayment(PaymentExecuteVm model)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {
                return new ResponsePaypal { Message = "User not Found", IsSuccess = false };
            }
            var orderRequest = await _orderProductService.GetOrderByIdAsync(model.OrderId);
            if (!orderRequest.IsSuccess)
            {
                return new ResponsePaypal { Message = "Order not Found", IsSuccess = false };
            }
            var order = orderRequest.Result;
            var accessToken = await GetAccessToken();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var paymentExecuteRequest = new PaymentExecuteRequest { PayerId = model.PayerId };

                var response = await httpClient.PostJsonAsync(
                    $"https://api{_payPalOptions.Value.EnvironmentUrlPart}.paypal.com/v1/payments/payment/{model.PaymentId}/execute",
                    paymentExecuteRequest);
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JObject.Parse(responseBody);
                var payment = new Payment
                {
                    PaymentMethodId = "Paypal",
                    GatewayTransactionId = model.PaymentId,
                    PaymentStatus = PaymentStatus.Failed,
                    Total = order.Total,
                    UserId = userRequest.Result.Id.ToGuid()
                };

                if (response.IsSuccessStatusCode)
                {
                    await _orderProductService.ChangeOrderStateAsync(model.OrderId, OrderState.PaymentReceived);
                    payment.PaymentStatus = PaymentStatus.Succeeded;
                    await _paymentService.AddPaymentAsync(model.OrderId, payment);
                    return new ResponsePaypal
                    {
                        Message = "Payment received",
                        OrderId = model.OrderId,
                        IsSuccess = true
                    };
                }

                await _orderProductService.ChangeOrderStateAsync(model.OrderId, OrderState.PaymentFailed);
                payment.PaymentStatus = PaymentStatus.Failed;
                payment.FailureMessage = responseBody;
                await _paymentService.AddPaymentAsync(model.OrderId, payment);

                string errorName = responseObject.name;
                string errorDescription = responseObject.message;
                return new ResponsePaypal { Message = $"{errorName} - {errorDescription}", IsSuccess = false };
            }
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
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

        /// <summary>
        /// Create experience profile
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<string> CreateExperienceProfile(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var experienceRequest = new ExperienceProfile
                {
                    Name = $"gear_{Guid.NewGuid()}",
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

        /// <summary>
        /// Calculate payment fee
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        private decimal CalculatePaymentFee(decimal total)
        {
            var percent = _payPalOptions.Value.PaymentFee;
            return total / 100 * percent;
        }
    }
}
