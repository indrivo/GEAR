using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GR.Card.Abstractions.Helpers;
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
        [StringLength(4)]
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
        /// Get card type
        /// </summary>
        public virtual string Type => CreditCardValidator.GetCardType(CardNumber).ToString();

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

        /// <summary>
        /// Not available on new card
        /// </summary>
        public virtual Guid? CardId { get; set; }

        /// <summary>
        /// is default card
        /// </summary>
        public virtual bool IsDefault { get; set; }
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

    public class OrderWithSavedCreditCardPayViewModel
    {
        /// <summary>
        /// Order id
        /// </summary>
        [Required]
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// Card id
        /// </summary>
        [Required]
        public virtual Guid CardId { get; set; }
    }

    public class HiddenCreditCardPayViewModel : CreditCardPayViewModel
    {
        private readonly string _cardNumber;
        public HiddenCreditCardPayViewModel(string cardNumber)
        {
            _cardNumber = cardNumber;
        }

        public override string CardCode => "***";
        public override string Type => CreditCardValidator.GetCardType(_cardNumber).ToString();

        public override string CardNumber
        {
            get
            {
                var starGroup = new string('*', 4);
                var hiddenCardBuilder = new StringBuilder();
                var last4Numbers = _cardNumber.Substring(12);
                hiddenCardBuilder.Append($"{starGroup} {starGroup} {starGroup} {last4Numbers}");
                return hiddenCardBuilder.ToString();
            }
        }
    }
}