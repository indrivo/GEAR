using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Core.Attributes;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class ProductsPaginatedViewModel : BaseModel
    {
        /// <summary>
        /// Product name
        /// </summary>
        [Required]
        [DisplayTranslate(Key = "name")]
        public virtual string Name { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// Short Description
        /// </summary>
        public virtual string ShortDescription { get; set; }
        /// <summary>
        /// Product Description
        /// </summary>
        [DisplayTranslate(Key = "description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Brand id
        /// </summary>
        public virtual Guid BrandId { get; set; }

        /// <summary>
        /// Publish state of product
        /// </summary>
        public virtual bool IsPublished { get; set; }

        /// <summary>
        /// Sku
        /// </summary>
        public virtual string Sku { get; set; }

        /// <summary>
        /// Gtin
        /// </summary>
        public virtual string Gtin { get; set; }
    }
}
