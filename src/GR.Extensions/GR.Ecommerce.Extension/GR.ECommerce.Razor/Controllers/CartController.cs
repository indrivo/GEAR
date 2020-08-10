using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.CartViewModels;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Razor.Controllers
{
    [Authorize]
    public class CartController : CommerceBaseController<Cart, AddToCartViewModel>
    {
        #region Injectable

        /// <summary>
        /// inserc cart service
        /// </summary>
        private readonly ICartService _cartService;

        #endregion

        public CartController(ICommerceContext context, ICartService cartService) : base(context)
        {
            _cartService = cartService;
        }
        // GET: /<controller>/
        public override IActionResult Index()
        {

            var result = _cartService.GetCartByUserAsync().Result;
            return View(result.Result);
        }

        [HttpPost]
        public async Task<JsonResult> AddToCard(AddToCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return Json(model);
            }
            var result = await _cartService.AddToCardAsync(model);

            return Json(result);
        }

        public async Task<JsonResult> DeleteCartItem([Required] Guid? cartItemId)
        {
            return Json(await _cartService.DeleteCartItemAsync(cartItemId));
        }

        public async Task<JsonResult> SetQuantity([Required] Guid? cartItemId, [Required] int? quantity)
        {

            return Json(await _cartService.SetQuantityAsync(cartItemId, quantity));
        }
    }
}
