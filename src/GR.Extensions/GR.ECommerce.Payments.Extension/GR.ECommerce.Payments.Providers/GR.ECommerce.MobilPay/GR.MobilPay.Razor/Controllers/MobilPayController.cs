using System;
using System.IO;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.MobilPay.Abstractions;
using GR.MobilPay.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GR.MobilPay.Razor.Controllers
{
    [Authorize]
    public class MobilPayController : Controller
    {
        #region  Injectable

        /// <summary>
        /// Inject options
        /// </summary>
        private readonly IOptions<MobilPayConfiguration> _options;

        /// <summary>
        /// Inject payment method
        /// </summary>
        private readonly IMobilPayPaymentMethod _paymentMethod;

        #endregion

        public MobilPayController(IOptions<MobilPayConfiguration> options, IMobilPayPaymentMethod paymentMethod)
        {
            _options = options;
            _paymentMethod = paymentMethod;
        }


        /// <summary>
        /// Get invoice
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task RequestInvoice(Guid? orderId)
        {
            var hostingUrl = Request.HttpContext.GetAppBaseUrl();
            await _paymentMethod.RequestInvoicePaymentAsync(hostingUrl, orderId);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCard(string value)
        {
            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var keypath = Path.Combine(rootPath, _options.Value.PathToPrivateKey);
            return Ok();
        }

        [HttpGet]
        public IActionResult ReturnCard()
        {
            return Ok();
        }
    }
}
