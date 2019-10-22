using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.ECommerce.Paypal.Razor.Helpers
{
    public class PaypalAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public PaypalAssetsRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}