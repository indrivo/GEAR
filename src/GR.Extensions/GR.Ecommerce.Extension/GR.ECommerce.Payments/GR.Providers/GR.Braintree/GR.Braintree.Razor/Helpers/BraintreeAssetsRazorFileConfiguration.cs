using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Braintree.Razor.Helpers
{
    public class BraintreeAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public BraintreeAssetsRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}