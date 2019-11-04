using System;
using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Abstractions.ViewModels.OrderViewModels
{
    public class OrderCartViewModel
    {
        /// <summary>
        /// Cart id
        /// </summary>
        [Required]
        public virtual Guid? CartId { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        public virtual string Notes { get; set; }
    }
}