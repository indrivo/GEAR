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
        /// <summary>
        /// Brands
        /// </summary>
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Display name
        /// </summary>
        [Display(Name = "Display Name")]
        public override string DisplayName { get; set; }

        /// <summary>
        /// Short description
        /// </summary>
        [Display(Name = "Short Description")]
        public override string ShortDescription { get; set; }

        /// <summary>
        /// Attributes
        /// </summary>
        public Dictionary<string, IEnumerable<SelectListItem>> ProductAttributesList { get; set; } = new Dictionary<string, IEnumerable<SelectListItem>>();

        [Display(Name = "Available Attributes")]
        public int ProductAttributeId { get; set; }

        /// <summary>
        /// Category list
        /// </summary>
        public List<ProductCategoryDto> ProductCategoryList { get; set; } = new List<ProductCategoryDto>();


        /// <summary>
        /// Images
        /// </summary>
        [Display(Name = "Product Image")]
        public List<IFormFile> ProductImagesList { get; set; } = new List<IFormFile>();

        /// <summary>
        /// Product types
        /// </summary>
        [Display(Name = "Type")]
        public List<SelectListItem> ProductTypeList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Last price
        /// </summary>
        public virtual double Price { get; set; }
    }

    public class ProductCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
