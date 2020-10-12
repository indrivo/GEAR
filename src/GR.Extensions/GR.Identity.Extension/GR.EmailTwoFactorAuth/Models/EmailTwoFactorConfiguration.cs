namespace GR.EmailTwoFactorAuth.Models
{
    /// <summary>
    /// The configuration for email two factor auth
    /// </summary>
    public class EmailTwoFactorConfiguration
    {
        public virtual bool UseHtmlTemplate { get; set; }
        public virtual string HtmlTemplateName { get; set; }
        public virtual int CodeLength { get; set; } = 6;
    }
}
