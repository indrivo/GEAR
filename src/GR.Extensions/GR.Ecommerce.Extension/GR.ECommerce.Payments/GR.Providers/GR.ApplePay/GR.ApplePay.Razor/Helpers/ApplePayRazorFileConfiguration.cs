using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.ApplePay.Razor.Helpers
{
    public class ApplePayRazorFileConfiguration : StaticFileConfiguration
    {
        public ApplePayRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}