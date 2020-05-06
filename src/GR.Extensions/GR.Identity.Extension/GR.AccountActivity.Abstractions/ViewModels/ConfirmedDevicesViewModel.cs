using GR.AccountActivity.Abstractions.Models;

namespace GR.AccountActivity.Abstractions.ViewModels
{
    public class ConfirmedDevicesViewModel : UserDevice
    {
        /// <summary>
        /// Confirm date text
        /// </summary>
        public virtual string ConfirmDateText { get; set; }

        /// <summary>
        /// Is current device
        /// </summary>
        public virtual bool IsCurrent { get; set; }
    }
}