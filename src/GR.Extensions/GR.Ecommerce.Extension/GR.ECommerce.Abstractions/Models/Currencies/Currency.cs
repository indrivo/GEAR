using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Abstractions.Models.Currencies
{
    public class Currency
    {
        /// <summary>
        /// Code
        /// </summary>
        [Required]
        public virtual string Code { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        [Required]
        public virtual string Symbol { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Plural name
        /// </summary>
        public virtual string PluralName { get; set; }

        /// <summary>
        /// Native symbol
        /// </summary>
        [Required]
        public virtual string NativeSymbol { get; set; }

        /// <summary>
        /// Decimal digits
        /// </summary>
        public virtual int DecimalDigits { get; set; } = 0;

        /// <summary>
        /// Rounding
        /// </summary>
        public virtual decimal Rounding { get; set; }
    }
}
