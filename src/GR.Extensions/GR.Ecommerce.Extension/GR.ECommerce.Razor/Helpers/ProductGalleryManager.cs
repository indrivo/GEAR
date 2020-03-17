using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.MimeTypes;
using GR.Core.Helpers.Responses;
using GR.Core.Razor.Extensions;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Models;
using GR.ECommerce.Razor.ViewModels.ProductsGalleryViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Razor.Helpers
{
    public sealed class ProductGalleryManager
    {
        #region Constants

        private const string URL_BASE = "/Products/GetImage";
        private const string DELETE_URL = "/Products/DeleteImage?imageId=";
        private const string DELETE_TYPE = "DELETE";

        #endregion

        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ICommerceContext _context;

        #endregion

        public ProductGalleryManager(ICommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get images for edit mode
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ResultModel<JsonFiles>> GetProductImagesOnEditModeAsync(Guid? productId)
        {
            if (productId == null) return new InvalidParametersResultModel<JsonFiles>();
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id.Equals(productId));
            if (product == null) return new NotFoundResultModel<JsonFiles>();
            var filesData = ParseResultFileList(product.ProductImages);
            return new SuccessResultModel<JsonFiles>(filesData);
        }

        /// <summary>
        /// Upload product images
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel<JsonFiles>> UploadProductImagesAsync(UploadImagesViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel<JsonFiles>();
            var images = MapAddImages(model.ProductId, model.Files).ToList();
            await _context.ProductImages.AddRangeAsync(images);
            var dbResult = await _context.PushAsync();
            if (!dbResult.IsSuccess) return dbResult.Map<JsonFiles>();
            return new SuccessResultModel<JsonFiles>(ParseResultFileList(images));
        }

        /// <summary>
        /// Add images on create async
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddImagesOnCreateAsync(Guid productId, IEnumerable<IFormFile> files)
        {
            var images = MapAddImages(productId, files);
            await _context.ProductImages.AddRangeAsync(images);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Get product images
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ResultModel<FilesViewModel>> GetProductImagesAsync(Guid? productId)
        {
            if (productId == null) return new InvalidParametersResultModel<FilesViewModel>();
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id.Equals(productId));
            if (product == null) return new NotFoundResultModel<FilesViewModel>();

            var filesData = ParseResultFileList(product.ProductImages);
            var model = new FilesViewModel
            {
                Files = filesData.Files
            };

            return new SuccessResultModel<FilesViewModel>(model);
        }


        /// <summary>
        /// Delete image by id
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteImageAsync(Guid? imageId)
        {
            if (imageId == null) return new InvalidParametersResultModel();
            var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id.Equals(imageId));
            if (image == null) return new NotFoundResultModel();
            _context.ProductImages.Remove(image);
            return await _context.PushAsync();
        }


        #region Helpers

        /// <summary>
        /// Map add images
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        private static IEnumerable<ProductImage> MapAddImages(Guid productId, IEnumerable<IFormFile> files)
        {
            var images = files.Select(x =>
            {
                var image = x.GetImageFromFormFile();
                var stream = x.GetMemoryStream();
                return new ProductImage
                {
                    Name = x.Name,
                    FileName = x.FileName,
                    ContentType = x.ContentType,
                    Height = image.Height,
                    Width = image.Width,
                    Image = stream.ToArray(),
                    ProductId = productId
                };
            }).ToList();

            return images;
        }


        /// <summary>
        /// Get file list
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        private static JsonFiles ParseResultFileList(IEnumerable<ProductImage> images)
        {
            var data = images.Select(UploadResult).ToList();
            var files = new JsonFiles(data);
            return files;
        }

        /// <summary>
        /// Upload result
        /// </summary>
        /// <param name="productImage"></param>
        /// <returns></returns>
        private static ViewDataUploadFilesResult UploadResult(ProductImage productImage)
        {
            var size = productImage.Image.Length;
            var fileName = productImage.FileName;
            var getType = MimeMapping.GetMimeMapping(fileName);
            var result = new ViewDataUploadFilesResult
            {
                Name = fileName,
                Size = size,
                Type = getType,
                Url = $"{URL_BASE}?imageId={productImage.Id}",
                DeleteUrl = DELETE_URL + productImage.Id,
                ThumbnailUrl = $"{URL_BASE}Thumb?imageId={productImage.Id}",
                DeleteType = DELETE_TYPE,
            };
            return result;
        }

        #endregion
    }
}
