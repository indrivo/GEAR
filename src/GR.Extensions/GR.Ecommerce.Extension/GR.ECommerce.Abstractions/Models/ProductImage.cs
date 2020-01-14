using System;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductImage : BaseModel
    {
        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public virtual string FileName { get; set; }

        /// <summary>
        /// Content type
        /// </summary>
        public virtual string ContentType { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        public virtual int Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public virtual int Height { get; set; }

        /// <summary>
        /// Image bytes
        /// </summary>
        public virtual byte[] Image { get; set; }
        /// <summary>
        /// Reference to product 
        /// </summary>
        public virtual Product Product { get; set; }
        public virtual Guid ProductId { get; set; }
    }
}
