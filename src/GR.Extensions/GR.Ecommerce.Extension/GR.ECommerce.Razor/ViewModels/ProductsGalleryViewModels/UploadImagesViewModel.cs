using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GR.ECommerce.Razor.ViewModels.ProductsGalleryViewModels
{
    public sealed class UploadImagesViewModel
    {
        public ICollection<IFormFile> Files { get; set; } = new List<IFormFile>();
        public Guid ProductId { get; set; }
    }
}
