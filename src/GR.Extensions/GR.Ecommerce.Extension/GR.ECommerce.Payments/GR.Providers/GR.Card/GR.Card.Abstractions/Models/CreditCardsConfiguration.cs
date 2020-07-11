namespace GR.Card.Abstractions.Models
{
    public class CreditCardsConfiguration
    {
        /// <summary>
        /// The currency with who will created test transaction
        /// </summary>
        public virtual string VerificationCardCurrencyCode { get; set; } = "USD";

        /// <summary>
        /// Value that will be extracted from user card and refund for card verification
        /// </summary>
        public virtual decimal VerificationCardValue { get; set; } = 0.01M;
    }
}