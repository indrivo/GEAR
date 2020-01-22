namespace GR.Identity.Models
{
    public class ErrorViewModel
    {
        private string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}