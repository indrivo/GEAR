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
using GR.ECommerce.Abstractions.ViewModels.CartViewModels;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using GR.ECommerce.Razor.ViewModels;
using GR.Identity.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        #region Helpers

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        #endregion

        public CartController(ICommerceContext context, IDataFilter dataFilter, ICartService cartService) : base(context, dataFilter)
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
         
            return Json(result, SerializerSettings);
        }
       

        public async Task<JsonResult> DeleteCartItem([Required]Guid? cartItemId)
        {
            return Json(await _cartService.DeleteCartItemAsync(cartItemId));
        }


        public async Task<JsonResult> SetQuantity([Required] Guid? cartItemId, [Required] int? quantity)
        {
           
            return Json(await _cartService.SetQuantityAsync(cartItemId, quantity));
        }
    }
}
