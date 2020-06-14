namespace GR.WebApplication.Models
{
    public class GearApplicationArgs
    {
        /// <summary>
        /// Use Kestrel configuration from appsettings configuration
        /// </summary>
        public virtual bool UseKestrel { get; set; }

        /// <summary>
        /// Redirect to https
        /// </summary>
        public virtual bool UseKestrelRedirectToHttps { get; set; }
    }
}