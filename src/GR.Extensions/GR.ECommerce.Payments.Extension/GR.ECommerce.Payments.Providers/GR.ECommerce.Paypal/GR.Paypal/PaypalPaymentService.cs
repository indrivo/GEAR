using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.Paypal.Abstractions;
using GR.Paypal.Abstractions.ViewModels;
using GR.Paypal.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GR.Paypal
{
    public class PaypalPaymentService : IPaypalPaymentService
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
        #endregion


        public PaypalPaymentService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PaypalExpressConfigForm> payPalOptions, IOrderProductService<Order> orderProductService)
        {
            _httpClientFactory = httpClientFactory;
            _payPalOptions = payPalOptions;
            _orderProductService = orderProductService;
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
                return new ResponsePaypal { Message = "Order not Found", IsSucces = false };
            }

            var order = orderRequest.Result;

            var accessToken = await GetAccessToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                return new ResponsePaypal { Message = "No access token", IsSucces = false };
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
                                total = (order.Total + 0.11).ToString("N2"),
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
                    string paymentId = payment.id;
                    return new ResponsePaypal { Message = paymentId, IsSucces = true };
                }

                return new ResponsePaypal { Message = responseBody, IsSucces = false }; ;
            }
        }

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponsePaypal> ExecutePayment(PaymentExecuteVm model)
        {
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
                if (response.IsSuccessStatusCode)
                {
                    return new ResponsePaypal { Message = "12", IsSucces = true };
                }

                string errorName = responseObject.name;
                string errorDescription = responseObject.message;
                return new ResponsePaypal { Message = $"{errorName} - {errorDescription}", IsSucces = false };
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
