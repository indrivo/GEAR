using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Razor.Extensions;
using GR.Core.Razor.Helpers;
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
        private const string URL_BASE = "/Products/GetImage";
        private const string DELETE_URL = "/Products/DeleteImage/?imageId=";
        private const string DELETE_TYPE = "GET";

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
        /// Add images on create async
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddImagesOnCreateAsync(Guid productId, IEnumerable<IFormFile> files)
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
            await _context.ProductImages.AddRangeAsync(images);
            return await _context.PushAsync();
        }

        public async Task<ResultModel<FilesViewModel>> GetProductImagesAsync(Guid? productId)
        {
            if (productId == null) return new InvalidParametersResultModel<FilesViewModel>();
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id.Equals(productId));
            if (product == null) return new NotFoundResultModel<FilesViewModel>();

            var filesData = GetFileList(product.ProductImages);
            var model = new FilesViewModel
            {
                Files = filesData.Files
            };

            return new SuccessResultModel<FilesViewModel>(model);
        }


        #region Helpers

        private JsonFiles GetFileList(IEnumerable<ProductImage> images)
        {
            var r = new List<ViewDataUploadFilesResult>();
            foreach (var image in images)
            {
                r.Add(UploadResult(image));
            }
            var files = new JsonFiles(r);

            return files;
        }

        /// <summary>
        /// Upload result
        /// </summary>
        /// <param name="productImage"></param>
        /// <returns></returns>
        public ViewDataUploadFilesResult UploadResult(ProductImage productImage)
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
                ThumbnailUrl = CheckThumb(getType, fileName),
                DeleteType = DELETE_TYPE,
            };
            return result;
        }

        private static string CheckThumb(string type, string fileName)
        {
            var splited = type.Split('/');
            if (splited.Length == 2)
            {
                var extension = splited[1].ToLower();
                if (extension.Equals("jpeg") || extension.Equals("jpg") || extension.Equals("png") || extension.Equals("gif"))
                {
                    var thumbnailUrl = URL_BASE + "thumbs/" + Path.GetFileNameWithoutExtension(fileName) + $"80x80{Path.GetExtension(fileName)}";
                    return thumbnailUrl;
                }
                else
                {
                    if (extension.Equals("octet-stream")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/exe.png";

                    }
                    if (extension.Contains("zip")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/zip.png";
                    }
                    var thumbnailUrl = "/Content/Free-file-icons/48px/" + extension + ".png";
                    return thumbnailUrl;
                }
            }
            else
            {
                return URL_BASE + "/thumbs/" + Path.GetFileNameWithoutExtension(fileName) + "80x80.jpg";
            }

        }

        #endregion
    }
}
