using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Extensions;
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
        private readonly IUserManager<ApplicationUser> _userManager;

        public CartController(ICommerceContext context, IDataFilter dataFilter, IUserManager<ApplicationUser> userManager) : base(context, dataFilter)
        {
            _userManager = userManager;
        }
        // GET: /<controller>/
        public override IActionResult Index()
        {
            return View(GetCartItem());
        }


        [HttpPost]
        public async Task<JsonResult> AddToCard(AddToCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return Json(model);
            }

            var product = Context.Products.FirstOrDefault(x => x.Id == model.ProductId);
            var user = _userManager.GetCurrentUserAsync();

            if (product != null)
            {
                var cart = Context.Carts.FirstOrDefault(x => x.UserId == user.Result.Result.Id.ToGuid());

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = user.Result.Result.Id.ToGuid(),
                    };

                    await Context.Carts.AddAsync(cart);
                }

                var cartItem = Context.CartItems.FirstOrDefault(x => x.ProductId == model.ProductId && x.CartId == cart.Id && x.ProductVariationId == model.VariationId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        ProductId = model.ProductId,
                        Amount = model.Quantity,
                        CartId = cart.Id,
                    };

                    await Context.CartItems.AddAsync(cartItem);
                }
                else
                {
                    cartItem.Amount += model.Quantity; 
                    Context.CartItems.Update(cartItem);
                }

                 await Context.SaveChangesAsync();
            }
            return Json("");
        }


        public AddToCartViewModel GetCartItem()
        {
            var user = _userManager.GetCurrentUserAsync();
            var cartByUser = Context.Carts.Include(i => i.CartItems).ThenInclude(i=>i.Product).FirstOrDefault(x => x.UserId == user.Result.Result.Id.ToGuid());

            var result = cartByUser.Adapt<AddToCartViewModel>();
            return result;
        }
    }
}
