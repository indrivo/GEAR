using System.Collections.Generic;

namespace GR.GooglePay.Abstractions.ViewModels
{
    public class GPayEditSettingsViewModel : GPaySettingsViewModel
    {
        public virtual IEnumerable<string> SupportedAllowedCardNetworks => new List<string>
        {
            "AMEX", "DISCOVER", "INTERAC", "JCB", "MASTERCARD", "VISA"
        };

        public virtual IEnumerable<string> SupportedAllowedCardAuthMethods => new List<string>
        {
            "PAN_ONLY", "CRYPTOGRAM_3DS"
        };
    }
}
