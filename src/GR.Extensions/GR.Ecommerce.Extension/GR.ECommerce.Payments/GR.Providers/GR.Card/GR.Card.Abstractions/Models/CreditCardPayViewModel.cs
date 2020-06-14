using System;
using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes.Validation;
using GR.Core.Helpers.Global;

namespace GR.Card.Abstractions.Models
{
    public class CreditCardPayViewModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        [Required]
        [DataType(DataType.CreditCard)]
        [CreditCard]
        [CardNumber]
        public virtual string CardNumber { get; set; }

        /// <summary>
        /// CVV code
        /// </summary>
        [Required]
        public virtual string CardCode { get; set; }

        /// <summary>
        /// Owner of current credit card
        /// </summary>
        [Required]
        public virtual string Owner { get; set; }

        /// <summary>
        /// Month of expired card
        /// </summary>
        [Required]
        [StringLength(2)]
        public virtual string Month { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        [Required]
        [StringLength(4)]
        public virtual string Year { get; set; }

        /// <summary>
        /// The expiration of card. EX: 06/12
        /// </summary>
        public virtual string ExpirationDate => $"{Month}/{Year.Substring(2)}";

        /// <summary>
        /// Sandbox example
        /// </summary>
        public static CreditCardPayViewModel SandboxCreditCard => new CreditCardPayViewModel
        {
            Owner = Authors.LUPEI_NICOLAE,
            CardNumber = "4111111111111111",
            CardCode = "123",
            Month = "10",
            Year = DateTime.Today.AddYears(3).Year.ToString()
        };
    }

    public class OrderCreditCardPayViewModel : CreditCardPayViewModel
    {
        /// <summary>
        /// Order id
        /// </summary>
        [Required]
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// Save for future purchases
        /// </summary>
        public virtual bool SaveCreditCard { get; set; }
    }
}