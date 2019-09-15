using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.ECommerce.Abstractions.Models;

namespace ST.ECommerce.Razor.ViewModels
{
    public class ProductViewModel : Product
    {
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();

        [Display(Name = "Display Name")]
        public override string DisplayName { get; set; }

        [Display(Name = "Short Description")]
        public override string ShortDescription { get; set; }

        public Dictionary<string,IEnumerable<SelectListItem>> ProductAttributesList { get; set; } = new Dictionary<string, IEnumerable<SelectListItem>>();

        [Display(Name = "Available Attributes")]
        public int ProductAttributeId { get; set; }

        public List<ProductCategoryDto> ProductCategoryList { get; set; } = new List<ProductCategoryDto>();


        [Display(Name = "Product Image")]
        public List<IFormFile> ProductImagesList { get; set; } = new List<IFormFile>();

        [Display(Name = "Type")]
        public List<SelectListItem> ProductTypeList { get; set; } = new List<SelectListItem>();
    }

    public class ProductCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
