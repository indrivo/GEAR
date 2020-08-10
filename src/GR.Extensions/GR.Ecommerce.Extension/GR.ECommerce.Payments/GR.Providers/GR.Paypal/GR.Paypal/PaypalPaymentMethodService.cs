using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Paypal.Abstractions;
using GR.Paypal.Abstractions.Helpers;
using GR.Paypal.Abstractions.Models;
using GR.Paypal.Abstractions.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GR.Paypal
{
    [Author(Authors.LUPEI_NICOLAE)]
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
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject user address
        /// </summary>
        private readonly IUserAddressService _userAddressService;

        #endregion

        public PaypalPaymentMethodService(IHttpClientFactory httpClientFactory,
            IOptionsSnapshot<PaypalExpressConfigForm> payPalOptions,
            IOrderProductService<Order> orderProductService,
            IPaymentService paymentService,
            IUserManager<GearUser> userManager,
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
            var result = new ResponsePaypal
            {
                OrderId = orderId
            };
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {
                result.AddError("Order not Found");
                return result;
            }

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess)
            {
                result.AddError("Order was payed before, Check your orders");
                return result;
            }

            var wasInvoicedRequest = await _orderProductService.ItWasInTheStateAsync(orderId, OrderState.Invoiced);
            if (!wasInvoicedRequest.Result)
            {
                result.AddError("Invoice was not created, before calling this api, you must create invoice");
                return result;
            }

            var order = orderRequest.Result;
            var userId = _userManager.FindUserIdInClaims().Result;
            if (!userId.Equals(order.UserId))
            {
                result.AddError("This order was created by another person");
                return result;
            }
            var addressRequest = await _userAddressService.GetAddressByIdAsync(order.BillingAddress);
            var address = addressRequest.IsSuccess ? addressRequest.Result : new Address();
            var user = await _userManager.UserManager.FindByIdAsync(order.UserId.ToString());

            var accessToken = await GetAccessTokenAsync();

            if (string.IsNullOrEmpty(accessToken))
            {
                result.AddError("No access token");
                return result;
            }

            var fullName = user?.FirstName != null && user.LastName != null
                ? $"{user.FirstName} {user.LastName}"
                : user?.UserName;

            var _ = new ShippingAddress
            {
                City = address.StateOrProvince?.Name,
                CountryCode = address.Country?.Id?.ToUpper(),
                Phone = address.Phone,
                Line1 = address.AddressLine1,
                Line2 = address.AddressLine2,
                PostalCode = address.ZipCode,
                RecipientName = fullName
            };

            var items = new List<Item>();
            foreach (var item in order.ProductOrders)
            {
                items.Add(new Item
                {
                    Name = item.Product.Name,
                    Currency = order.Currency?.Code,
                    Price = item.Product.PriceWithoutDiscount.ToString("N2"),
                    Description = item.Product.Description?.StripHtml(),
                    Quantity = item.Amount.ToString(),
                    Sku = item.Product.Sku,
                    Tax = "0"
                });
            }

            using (var httpClient = new HttpClient())
            {
                var experienceProfileId = await CreateExperienceProfileAsync(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var paypalAcceptedNumericFormatCulture = CultureInfo.CreateSpecificCulture("en-US");
                var paymentCreateRequest = new PaymentCreateRequest
                {
                    ExperienceProfileId = experienceProfileId,
                    Intent = "sale",
                    Payer = new Payer
                    {
                        PaymentMethod = "paypal",
                        PayerInfo = new PayerInfo
                        {
                            Email = user?.Email
                        }
                    },
                    Transactions = new[]
                    {
                        new Transaction
                        {
                            Amount = new Amount
                            {
                                Total = (order.Total + CalculatePaymentFee(order.Total)).ToString("N2", paypalAcceptedNumericFormatCulture),
                                Currency = order.Currency?.Code,
                                Details = new Details
                                {
                                    Subtotal =  order.Total.ToString("N2", paypalAcceptedNumericFormatCulture),
                                    Tax = "0",
                                    HandlingFee  = CalculatePaymentFee(order.Total).ToString("N2", paypalAcceptedNumericFormatCulture)
                                }
                            },
                            Items = new ItemList
                            {
                                Items = items.ToArray(),
                                //ShippingAddress = shippingAddress
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
                    result.Result = paymentId;
                    result.IsSuccess = true;
                    return result;
                }
                await _orderProductService.ChangeOrderStateAsync(orderId, OrderState.PaymentFailed);
                result.AddError(responseBody);
                return result;
            }
        }

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponsePaypal> ExecutePaymentAsync(PaymentExecuteVm model)
        {
            var result = new ResponsePaypal
            {
                OrderId = model.OrderId
            };
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {
                result.AddError("User not Found");
                return result;
            }
            var orderRequest = await _orderProductService.GetOrderByIdAsync(model.OrderId);
            if (!orderRequest.IsSuccess)
            {
                result.AddError("Order not Found");
                return result;
            }
            var order = orderRequest.Result;
            var userId = _userManager.FindUserIdInClaims().Result;
            if (!userId.Equals(order.UserId))
            {
                result.AddError("This order was created by another person");
                return result;
            }

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(model.OrderId);
            if (isPayedRequest.IsSuccess)
            {
                result.AddError("Order was payed before, Check your orders");
                return result;
            }

            var wasInvoicedRequest = await _orderProductService.ItWasInTheStateAsync(model.OrderId, OrderState.Invoiced);
            if (!wasInvoicedRequest.Result)
            {
                result.AddError("Invoice was not created, before calling this api, you must create invoice");
                return result;
            }

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
                    PaymentMethodId = PaypalResources.PayPalProvider,
                    GatewayTransactionId = model.PaymentId,
                    PaymentStatus = PaymentStatus.Failed,
                    Total = order.Total,
                    UserId = userRequest.Result.Id,
                    FailureMessage = responseBody
                };

                if (response.IsSuccessStatusCode)
                {
                    await _orderProductService.ChangeOrderStateAsync(model.OrderId, OrderState.PaymentReceived);
                    payment.PaymentStatus = PaymentStatus.Succeeded;
                    await _paymentService.AddPaymentAsync(model.OrderId, payment);
                    result.Result = "Payment received";
                    result.IsSuccess = true;
                    return result;
                }

                await _orderProductService.ChangeOrderStateAsync(model.OrderId, OrderState.PaymentFailed);
                payment.PaymentStatus = PaymentStatus.Failed;
                await _paymentService.AddPaymentAsync(model.OrderId, payment);

                string errorName = responseObject.name;
                string errorDescription = responseObject.message;
                result.AddError($"{errorName} - {errorDescription}");
                return result;
            }
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> GetAccessTokenAsync()
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
        public virtual async Task<string> CreateExperienceProfileAsync(string accessToken)
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
        public virtual decimal CalculatePaymentFee(decimal total)
        {
            var percent = _payPalOptions.Value.PaymentFee;
            return total / 100 * percent;
        }
    }
}
