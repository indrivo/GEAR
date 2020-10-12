using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Paypal.Razor.Helpers
{
    public class PaypalAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public PaypalAssetsRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}