using System;
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
        /// Category Description
        /// </summary>
        [Required]
        public virtual string Description { get; set; }

        /// <summary>
        /// Category Display Order
        /// </summary>
        [Required]
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Parent Category
        /// </summary>
        public virtual Category ParentCategory { get; set; }

        /// <summary>
        /// Parent Category Id
        /// </summary>
        public virtual Guid? ParentCategoryId { get; set; }

        /// <summary>
        /// Category Is Published
        /// </summary>
        [Required]
        public virtual bool IsPublished { get; set; }

        /// <summary>
        /// Product categories
        /// </summary>
        public virtual IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
}