using GR.AccountActivity.Abstractions.Models;

namespace GR.AccountActivity.Abstractions.ViewModels
{
    public class UserDeviceViewModel : UserDevice
    {
        /// <summary>
        /// Confirm date text
        /// </summary>
        public virtual string ConfirmDateText { get; set; }
    }
}