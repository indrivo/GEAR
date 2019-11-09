using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Abstractions;
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
        private readonly string _yourCurrency = "RON";

        private readonly MobilPayConfiguration _configuration;

        #region Injectable

        /// <summary>
        /// Inject http client
        /// </summary>
        private readonly HttpClient _httpClient;

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


        public MobilPayPaymentMethodService(IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<ApplicationUser> userManager, IOptions<MobilPayConfiguration> options, HttpClient httpClient)
        {
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _httpClient = httpClient;
            _configuration = options.Value;
        }

        /// <summary>
        /// Request payment async
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task RequestInvoicePaymentAsync(string hostingDomain, Guid? orderId)
        {
            var encrypt = await CreatePaymentAsync(hostingDomain, orderId);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", encrypt.EncryptedData),
                new KeyValuePair<string, string>("env_key", encrypt.EnvelopeKey)
            });

            await _httpClient.PostAsync(_configuration.MobilPayUrl, content);
        }

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="hostingDomain"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<MobilpayEncrypt> CreatePaymentAsync(string hostingDomain, Guid? orderId)
        {
            var encrypt = new MobilpayEncrypt();
            var encDec = new MobilpayEncryptDecrypt.MobilpayEncryptDecrypt();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return encrypt;

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess) return encrypt;

            var order = orderRequest.Result;

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
                invoice.Amount = 1;
                invoice.Currency = _yourCurrency;
                invoice.Details = "Product name";

                contactInfo.Billing = billing;
                shipping.Sameasbilling = "1";
                contactInfo.Shipping = shipping;
                invoice.ContactInfo = contactInfo;
                card.Invoice = invoice;
                encrypt.Data = enc.GetXmlText(card);
                encrypt.X509CertificateFilePath = GetPathToCertificate();
                encDec.EncryptWithCng(encrypt);
                await _orderProductService.ChangeOrderStateAsync(orderId, OrderState.PendingPayment);
                return encrypt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return encrypt;
        }


        /// <summary>
        /// Confirm payment
        /// </summary>
        /// <param name="textXml"></param>
        /// <param name="envKey"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<MobilPayPaymentResponse> ConfirmPaymentAsync(string textXml, string envKey, Guid? orderId)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {

            }

            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {

            }

            var order = orderRequest.Result;

            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.GetFullPath(Path.Combine(rootPath, _configuration.PathToPrivateKey));

            var result = new MobilPayPaymentResponse();

            var encryptDecrypt = new MobilpayEncryptDecrypt.MobilpayEncryptDecrypt();
            var decrypt = new MobilpayDecrypt
            {
                Data = textXml,
                EnvelopeKey = envKey,
                PrivateKeyFilePath = filePath
            };

            encryptDecrypt.Decrypt(decrypt);
            var card = encryptDecrypt.GetCard(decrypt.DecryptedData);
            var payment = new Payment
            {
                PaymentMethodId = "MobilPay",
                GatewayTransactionId = "",
                PaymentStatus = PaymentStatus.Failed,
                Total = order.Total,
                UserId = userRequest.Result.Id.ToGuid()
            };

            switch (card.Confirm.Action)
            {
                case "confirmed":
                case "paid":
                    {
                        var paidAmount = card.Confirm.Original_Amount;
                        result.ErrorMessage = card.Confirm.Crc;
                        if (card.Confirm.Action == "confirmed" && card.Confirm.Error.Code == "0")
                        {
                            payment.PaymentStatus = PaymentStatus.Succeeded;
                        }
                        break;
                    }
                default:
                    {
                        result.ErrorType = "0x02";
                        result.ErrorCode = "0x300000f6";
                        result.ErrorMessage = "mobilpay_refference_action paramaters is invalid";
                        break;
                    }
            }

            await _paymentService.AddPaymentAsync(orderId, payment);

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
