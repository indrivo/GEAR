using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Abstractions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using GR.ECommerce.Razor.ViewModels;

namespace GR.ECommerce.Razor.Controllers
{
    public class ProductCategoryController : CommerceBaseController<Category, ProductCategoryViewModel>
    {
        public ProductCategoryController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
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
        public override async Task<IActionResult> Edit([Required]Guid? id)
        {
            if (id == null) return NotFound();
            var model = await Context.SetEntity<Category>().FirstOrDefaultAsync(x => x.Id == id);
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
            var filtered = DataFilter.FilterAbstractEntity<Category, ICommerceContext>(Context, param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).Select(x =>
            {
                var listModel = x.Adapt<ProductCategoryViewModel>();
                listModel.CategoryParentName = Context.Categories
                    .FirstOrDefault(y => y.Id == x.ParentCategoryId)?.Name;
                return listModel;
            }).ToList();

            var result = new DTResult<ProductCategoryViewModel>
            {
                Draw = param.Draw,
                Data = filtered,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
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
