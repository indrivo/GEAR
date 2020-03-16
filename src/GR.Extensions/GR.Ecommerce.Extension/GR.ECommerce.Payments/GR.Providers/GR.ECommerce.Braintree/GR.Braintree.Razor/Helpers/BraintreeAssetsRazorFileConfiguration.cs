using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Braintree.Razor.Helpers
{
    public class BraintreeAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public BraintreeAssetsRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}