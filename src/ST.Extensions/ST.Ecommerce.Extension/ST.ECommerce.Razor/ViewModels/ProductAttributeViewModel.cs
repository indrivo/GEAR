using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.ECommerce.Abstractions.Models;

namespace ST.ECommerce.Razor.ViewModels
{
    public class ProductAttributeViewModel : ProductAttribute
    {
        /// <summary>
        /// Groups
        /// </summary>
        public List<SelectListItem> AttributeGroups { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Group
        /// </summary>
        [Display(Name = "Attribute Group")]
        public override Guid? AttributeGroupId { get; set; }
    }
}
