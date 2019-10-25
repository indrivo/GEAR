using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Paypal.Abstractions;
using GR.ECommerce.Paypal.Models;
using GR.Paypal.Abstractions.ViewModels;
using GR.Paypal.Razor.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GR.ECommerce.Paypal
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
        #endregion


        public PaypalPaymentService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PaypalExpressConfigForm> payPalOptions)
        {
            _httpClientFactory = httpClientFactory;
            _payPalOptions = payPalOptions;           
        }

        public ResultModel Pay()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> PayAsync()
        {
            throw new NotImplementedException();
        }


        public async Task<ResponsePaypal> CreatePayment(string hostingDomain)
        {
            var accessToken = await GetAccessToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                return new ResponsePaypal() {Message = "No access token", IsSucces = false};
            }

            using (var httpClient = new HttpClient())
            {
                var experienceProfileId = await CreateExperienceProfile(accessToken);
                dynamic cart = new { OrderTotal = 2, SubTotalWithDiscountWithoutTax = 1, TaxAmount = 1, ShippingAmount = 2 };
                if (cart == null)
                {
                    return  new ResponsePaypal() { Message = "NotFound", IsSucces = false };
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
                                //total = (cart.OrderTotal + CalculatePaymentFee(cart.OrderTotal)).ToString("N2",
                                //    paypalAcceptedNumericFormatCulture),
                                total = "30.11",
                                currency = "USD",
                                details = new Details
                                {
                                    //handling_fee = CalculatePaymentFee(cart.OrderTotal)
                                    //    .ToString("N2", paypalAcceptedNumericFormatCulture),
                                    //subtotal = cart.SubTotalWithDiscountWithoutTax.ToString("N2",
                                    //    paypalAcceptedNumericFormatCulture),
                                    //tax = cart.TaxAmount?.ToString("N2", paypalAcceptedNumericFormatCulture) ?? "0",
                                    //shipping =
                                    //    cart.ShippingAmount?.ToString("N2", paypalAcceptedNumericFormatCulture) ?? "0"
                                    subtotal = "30.00",
                                    tax = "0.07",
                                    shipping = "0.03",
                                    handling_fee = "1.00",
                                    shipping_discount = "-1.00",
                                    insurance = "0.01"
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
                    return new ResponsePaypal() { Message = paymentId, IsSucces = true };
                }

                return new ResponsePaypal() { Message = responseBody, IsSucces = false }; ;
            }
        }


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
                    // Has to explicitly declare the type to be able to get the propery
                    //string payPalPaymentId = responseObject.id;
                    //payment.Status = PaymentStatus.Succeeded;
                    //payment.GatewayTransactionId = payPalPaymentId;
                    //_paymentRepository.Add(payment);
                    //order.OrderStatus = OrderStatus.PaymentReceived;
                    //await _paymentRepository.SaveChangesAsync();
                   // return Ok(new { Status = "success", OrderId = 12 });
                    return new ResponsePaypal() { Message = "12", IsSucces = true };
                }

                string errorName = responseObject.name;
                string errorDescription = responseObject.message;
                //return BadRequest($"{errorName} - {errorDescription}");
                return new ResponsePaypal() { Message = $"{errorName} - {errorDescription}", IsSucces = false };
            }
           
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

         private decimal CalculatePaymentFee(decimal total)
        {
            var percent = _payPalOptions.Value.PaymentFee;
            return total / 100 * percent;
        }
    }
}
