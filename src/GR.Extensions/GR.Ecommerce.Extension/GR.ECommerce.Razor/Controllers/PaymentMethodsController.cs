using System.Threading.Tasks;
using GR.Core;
using GR.ECommerce.Payments.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class PaymentMethodsController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        #endregion

        public PaymentMethodsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Payment providers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var methodsRequest = await _paymentService.GetAllPaymentMethodsAsync();
            if (!methodsRequest.IsSuccess) return NotFound();
            return View(methodsRequest.Result);
        }
    }
}