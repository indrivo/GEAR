namespace GR.Identity.LdapAuth.Abstractions.Models
{
    public class LdapConfiguration
    {
        /// <summary>
        /// Import automatically on user log in 
        /// </summary>
        public virtual bool AutoImportOnLogin { get; set; }
    }
}