using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using GR.ECommerce.Razor.ViewModels;

namespace GR.ECommerce.Razor.Controllers
{
    public class ProductsController : CommerceBaseController<Product, ProductViewModel>
    {
        public ProductsController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override IActionResult Index()
        {
            return View();
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override IActionResult Create()
        {
            var result = new ProductViewModel();
            return View(GetDropdownItems(result));
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return View(model);
            }

            if (model.ProductImagesList != null && model.ProductImagesList.Any())
            {
                model.ProductImages = model.ProductImagesList.Select(async x =>
                {
                    var stream = new MemoryStream();
                    await x.CopyToAsync(stream);
                    return stream;
                }).Select(x => x.Result).Select(x => x.ToArray()).Select(x => new ProductImage
                {
                    Image = x,
                    ProductId = model.Id
                }).ToList();
            }

            await Context.Products.AddAsync(model);
            await Context.ProductPrices.AddAsync(new ProductPrice
            {
                Product = model,
                Price = model.Price
            });
            var dbResult = await Context.PushAsync();

            if (dbResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AppendResultModelErrors(dbResult.Errors);

            return View(model);
        }

        /// <inheritdoc />
        /// <summary>
        /// Edit product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public override async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var model = await Context.Products
                .Include(x => x.ProductPrices)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();

            var result = model.Adapt<ProductViewModel>();

            result.Brands = new List<SelectListItem>();
            result.ProductAttributesList = new Dictionary<string, IEnumerable<SelectListItem>>();
            result.ProductCategoryList = new List<ProductCategoryDto>();
            result.ProductTypeList = new List<SelectListItem>();
            result.Price = result.CurrentPrice;

            return View(GetDropdownItems(result));
        }

        /// <inheritdoc />
        /// <summary>
        /// Update product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid || model.Id.Equals(Guid.Empty))
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return View(model);
            }

            var dbModel = await Context.Products
                .Include(x => x.ProductPrices)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(model.Id));

            dbModel.Name = model.Name;
            dbModel.DisplayName = model.DisplayName;
            dbModel.Description = model.Description;
            dbModel.IsPublished = model.IsPublished;
            dbModel.ShortDescription = model.ShortDescription;
            dbModel.Specification = model.Specification;
            dbModel.BrandId = model.BrandId;
            dbModel.ProductTypeId = model.ProductTypeId;

            if (model.ProductAttributesList != null && model.ProductImagesList.Any())
            {
                dbModel.ProductImages = model.ProductImagesList.Select(async x =>
                {
                    var stream = new MemoryStream();
                    await x.CopyToAsync(stream);
                    return stream;
                }).Select(x => x.Result).Select(x => x.ToArray()).Select(x => new ProductImage
                {
                    Image = x,
                    ProductId = model.Id
                }).ToList();
            }

            if (model.Price.AreEqual(dbModel.CurrentPrice).Negate())
            {
                Context.ProductPrices.Add(new ProductPrice
                {
                    Price = model.Price,
                    ProductId = model.Id
                });
            }

            Context.Products.Update(dbModel);
            var dbResult = await Context.PushAsync();

            if (dbResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AppendResultModelErrors(dbResult.Errors);

            return View(model);
        }

        /// <summary>
        /// Load dropdown items
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProductViewModel GetDropdownItems(ProductViewModel model)
        {
            model.Brands.AddRange(Context.Brands.Where(x => x.IsDeleted == false).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }));

            model.ProductAttributesList = Context.ProductAttribute.Include(x => x.AttributeGroup)
                .GroupBy(x => x.AttributeGroup.Name)
                .ToDictionary(grouping => grouping.Key, x => x.ToList().Select(w => new SelectListItem
                {
                    Text = w.Name,
                    Value = w.Id.ToString()
                }));

            model.ProductCategoryList = Context.Categories.Select(x => new ProductCategoryDto
            {
                Name = x.Name,
                CategoryId = x.Id,
                IsChecked = Context.ProductCategories.Any(
                    w => w.ProductId.Equals(model.Id) && w.CategoryId.Equals(x.Id))
            }).ToList();

            model.ProductTypeList.AddRange(Context.ProductTypes.Where(x => x.IsDeleted == false).Select(x =>
                new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }));
            return model;
        }


        [HttpPost]
        public async Task<JsonResult> EditProductAttributes([FromBody] IEnumerable<ProductAttributesViewModel> model)
        {
            foreach (var item in model)
            {
                var attribute = Context.ProductAttributes
                    .FirstOrDefault(x =>
                        x.ProductAttributeId == item.ProductAttributeId && x.ProductId == item.ProductId);

                if (attribute != null)
                {
                    Context.ProductAttributes.Remove(attribute);
                }

                Context.ProductAttributes.Add(item);
            }

            var dbResult = await Context.PushAsync();
            return Json(dbResult);
        }


        [HttpGet]
        public JsonResult GetProductAttributes(string productId) => Json(Context.ProductAttributes
            .Include(x => x.ProductAttribute)
            .Where(x => x.ProductId == productId.ToGuid())
            .Select(x => new
            {
                AttributeId = x.ProductAttributeId,
                Label = x.ProductAttribute.Name,
                x.Value,
                x.IsAvailable,
                x.IsPublished
            }));

        /// <summary>
        /// Remove attribute
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveAttribute(Guid productId, Guid attributeId)
        {
            var resultModel = new ResultModel();
            var result = Context.ProductAttributes
                .FirstOrDefault(x => x.ProductAttributeId == attributeId && x.ProductId == productId);

            if (result == null) return Json(resultModel);
            Context.ProductAttributes.Remove(result);
            var dbResult = await Context.PushAsync();
            return Json(dbResult);
        }

        /// <summary>
        /// Edit map product categories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> EditProductCategories([FromBody] IEnumerable<ProductCategoriesViewModel> model)
        {
            foreach (var item in model)
            {
                if (Context.ProductCategories.Any(x =>
                    x.CategoryId == item.CategoryId && x.ProductId == item.ProductId))
                {
                    if (!item.Checked)
                    {
                        Context.ProductCategories.Remove(item);
                    }
                }
                else
                {
                    if (item.Checked)
                    {
                        Context.ProductCategories.Add(item);
                    }
                }
            }

            var dbResult = await Context.PushAsync();
            return Json(dbResult);
        }

        [HttpGet]
        public JsonResult GetProductCategories(Guid productId)
        {
            return Json(Context.ProductCategories.Where(x => x.ProductId == productId));
        }
    }
}