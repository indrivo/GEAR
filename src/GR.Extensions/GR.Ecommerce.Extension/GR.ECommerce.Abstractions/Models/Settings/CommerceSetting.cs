using System.ComponentModel.DataAnnotations;
using GR.Core.Abstractions;
using GR.ECommerce.Abstractions.Models.Currencies;

namespace GR.ECommerce.Abstractions.Models.Settings
{
    public class CommerceSetting : IBase<int>
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public virtual int Id { get; set; } = 1;

        /// <summary>
        /// Global usage of currency
        /// </summary>
        public virtual Currency Currency { get; set; }
        [Required]
        public virtual string CurrencyId { get; set; }
    }
}
