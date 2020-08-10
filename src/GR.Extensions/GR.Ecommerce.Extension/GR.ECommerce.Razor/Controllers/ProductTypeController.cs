using Microsoft.AspNetCore.Mvc;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Helpers.BaseControllers;

namespace GR.ECommerce.Razor.Controllers
{
    public class ProductTypeController : CommerceBaseController<ProductType, ProductType>
    {
        public ProductTypeController(ICommerceContext context) : base(context)
        {
        }

        public override IActionResult Index()
        {
            return View();
        }
    }
}
