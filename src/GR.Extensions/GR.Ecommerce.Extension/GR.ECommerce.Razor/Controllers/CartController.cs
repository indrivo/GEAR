using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using GR.ECommerce.Razor.ViewModels;
using GR.Identity.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.ECommerce.Razor.Controllers
{
    public class CartController  : CommerceBaseController<Cart, AddToCartViewModel>
    {
        #region Injectable

        /// <summary>
        /// inserc cart service
        /// </summary>
        private readonly ICartService _cartService;

        #endregion

        public CartController(ICommerceContext context, IDataFilter dataFilter, ICartService cartService) : base(context, dataFilter)
        {
            _cartService = cartService;
        }
        // GET: /<controller>/
        public override IActionResult Index()
        {

            var cart = _cartService.GetCartByUser().Result;
            var result = cart.Result.Adapt<AddToCartViewModel>();

            return View(result);
        }

        [HttpPost]
        public async Task<JsonResult> AddToCard(AddToCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return Json(model);
            }
            var result = _cartService.AddToCard(model).Result;
         
            return Json(result);
        }
       

        public async Task<JsonResult> DeleteCartItem([Required]Guid? cartItemId)
        {
            return Json(_cartService.DeleteCartItem(cartItemId).Result);
        }


        public async Task<JsonResult> SetQuantity([Required] Guid? cartItemId, [Required] int? quantity)
        {
           
            return Json(_cartService.SetQuantity(cartItemId, quantity).Result);
        }
    }
}
