using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Razor.Controllers
{
    public class ProductOptionController : CommerceBaseController<ProductOption, ProductOptionViewModel>
    {

        public ProductOptionController(ICommerceContext context) : base(context)
        {
        }

        // GET: /<controller>/
        public override IActionResult Index()
        {
            return View();
        }

        public override IActionResult Create()
        {
            return View();
        }

        /// <inheritdoc />
        /// <summary>
        /// Edit attribute
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public override async Task<IActionResult> Edit([Required] Guid? id)
        {
            if (id == null) return NotFound();
            var model = await Context.ProductOption.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();
            var result = model.Adapt<ProductOptionViewModel>();
            return View(result);
        }



        public override JsonResult OrderedList(DTParameters param)
        {
            var filtered = Context.ProductOption.GetPagedAsDtResultAsync(param).GetAwaiter().GetResult();

            var result = new DTResult<ProductOptionViewModel>
            {
                Draw = param.Draw,
                Data = filtered.Data.Adapt<List<ProductOptionViewModel>>(),
                RecordsFiltered = filtered.RecordsFiltered,
                RecordsTotal = filtered.RecordsTotal
            };
            return Json(result);
        }
    }
}