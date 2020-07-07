using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.ECommerce.Abstractions.ViewModels.ProductCategoriesViewModels
{
    public class GetProductCategoryViewModel : BaseModel
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
        /// Parent Category Id
        /// </summary>
        public virtual Guid? ParentCategoryId { get; set; }

        /// <summary>
        /// Category Is Published
        /// </summary>
        [Required]
        public virtual bool IsPublished { get; set; }
    }
}