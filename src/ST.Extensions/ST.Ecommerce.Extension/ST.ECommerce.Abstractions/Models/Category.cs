using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
{
    public class Category : BaseModel
    {
        /// <summary>
        /// Category name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Product categories
        /// </summary>
        public virtual IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
}
