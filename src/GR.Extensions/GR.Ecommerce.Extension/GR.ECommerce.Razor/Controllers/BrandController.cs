﻿using Microsoft.AspNetCore.Mvc;
using GR.Core.Abstractions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using Microsoft.AspNetCore.Authorization;

namespace GR.ECommerce.Razor.Controllers
{
    [Authorize]
    public class BrandController : CommerceBaseController<Brand, Brand>
    {
        public BrandController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        public override IActionResult Index()
        {
            return View();
        }
    }
}
