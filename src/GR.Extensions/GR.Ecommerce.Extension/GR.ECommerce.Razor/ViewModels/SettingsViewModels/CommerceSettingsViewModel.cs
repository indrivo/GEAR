using System.Collections.Generic;
using GR.ECommerce.Abstractions.Models.Currencies;

namespace GR.ECommerce.Razor.ViewModels.SettingsViewModels
{
    public class CommerceSettingsViewModel
    {
        public virtual string CurrencyCode { get; set; }
        public virtual IEnumerable<Currency> Currencies { get; set; }

        public virtual int DaysToNotifyExpiringSubscriptions { get; set; }

        public virtual int DaysToFreeTailPeriod { get; set; }
    }
}