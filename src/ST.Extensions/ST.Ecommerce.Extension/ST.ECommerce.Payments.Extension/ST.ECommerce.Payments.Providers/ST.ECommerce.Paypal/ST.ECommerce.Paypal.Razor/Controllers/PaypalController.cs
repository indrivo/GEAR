using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.ECommerce.Payments.Abstractions;
using ST.ECommerce.Paypal.Impl;

namespace ST.ECommerce.Paypal.Razor.Controllers
{
    public class PaypalController : Controller
    {
        private readonly IPaymentManager _paymentManager;

        public PaypalController()
        {
            _paymentManager = IoC.Resolve<IPaymentManager>(nameof(PaypalPaymentManager));
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
