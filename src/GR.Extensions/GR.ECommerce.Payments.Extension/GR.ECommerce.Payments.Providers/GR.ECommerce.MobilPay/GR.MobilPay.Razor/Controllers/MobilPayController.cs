using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.MobilPay.Abstractions;
using GR.MobilPay.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        #endregion

        public MobilPayController(IOptions<MobilPayConfiguration> options, IMobilPayPaymentMethod paymentMethod, IPaymentService paymentService)
        {
            _options = options;
            _paymentMethod = paymentMethod;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Get invoice
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Dictionary<string, string>>))]
        public async Task<JsonResult> RequestInvoiceData(Guid? orderId)
        {
            var hostingUrl = Request.HttpContext.GetAppBaseUrl();
            var data = await _paymentMethod.RequestInvoicePaymentAsync(hostingUrl, orderId);
            return Json(data);
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(MobilPayConfiguration))]
        public JsonResult GetConfiguration() => Json(_options.Value);

        /// <summary>
        /// Confirm card
        /// </summary>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ConfirmCard()
        {
            Response.ContentType = "text/xml";
            string errorCode;
            string errorMessage;
            string errorType;
            if ((HttpContext.Request.Form.ContainsKey("data") == false
                    || HttpContext.Request.Form["data"] == "")
                    & (HttpContext.Request.Form.ContainsKey("env_key") == false
                    || HttpContext.Request.Form["env_key"] == ""))
            {

                errorType = "0x02";
                errorCode = "0x300000f5";
                errorMessage = "mobilpay.ro posted invalid parameters";
            }
            else
            {
                var textXml = HttpContext.Request.Form["data"].ToString();
                var envKey = HttpContext.Request.Form["env_key"].ToString();
                var result = await _paymentMethod.ConfirmPaymentAsync(textXml, envKey);

                errorType = result.ErrorType;
                errorCode = result.ErrorCode;
                errorMessage = result.ErrorMessage;
            }

            var message = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            if (errorCode == "0")
            {
                message = message + "<crc>" + errorMessage + "</crc>";
            }
            else
            {
                message = message + "<crc error_type=\"" + errorType + "\" error_code=\"" + errorCode + "\"> " + errorMessage + "</crc>";
            }

            await Response.WriteAsync(message);
            return Ok();
        }

        /// <summary>
        /// Confirm 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReturnCard(Guid? orderId)
        {
            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);

            if (isPayedRequest.IsSuccess)
                return RedirectToAction("Success", "Checkout", new
                {
                    OrderId = orderId
                });
            else
                return RedirectToAction("Fail", "Checkout", new
                {
                    OrderId = orderId
                });
        }
    }
}
