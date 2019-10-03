using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ST.Core;
using ST.Core.Abstractions;
using ST.ECommerce.Abstractions;
using ST.ECommerce.Abstractions.Models;
using ST.ECommerce.Razor.Helpers.BaseControllers;
using ST.ECommerce.Razor.ViewModels;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ST.ECommerce.Razor.Controllers
{
    public class ProductAttributeController : CommerceBaseController<ProductAttribute, ProductAttributeViewModel>
    {
        public ProductAttributeController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
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
        /// Create view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override IActionResult Create()
        {
            var result = new ProductAttributeViewModel
            {
                AttributeGroups = new List<SelectListItem>()
            };

            return View(AddDropdownItems(result));
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
            var model = await Context.ProductAttribute.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();
            var result = model.Adapt<ProductAttributeViewModel>();
            return View(AddDropdownItems(result));
        }

        /// <inheritdoc />
        /// <summary>
        /// List
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override JsonResult OrderedList(DTParameters param)
        {
            var filtered = DataFilter.FilterAbstractEntity<ProductAttribute, ICommerceContext>(Context, param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).Select(x =>
                {
                    var listModel = x.Adapt<ProductAttributeListViewModel>();
                    listModel.AttributeGroupName = Context.AttributeGroups
                    .FirstOrDefault(y => y.Id == x.AttributeGroupId)?.Name;
                    return listModel;
                }).ToList();

            var result = new DTResult<ProductAttributeListViewModel>
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
        public ProductAttributeViewModel AddDropdownItems(ProductAttributeViewModel model)
        {
            if (model == null) model = new ProductAttributeViewModel();
            model.AttributeGroups.AddRange(Context.AttributeGroups.Where(x => x.IsDeleted == false).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }));
            return model;
        }
    }
}
