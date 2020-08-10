namespace GR.Localization.Abstractions.ViewModels.LocalizationViewModels
{
    public class LocalizedStringViewModel
    {
        /// <summary>
        /// The name of the string in the resource it was loaded from.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The actual string.
        /// </summary>
        public string Value { get; set; }
    }
}
