using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GR.ECommerce.Razor.ViewModels
{
    public class OrderCartViewModel
    {
        [Required]
        public Guid CartId { get; set; }
        public string Notes { get; set; }
    }
}
