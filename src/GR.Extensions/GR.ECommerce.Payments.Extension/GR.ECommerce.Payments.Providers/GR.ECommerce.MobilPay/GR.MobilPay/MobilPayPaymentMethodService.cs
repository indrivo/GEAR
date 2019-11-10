using System;
using System.IO;
using System.Threading.Tasks;
using GR.ECommerce.Payments.Abstractions;
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

        public MobilPayPaymentMethodService(IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<ApplicationUser> userManager, IOptions<MobilPayConfiguration> options)
        {
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _configuration = options.Value;
        }


        public async Task<MobilpayEncrypt> CreatePaymentAsync(string hostingDomain, Guid? orderId)
        {
            var encrypt = new MobilpayEncrypt();
            var encDecr = new MobilpayEncryptDecrypt.MobilpayEncryptDecrypt();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {
                return encrypt;
            }

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess)
            {
                return encrypt;
            }

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
                encDecr.EncryptWithCng(encrypt);

                return encrypt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return encrypt;
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
