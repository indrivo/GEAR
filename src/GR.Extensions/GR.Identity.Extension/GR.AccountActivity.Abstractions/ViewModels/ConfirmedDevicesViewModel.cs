namespace GR.AccountActivity.Abstractions.ViewModels
{
    public class ConfirmedDevicesViewModel : UserDeviceViewModel
    {
        /// <summary>
        /// Is current device
        /// </summary>
        public virtual bool IsCurrent { get; set; }
    }
}