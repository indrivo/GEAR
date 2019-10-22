using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Paypal.Razor.Helpers
{
    public class PaypalAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public PaypalAssetsRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}