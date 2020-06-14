namespace GR.Card.Abstractions.Models
{
    public class CardSettingsViewModel
    {
        public virtual bool IsSandbox { get; set; }
        public virtual string ApiKey { get; set; }
        public virtual string TransactionKey { get; set; }
    }
}