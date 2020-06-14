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
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Helpers.BaseControllers;
using System.Drawing.Imaging;
using GR.Core.Razor.Extensions;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.ECommerce.Razor.Helpers;
using GR.ECommerce.Razor.Models;
using GR.ECommerce.Razor.ViewModels.ProductsGalleryViewModels;
using Microsoft.AspNetCore.Authorization;

namespace GR.ECommerce.Razor.Controllers
{
    [Authorize]
    public class ProductsController : CommerceBaseController<Product, ProductViewModel>
    {
        #region Injectable

        /// <summary>
        /// Inject product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        /// <summary>
        /// Inject gallery manager
        /// </summary>
        private readonly ProductGalleryManager _galleryManager;
        #endregion

        public ProductsController(ICommerceContext context, IDataFilter dataFilter, IProductService<Product> productService, ProductGalleryManager galleryManager) : base(context, dataFilter)
        {
            _productService = productService;
            _galleryManager = galleryManager;
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

        /// <summary>
        /// Store
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Store()
        {
            return View();
        }

        /// <summary>
        /// Dashboard
        /// </summary>
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ProductDetail(Guid? productId)
        {
            if (productId == null) return NotFound();

            var productBd = await Context.Products
                .Include(i => i.ProductPrices)
                .Include(i => i.ProductImages)
                .Include(i => i.ProductAttributes)
                .ThenInclude(i => i.ProductAttribute)
                .Include(i => i.ProductVariations)
                .ThenInclude(i => i.ProductVariationDetails)
                .ThenInclude(i => i.ProductOption)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (productBd is null) return NotFound();

            var result = productBd.Adapt<ProductViewModel>();
            result.ProductOption = GetProdOptionByVariation(result.Id);
            result.ProductVariationList = GetProdVariationList(result.Id);

            return View(result);
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

            await Context.Products.AddAsync(model);
            await Context.ProductPrices.AddAsync(new ProductPrice
            {
                Product = model,
                Price = model.Price
            });
            var dbResult = await Context.PushAsync();

            if (dbResult.IsSuccess)
            {
                if (model.ProductImagesList != null && model.ProductImagesList.Any())
                {
                    await _galleryManager.AddImagesOnCreateAsync(model.Id, model.ProductImagesList);
                }
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
            result.Price = result.PriceWithoutDiscount;
            result.ProductOption = new List<SelectListItem>();


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

            if (model.Price.Equals(dbModel.PriceWithoutDiscount).Negate())
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
        /// Get product images
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetProductImages(Guid? productId)
        {
            var imagesRequest = await _galleryManager.GetProductImagesOnEditModeAsync(productId);
            return Json(!imagesRequest.IsSuccess ? new JsonFiles(null) : imagesRequest.Result);
        }

        /// <summary>
        /// Get product image
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductImage(Guid? productId)
        {
            var photo = await _productService.Context.ProductImages.FirstOrDefaultAsync(x => x.ProductId.Equals(productId));
            if (photo?.Image == null)
            {
                var def = GetDefaultImage();
                if (def == null) return NotFound();
                return File(def, "image/png");
            }

            return File(photo.Image, photo.ContentType);
        }

        /// <summary>
        /// Get default image
        /// </summary>
        /// <returns></returns>
        private static byte[] GetDefaultImage()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Static/EmbeddedResources/no-image.png");
            if (!System.IO.File.Exists(path))
                return default;

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var binary = new BinaryReader(stream))
                {
                    var data = binary.ReadBytes((int)stream.Length);
                    return data;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return default;
        }

        /// <summary>
        /// Upload images
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadImages(UploadImagesViewModel model)
        {
            var result = await _galleryManager.UploadProductImagesAsync(model);
            return !result.IsSuccess ? Json("Error") : Json(result.Result);
        }

        /// <summary>
        /// Delete image by id
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(Guid? imageId)
        {
            await _galleryManager.DeleteImageAsync(imageId);
            return Json("OK");
        }

        /// <summary>
        /// Get user image
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult GetImage(Guid? imageId)
        {
            if (imageId == null) return NotFound();
            try
            {
                var photo = _productService.Context.ProductImages.SingleOrDefault(x => x.Id == imageId);
                if (photo?.Image != null) return File(photo.Image, photo.ContentType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return NotFound();
        }


        /// <summary>
        /// Get user image
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult GetImageThumb(Guid? imageId)
        {
            if (imageId == null) return NotFound();
            try
            {
                var photo = _productService.Context.ProductImages.SingleOrDefault(x => x.Id == imageId);
                if (photo?.Image != null)
                {
                    var resizedImage = photo.Image
                        .ToMemoryStream()
                        .GetImageFromStream()
                        .ResizeImage(80, 80)
                        .ToBytes(ImageFormat.Png);

                    return File(resizedImage, photo.ContentType);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return NotFound();
        }


        #region Helpers

        private List<SelectListItem> GetProdOptionByVariation(Guid productId)
        {
            return Context.ProductVariationDetails.Where(x => x.ProductVariation.ProductId == productId).Select(s => new SelectListItem
            {
                Text = s.ProductOption.Name,
                Value = s.ProductOptionId.ToString(),
            }).AsEnumerable().DistinctBy(d => d.Value).ToList();
        }

        private List<ProductVariation> GetProdVariationList(Guid productId)
        {

            var listVariation = Context.ProductVariations
                .Include(i => i.ProductVariationDetails)
                .ThenInclude(i => i.ProductOption)
                .Where(x => x.ProductId == productId).ToList();

            return listVariation;
        }

        /// <summary>
        /// Load dropdown items
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ProductViewModel GetDropdownItems(ProductViewModel model)
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
                    Value = w.Id.ToString(),
                }));

            model.ProductOption = Context.ProductOption.ToList().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
            }).ToList();


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

        #endregion

    }
}