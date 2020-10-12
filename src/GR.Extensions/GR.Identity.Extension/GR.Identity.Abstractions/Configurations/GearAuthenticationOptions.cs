namespace GR.Identity.Abstractions.Configurations
{
    public class GearAuthenticationOptions
    {
        /// <summary>
        /// Allow password to expire after 1 month
        /// </summary>
        public virtual bool AllowPasswordExpiration { get; set; } = true;
    }
}
