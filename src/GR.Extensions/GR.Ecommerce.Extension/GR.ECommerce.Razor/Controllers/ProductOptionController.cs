using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using GR.ECommerce.Razor.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.ECommerce.Razor.Controllers
{
    public class ProductOptionController : CommerceBaseController<ProductOption, ProductOptionViewModel>
    {

        public ProductOptionController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
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
        public override async Task<IActionResult> Edit([Required]Guid? id)
        {
            if (id == null) return NotFound();
            var model = await Context.ProductOption.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();
            var result = model.Adapt<ProductOptionViewModel>();
            return View(result);
        }



        public override JsonResult OrderedList(DTParameters param)
        {
            var filtered = DataFilter.FilterAbstractEntity<ProductOption, ICommerceContext>(Context, param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).Select(x =>
            {
                var listModel = x.Adapt<ProductOptionViewModel>();
                return listModel;
            }).ToList();

            var result = new DTResult<ProductOptionViewModel>
            {
                Draw = param.Draw,
                Data = filtered,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return Json(result);
        }

      
    }
}
