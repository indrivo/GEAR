using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using GR.ECommerce.Razor.ViewModels;
using GR.Identity.Abstractions;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.ECommerce.Razor.Controllers
{
    public class CartController  : CommerceBaseController<Cart, CartViewModel>
    {
        private IUserManager<ApplicationUser> _userManager;

        public CartController(ICommerceContext context, IDataFilter dataFilter, IUserManager<ApplicationUser> userManager) : base(context, dataFilter)
        {
            _userManager = userManager;
        }
        // GET: /<controller>/
        public override IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> AddToCard(CartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return Json(model);
            }

            var product = Context.Products.FirstOrDefault(x => x.Id == model.ProductId);

            if (product != null)
            {
                // var card = Context
            }


            return Json("");
        }

    }
}
