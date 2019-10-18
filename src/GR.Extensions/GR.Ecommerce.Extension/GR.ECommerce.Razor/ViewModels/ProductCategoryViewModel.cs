using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Razor.ViewModels
{
    public class ProductCategoryViewModel : Category
    {
        /// <summary>
        /// Category list
        /// </summary>
        public List<SelectListItem> ParentCategoryList { get; set; } = new List<SelectListItem>();

        [Display(Name = "Parent Category")]
        public override Guid? ParentCategoryId { get; set; }

        [Display(Name = "Display Order")]
        public override int DisplayOrder { get; set; }

        public string CategoryParentName { get; set; }
    }
}
