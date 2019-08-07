using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.ECommerce.Abstractions.Models;

namespace ST.ECommerce.Razor.ViewModels
{
    public class ProductAttributeViewModel : ProductAttribute
    {
        public List<SelectListItem> AttributeGroups { get; set; }

        [Display(Name = "Attribute Group")]
        public override Guid? AttributeGroupId { get; set; }
    }
}
