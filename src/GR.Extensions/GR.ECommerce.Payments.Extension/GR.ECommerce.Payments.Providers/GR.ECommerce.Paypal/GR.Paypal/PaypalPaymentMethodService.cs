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
using GR.Identity.Abstractions.Models.AddressModels;
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

        /// <summary>
        /// Inject user address
        /// </summary>
        private readonly IUserAddressService _userAddressService;

        #endregion

        public PaypalPaymentMethodService(IHttpClientFactory httpClientFactory,
            IOptionsSnapshot<PaypalExpressConfigForm> payPalOptions,
            IOrderProductService<Order> orderProductService,
            IPaymentService paymentService,
            IUserManager<ApplicationUser> userManager,
            IUserAddressService userAddressService)
        {
            _httpClientFactory = httpClientFactory;
            _payPalOptions = payPalOptions;
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _userAddressService = userAddressService;
        }

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResponsePaypal> CreatePaymentAsync(string hostingDomain, Guid? orderId)
        {
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {
                return new ResponsePaypal
                {
                    Message = "Order not Found",
                    IsSuccess = false
                };
            }

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess)
            {
                return new ResponsePaypal
                {
                    Message = "Order was payed before, Check your orders",
                    IsSuccess = false
                };
            }

            var order = orderRequest.Result;
            var addressRequest = await _userAddressService.GetAddressByIdAsync(order.BillingAddress);
            var address = addressRequest.IsSuccess ? addressRequest.Result : new Address();
            var user = await _userManager.UserManager.FindByIdAsync(order.UserId.ToString());

            var accessToken = await GetAccessTokenAsync();

            if (string.IsNullOrEmpty(accessToken))
            {
                return new ResponsePaypal { Message = "No access token", IsSuccess = false };
            }

            var shippingAddress = new ShippingAddress
            {
                City = address.StateOrProvince?.Name,
                CountryCode = address.Country?.Id,
                Phone = address.Phone,
                Line1 = address.AddressLine1,
                Line2 = address.AddressLine2,
                PostalCode = address.ZipCode,
                RecipientName = $"{user?.UserFirstName} {user?.UserLastName}"
            };

            using (var httpClient = new HttpClient())
            {
                var experienceProfileId = await CreateExperienceProfileAsync(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //TODO: Apply payment fee
                var unused = CalculatePaymentFee(order.Total);
                var paymentCreateRequest = new PaymentCreateRequest
                {
                    ExperienceProfileId = experienceProfileId,
                    Intent = "sale",
                    Payer = new Payer
                    {
                        PaymentMethod = "paypal"
                    },
                    Transactions = new[]
                    {
                        new Transaction
                        {
                            Amount = new Amount
                            {
                                Total = (order.Total + 0.11m).ToString("N2"),
                                Currency = "EUR",
                                Details = new Details
                                {
                                    Subtotal =  order.Total.ToString("N2"),
                                    Tax = "0.07",
                                    Shipping = "0.03",
                                    HandlingFee  = "1.00",
                                    ShippingDiscount = "-1.00",
                                    Insurance = "0.01"
                                }
                            },
                            Items = new ItemList
                            {
                                ShippingAddress = shippingAddress
                            }
                        }
                    },
                    RedirectUrls = new RedirectUrls
                    {
                        CancelUrl = $"{hostingDomain}/Paypal/Cancel",
                        ReturnUrl = $"{hostingDomain}/Paypal/Success"
                    },
                    Notes = order.Notes
                };
                var serializedData = paymentCreateRequest.SerializeAsJson();
                var content = new StringContent(serializedData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(
                    $"https://api{_payPalOptions.Value.EnvironmentUrlPart}.paypal.com/v1/payments/payment", content);

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
        public async Task<ResponsePaypal> ExecutePaymentAsync(PaymentExecuteVm model)
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
            var accessToken = await GetAccessTokenAsync();

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
                    UserId = userRequest.Result.Id.ToGuid(),
                    FailureMessage = responseBody
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
        private async Task<string> GetAccessTokenAsync()
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
        private async Task<string> CreateExperienceProfileAsync(string accessToken)
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
