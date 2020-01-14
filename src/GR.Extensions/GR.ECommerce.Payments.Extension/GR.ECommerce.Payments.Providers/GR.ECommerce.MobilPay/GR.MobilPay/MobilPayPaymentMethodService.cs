using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.MobilPay.Abstractions;
using GR.MobilPay.Abstractions.Models;
using GR.MobilPay.Extensions;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.Extensions.Options;
using MobilpayEncryptDecrypt;

namespace GR.MobilPay
{
    public class MobilPayPaymentMethodService : IMobilPayPaymentMethod
    {
        /// <summary>
        /// Configuration
        /// </summary>
        private readonly MobilPayConfiguration _configuration;

        #region Injectable

        /// <summary>
        /// Inject user address service
        /// </summary>
        private readonly IUserAddressService _userAddressService;

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

        #endregion

        public MobilPayPaymentMethodService(IOrderProductService<Order> orderProductService,
            IPaymentService paymentService,
            IUserManager<GearUser> userManager,
            IOptionsSnapshot<MobilPayConfiguration> options,
            IUserAddressService userAddressService)
        {
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _userAddressService = userAddressService;
            _configuration = options.Value;
        }

        /// <summary>
        /// Request payment async
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Dictionary<string, string>>> RequestInvoicePaymentAsync(string hostingDomain, Guid? orderId)
        {
            var encryptRequest = await CreatePaymentAsync(hostingDomain, orderId);
            if (!encryptRequest.IsSuccess) return encryptRequest.Map(new Dictionary<string, string>());
            return encryptRequest.Map(new Dictionary<string, string>
            {
                { "data", encryptRequest.Result.EncryptedData },
                { "env_key", encryptRequest.Result.EnvelopeKey}
            });
        }

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel<MobilpayEncrypt>> CreatePaymentAsync(string hostingDomain, Guid? orderId)
        {
            var response = new ResultModel<MobilpayEncrypt>();
            var encrypt = new MobilpayEncrypt();
            var encDec = new MobilpayEncryptDecrypt.MobilpayEncryptDecrypt();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return response;

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess) return response;

            var order = orderRequest.Result;
            var addressRequest = await _userAddressService.GetAddressByIdAsync(order.BillingAddress);
            var address = addressRequest.IsSuccess ? addressRequest.Result : new Address();
            var user = await _userManager.UserManager.FindByIdAsync(order.UserId.ToString());

            try
            {
                var card = new Mobilpay_Payment_Request_Card();
                var invoice = new Mobilpay_Payment_Invoice();
                var billing = new Mobilpay_Payment_Address();
                var shipping = new Mobilpay_Payment_Address();
                var contactInfo = new Mobilpay_Payment_Request_Contact_Info();
                var url = new Mobilpay_Payment_Request_Url();

                var enc = new MobilpayEncryptDecrypt.MobilpayEncryptDecrypt();
                card.OrderId = order.Id.ToString();
                card.Type = "card";
                card.Signature = _configuration.Signature;
                url.ConfirmUrl = $"{hostingDomain}/MobilPay/ConfirmCard";
                url.ReturnUrl = $"{hostingDomain}/MobilPay/ReturnCard";
                card.Url = url;
                card.TimeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
                invoice.Amount = order.Total;
                invoice.Currency = order.Currency?.Code;
                invoice.Details = $"#{orderId}";

                billing.FirstName = user?.UserFirstName;
                billing.LastName = user?.UserLastName;
                billing.Email = user?.Email;
                billing.MobilPhone = address.Phone;
                billing.Address = address.AddressLine1;
                billing.ZipCode = address.ZipCode;
                billing.Country = address.Country?.Name;
                billing.City = address.StateOrProvince?.Name;

                contactInfo.Billing = billing;
                shipping.Sameasbilling = "1";
                contactInfo.Shipping = shipping;
                invoice.ContactInfo = contactInfo;
                card.Invoice = invoice;
                encrypt.Data = enc.GetXmlText(card);
                encrypt.X509CertificateFilePath = GetPathToCertificate();
                encDec.EncryptWithCng(encrypt);
                await _orderProductService.ChangeOrderStateAsync(orderId, OrderState.PendingPayment);
                response.IsSuccess = true;
                response.Result = encrypt;
                return response;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ErrorModel(string.Empty, ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Confirm payment
        /// </summary>
        /// <param name="textXml"></param>
        /// <param name="envKey"></param>
        /// <returns></returns>
        public async Task<MobilPayPaymentResponse> ConfirmPaymentAsync(string textXml, string envKey)
        {
            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.GetFullPath(Path.Combine(rootPath, _configuration.PathToPrivateKey));

            var result = new MobilPayPaymentResponse
            {
                ErrorCode = "0"
            };

            var encryptDecrypt = new MobilpayEncryptDecrypt.MobilpayEncryptDecrypt();
            var decrypt = new MobilpayDecrypt
            {
                Data = textXml,
                EnvelopeKey = envKey,
                PrivateKeyFilePath = filePath
            };

            encryptDecrypt.Decrypt(decrypt);
            var card = encryptDecrypt.GetCard(decrypt.DecryptedData);
            var orderId = card.OrderId.ToGuid();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {
                return new MobilPayPaymentResponse
                {
                    ErrorType = "0x02",
                    ErrorCode = "0x300000f6",
                    ErrorMessage = "mobilpay_refference_action paramaters is invalid"
                };
            }

            var order = orderRequest.Result;
            var payment = new Payment
            {
                PaymentMethodId = "MobilPay",
                GatewayTransactionId = orderId.ToString(),
                PaymentStatus = PaymentStatus.Failed,
                Total = order.Total,
                UserId = order.UserId,
                FailureMessage = card.Card.SerializeAsJson()
            };
            var orderState = order.OrderState;
            switch (card.Confirm.Action)
            {
                case "confirmed":
                case "paid":
                    {
                        result.ErrorMessage = card.Confirm.Crc;
                        if (card.Confirm.Action == "confirmed" && card.Confirm.Error.Code == "0")
                        {
                            payment.PaymentStatus = PaymentStatus.Succeeded;
                            orderState = OrderState.PaymentReceived;
                        }
                        break;
                    }
                default:
                    {
                        result.ErrorType = "0x02";
                        result.ErrorCode = "0x300000f6";
                        result.ErrorMessage = "mobilpay_refference_action paramaters is invalid";
                        orderState = OrderState.PaymentFailed;
                        break;
                    }
            }

            var addPaymentRequest = await _paymentService.AddPaymentAsync(orderId, payment);
            if (addPaymentRequest.IsSuccess)
            {
                await _orderProductService.ChangeOrderStateAsync(orderId, orderState);
            }

            return result;
        }


        /// <summary>
        /// Get path to certificate
        /// </summary>
        /// <returns></returns>
        private string GetPathToCertificate()
        {
            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathToCertificate = Path.GetFullPath(Path.Combine(rootPath, _configuration.PathToCertificate));
            return pathToCertificate;
        }
    }
}
