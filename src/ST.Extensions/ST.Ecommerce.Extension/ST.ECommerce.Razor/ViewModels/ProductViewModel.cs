using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.ECommerce.Abstractions.Models;

namespace ST.ECommerce.Razor.ViewModels
{
    public class ProductViewModel : Product
    {
        public List<SelectListItem> Brands { get; set; }

        [Display(Name = "Display Name")]
        public override string DisplayName { get; set; }

        [Display(Name = "Short Description")]
        public override string ShortDescription { get; set; }

        public Dictionary<string,IEnumerable<SelectListItem>> ProductAttributesList { get; set; }

        [Display(Name = "Available Attributes")]
        public int ProductAttributeId { get; set; }

    }
}
