using System.ComponentModel.DataAnnotations;

namespace GR.Subscriptions.Razor.ViewModels.QuotationViewModels
{
    public sealed class EnterpriseQuotationViewModel : QuotationBaseViewModel
    {
        /// <summary>
        /// NUmber of users
        /// </summary>
        [Required]
        public int NumberOfUsers { get; set; }

        /// <summary>
        /// Payment frequency
        /// </summary>
        public FrequencyOfPayment FrequencyOfPayment { get; set; } = FrequencyOfPayment.Monthly;
    }

    public enum FrequencyOfPayment
    {
        Monthly,
        Annually,
        Daily
    }
}
