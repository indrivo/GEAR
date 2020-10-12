using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.MobilPay.Razor.Helpers
{
    public class MobilPayAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public MobilPayAssetsRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}