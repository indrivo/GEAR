using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GR.ApplePay.Abstractions;
using GR.ApplePay.Abstractions.Helpers;
using GR.ApplePay.Abstractions.Models;
using GR.ApplePay.Abstractions.ViewModels;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace GR.ApplePay
{
    /// <summary>
    /// This is an implementation of apple pay
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE)]
    public class ApplePayPaymentMethodService : IApplePayPaymentMethodService
    {
        #region Injectable

        /// <summary>
        /// Inject options
        /// </summary>
        private readonly ApplePaySettingsViewModel _options;

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

        public ApplePayPaymentMethodService(IWritableOptions<ApplePaySettingsViewModel> options, IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<GearUser> userManager, IUserAddressService userAddressService)
        {
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _userAddressService = userAddressService;
            _options = options.Value;
        }

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel<ApplePayTransactionInfo>> CreatePaymentAsync(Guid? orderId)
        {
            var result = new ResultModel<ApplePayTransactionInfo>();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {
                result.AddError("Order not found");
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
            var price = order.ProductOrders
                .Sum(item => item.AmountFinalPriceWithOutDiscount);
            var transactionResponse = new ApplePayTransactionInfo
            {
                ShopName = _options.StoreName,
                ProductPrice = price,
                ShopLocalization = new AppleShopLocalization
                {
                    CurrencyCode = order.Currency?.Code,
                    CountryCode = address?.CountryId
                }
            };

            await _orderProductService.ChangeOrderStateAsync(orderId, OrderState.PendingPayment);
            result.Result = transactionResponse;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Get merchant session
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<JObject>> GetMerchantSessionAsync(
            ValidateMerchantSessionModel model,
            HttpContext httpContext,
            CancellationToken cancellationToken = default)
        {
            var result = new ResultModel<JObject>();
            var modelState = ModelValidator.IsValid(model);
            if (!modelState.IsSuccess) return modelState.Map<JObject>();

            // You may wish to additionally validate that the URI specified for merchant validation in the
            // request body is a documented Apple Pay JS hostname. The IP addresses and DNS hostnames of
            // these servers are available here: https://developer.apple.com/documentation/applepayjs/setting_up_server_requirements
            if (string.IsNullOrWhiteSpace(model?.ValidationUrl) ||
                !Uri.TryCreate(model.ValidationUrl, UriKind.Absolute, out var requestUri))
            {
                result.AddError("Invalid Url");
                return result;
            }

            // Create the JSON payload to POST to the Apple Pay merchant validation URL.
            var request = new MerchantSessionRequest()
            {
                DisplayName = _options.StoreName,
                Initiative = "web",
                InitiativeContext = httpContext.Request.GetTypedHeaders().Host.Value,
                MerchantIdentifier = GetMerchantIdentifier(),
            };

            // POST the data to create a valid Apple Pay merchant session.
            var json = request.SerializeAsJson();

            var handler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12
            };
            var certificate = GetCertificate();
            handler.ClientCertificates.Add(certificate);
            var httpClient = new HttpClient(handler);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var response = await httpClient.PostAsync(requestUri, content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    result.AddError("Something went wrong");
                }
                else
                {
                    // Read the opaque merchant session JSON from the response body.
                    var stream = await response.Content.ReadAsStringAsync();
                    try
                    {
                        result.Result = JObject.Parse(stream);
                        result.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        result.AddError(e.Message);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Approve transaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> ApproveCompleteTransactionAsync(ApproveTransactionRequestViewModel model)
        {
            var modelState = ModelValidator.IsValid(model);
            if (!modelState.IsSuccess) return modelState;
            var result = new ResultModel();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(model.OrderId);
            if (!orderRequest.IsSuccess)
            {
                result.AddError("Order not found");
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

            var order = orderRequest.Result;
            var userId = _userManager.FindUserIdInClaims().Result;
            if (!userId.Equals(order.UserId))
            {
                result.AddError("This order was created by another person");
                return result;
            }

            var payment = new Payment
            {
                PaymentMethodId = ApplePayResources.ApplePayProvider,
                GatewayTransactionId = model.Token.TransactionIdentifier,
                FailureMessage = model.SerializeAsJson(),
                PaymentStatus = PaymentStatus.Succeeded,
                Total = order.Total,
                UserId = userId
            };

            await _orderProductService.ChangeOrderStateAsync(model.OrderId, OrderState.PaymentReceived);

            await _paymentService.AddPaymentAsync(model.OrderId, payment);
            result.Result = "Payment received";
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Get certificate
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 GetCertificate()
        {
            // Get the merchant certificate for two-way TLS authentication with the Apple Pay server.
            return _options.UseCertificateStore ? LoadCertificateFromStore() : LoadCertificateFromDisk();
        }

        /// <summary>
        /// Get merchant identifier
        /// </summary>
        /// <returns></returns>
        public string GetMerchantIdentifier()
        {
            try
            {
                var merchantCertificate = GetCertificate();
                return GetMerchantIdentifier(merchantCertificate);
            }
            catch (InvalidOperationException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get association domain file
        /// </summary>
        /// <returns></returns>
        public ResultModel<byte[]> GetAssociationDomainFile()
        {
            var result = new ResultModel<byte[]>();
            var path = "DomainAssociation/apple-developer-merchantid-domain-association.txt";
            var associationFile = Path.Combine(AppContext.BaseDirectory, path);
            if (!File.Exists(associationFile))
            {
                result.AddError("File not found");
                return result;
            }
            using (var stream = new FileStream(associationFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var binary = new BinaryReader(stream))
            {
                try
                {
                    var data = binary.ReadBytes((int)stream.Length);
                    result.IsSuccess = true;
                    result.Result = data;
                }
                catch (Exception e)
                {
                    result.AddError(e.Message);
                }
            }

            return result;
        }

        #region Helpers

        private static string GetMerchantIdentifier(X509Certificate2 certificate)
        {
            // This OID returns the ASN.1 encoded merchant identifier
            var extension = certificate.Extensions["1.2.840.113635.100.6.32"];

            return extension == null ? string.Empty : Encoding.ASCII.GetString(extension.RawData).Substring(2);

            // Convert the raw ASN.1 data to a string containing the ID
        }

        /// <summary>
        /// Load certificate from local
        /// </summary>
        /// <returns></returns>
        private X509Certificate2 LoadCertificateFromDisk()
        {
            var rootPath = Path.Combine(AppContext.BaseDirectory, "Certificates");
            var certificatePath = Path.Combine(rootPath, _options.MerchantCertificateFileName);
            if (!File.Exists(certificatePath))
            {
                throw new InvalidOperationException("Missing Apple merchant certificate configuration");
            }

            try
            {
                return new X509Certificate2(
                    certificatePath,
                    _options.MerchantCertificatePassword);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load Apple Pay merchant certificate file from '{_options.MerchantCertificateFileName}'.", ex);
            }
        }

        private X509Certificate2 LoadCertificateFromStore()
        {
            // Load the certificate from the current user's certificate store. This
            // is useful if you do not want to publish the merchant certificate with
            // your application, but it is also required to be able to use an X.509
            // certificate with a private key if the user profile is not available,
            // such as when using IIS hosting in an environment such as Microsoft Azure.
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                _options.MerchantCertificateThumbprint.Trim(),
                validOnly: false);

            if (certificates.Count < 1)
            {
                throw new InvalidOperationException(
                    $"Could not find Apple Pay merchant certificate with thumbprint '{_options.MerchantCertificateThumbprint}' from store '{store.Name}' in location '{store.Location}'.");
            }

            store.Close();
            return certificates[0];
        }

        #endregion
    }
}