namespace GR.Subscriptions.Razor.ViewModels.QuotationViewModels
{
    public sealed class AdditionalServicesViewModel : QuotationBaseViewModel
    {
        /// <summary>
        /// Request for new implementation
        /// </summary>
        public bool RequestForNewImplementation { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }
    }
}
