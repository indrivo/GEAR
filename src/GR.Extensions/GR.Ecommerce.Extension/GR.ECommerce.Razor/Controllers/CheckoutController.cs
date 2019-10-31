using System;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Razor.Controllers
{
    public class CheckoutController : Controller
    {
        /// <summary>
        /// Checkout page
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IActionResult Index(Guid? orderId)
        {
            if (orderId == null) return NotFound();


            return View();
        }
    }
}