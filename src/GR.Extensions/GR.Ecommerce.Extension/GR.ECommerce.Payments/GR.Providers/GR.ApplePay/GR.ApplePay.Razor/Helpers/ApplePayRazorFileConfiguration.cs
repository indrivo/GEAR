using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.ApplePay.Razor.Helpers
{
    public class ApplePayRazorFileConfiguration : StaticFileConfiguration
    {
        public ApplePayRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}