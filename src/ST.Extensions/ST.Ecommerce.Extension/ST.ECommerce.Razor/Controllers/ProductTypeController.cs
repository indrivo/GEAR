using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ST.Core.Abstractions;
using ST.ECommerce.Abstractions;
using ST.ECommerce.Abstractions.Models;
using ST.ECommerce.Razor.Helpers.BaseControllers;

namespace ST.ECommerce.Razor.Controllers
{
    public class ProductTypeController : CommerceBaseController<ProductType, ProductType>
    {
        public ProductTypeController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
        {
        }

        public override IActionResult Index()
        {
            return View();
        }
    }
}
