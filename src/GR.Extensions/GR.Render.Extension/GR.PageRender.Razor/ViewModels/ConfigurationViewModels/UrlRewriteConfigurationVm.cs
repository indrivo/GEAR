namespace GR.PageRender.Razor.ViewModels.ConfigurationViewModels
{
    public class UrlRewriteConfigurationVm
    {
        public virtual bool UseDynamicPageUrlRewrite { get; set; } = true;
        public virtual string NotFoundPageRelativeUrl { get; set; } = "/Handler/NotFound";
    }
}