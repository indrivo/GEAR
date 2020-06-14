using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Razor.BaseControllers;
using GR.ECommerce.Payments.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Profile.Abstractions;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using GR.Orders.Razor.ViewModels.OrderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Orders.Razor.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    public class OrdersController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject order product service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Inject user address
        /// </summary>
        private readonly IUserAddressService _userAddressService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderProductService"></param>
        /// <param name="paymentService"></param>
        /// <param name="userAddressService"></param>
        public OrdersController(IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserAddressService userAddressService)
        {
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userAddressService = userAddressService;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MyOrders() => View();

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public IActionResult AllOrders() => View();

        /// <summary>
        /// Get order details
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(Guid? orderId)
        {
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return NotFound();
            var order = orderRequest.Result;
            var paymentsRequest = await _paymentService.GetPaymentsForOrderAsync(orderId);
            var billingAddressRequest = await _userAddressService.GetAddressByIdAsync(order.BillingAddress);
            var shippingAddressRequest = await _userAddressService.GetAddressByIdAsync(order.ShipmentAddress);
            return View(new OrderViewModel
            {
                Order = orderRequest.Result,
                Payments = paymentsRequest.Result?.ToList(),
                BillingAddress = billingAddressRequest.Result,
                ShippingAddress = shippingAddressRequest.Result
            });
        }
    }
}