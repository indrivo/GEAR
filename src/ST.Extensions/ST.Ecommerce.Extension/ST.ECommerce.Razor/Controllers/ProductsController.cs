using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST.Core;
using ST.Core.Abstractions;
using ST.ECommerce.Abstractions;
using ST.ECommerce.Abstractions.Models;
using ST.ECommerce.Razor.Helpers.BaseControllers;
using ST.ECommerce.Razor.ViewModels;

namespace ST.ECommerce.Razor.Controllers
{
    //[Route("commerce/[controller]")]
    public class ProductsController : CommerceBaseController<Product, ProductViewModel>
    {
        public ProductsController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
        {
        }

        public override IActionResult Index()
        {
            return View();
        }

        public override IActionResult Create()
        {
            var result = new ProductViewModel
            {
                Brands = new List<SelectListItem>(),
                ProductAttributesList = new Dictionary<string, IEnumerable<SelectListItem>>()

            };

            return View(AddDropdownItems(result));

        }

        [HttpPost]
        public override Task<IActionResult> Create(ProductViewModel model)
        {
            return base.Create(model);
        }

        [HttpPost]
        public IActionResult CreateAttribute(ProductViewModel model)
        {
            return RedirectToAction("Index");
        }
        public override JsonResult OrderedList(DTParameters param)
        {

            return default;
        }
        public ProductViewModel AddDropdownItems(ProductViewModel model)
        {
            model.Brands.AddRange(Context.Brands.Where(x => x.IsDeleted == false).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }));

            model.ProductAttributesList = Context.ProductAttribute.Include(x => x.AttributeGroup).GroupBy(x => x.AttributeGroup.Name)
                .ToDictionary(grouping => grouping.Key, x => x.ToList().Select(w => new SelectListItem
                    {
                        Text = w.Name,
                        Value = w.Id.ToString()
                    }));

            return model;
        }
    }
}