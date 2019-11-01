using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.CheckoutViewModels;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Razor.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject order service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        #endregion

        public CheckoutController(IUserManager<ApplicationUser> userManager, IOrderProductService<Order> orderProductService)
        {
            _userManager = userManager;
            _orderProductService = orderProductService;
        }

        /// <summary>
        /// Checkout page
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Shipping(Guid? orderId)
        {
            if (orderId == null) return NotFound();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return NotFound();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return NotFound();
            if (orderRequest.Result.OrderState != OrderState.New)
            {
                return NotFound();
            }
            var addressesRequest = await _userManager.GetUserAddressesAsync(userRequest.Result.Id.ToGuid());
            var model = new CheckoutShippingViewModel
            {
                Order = orderRequest.Result,
                Addresses = addressesRequest.Result?.ToList() ?? new List<Address>()
            };

            return View(model);
        }

        /// <summary>
        /// Set up shipment and billing address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Shipping([Required]CheckoutShippingViewModel model)
        {
            if (model == null) return NotFound();

            if (model.Order != null)
            {
                var addressUpdateRequest = await _orderProductService.SetOrderBillingAddressAndShipmentAsync(model.Order?.Id, model.ShipmentAddress, model.BillingAddressId);
                if (addressUpdateRequest.IsSuccess)
                    return RedirectToAction("Payment", new { OrderId = model.Order.Id });
                ModelState.AppendResultModelErrors(addressUpdateRequest.Errors);
            }

            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return NotFound();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(model.Order?.Id);
            if (!orderRequest.IsSuccess) return NotFound();
            if (orderRequest.Result.OrderState != OrderState.New) return NotFound();
            var addressesRequest = await _userManager.GetUserAddressesAsync(userRequest.Result.Id.ToGuid());
            model.Addresses = addressesRequest.Result;
            return View(model);
        }

        /// <summary>
        /// Make payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Payment(Guid? orderId)
        {
            return View();
        }
    }
}