using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.ECommerce.Razor.Helpers.BaseControllers;

namespace GR.ECommerce.Razor.Controllers
{
    public class ProductCategoryController : CommerceBaseController<Category, ProductCategoryViewModel>
    {
        public ProductCategoryController(ICommerceContext context) : base(context)
        {
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        public override IActionResult Index()
        {
            return View();
        }

        /// <inheritdoc />
        /// <summary>
        /// Create category
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override IActionResult Create()
        {
            var result = new ProductCategoryViewModel();
            return View(AddDropdownItems(result));
        }

        /// <inheritdoc />
        /// <summary>
        /// Edit category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public override async Task<IActionResult> Edit([Required] Guid? id)
        {
            if (id == null) return NotFound();
            var model = await Context.Set<Category>().FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();
            var result = model.Adapt<ProductCategoryViewModel>();
            return View(AddDropdownItems(result));
        }

        /// <summary>
        /// Ajax list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override JsonResult OrderedList(DTParameters param)
        {
            var filtered = Context.Categories.GetPagedAsDtResultAsync(param).GetAwaiter().GetResult();

            var mapped = filtered.Data.Select(x =>
        {
            var listModel = x.Adapt<ProductCategoryViewModel>();
            listModel.CategoryParentName = Context.Categories
                .FirstOrDefault(y => y.Id == x.ParentCategoryId)?.Name;
            return listModel;
        }).ToList();

            var result = new DTResult<ProductCategoryViewModel>
            {
                Draw = param.Draw,
                Data = mapped,
                RecordsFiltered = filtered.RecordsFiltered,
                RecordsTotal = filtered.RecordsTotal
            };
            return Json(result);
        }

        /// <summary>
        /// Load data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProductCategoryViewModel AddDropdownItems(ProductCategoryViewModel model)
        {
            if (model == null) model = new ProductCategoryViewModel();
            model.ParentCategoryList.AddRange(Context.Categories.Where(x => x.IsDeleted == false && x.Id != model.Id).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }));

            return model;
        }
    }
}
