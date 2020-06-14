using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.MobilPay.Razor.Helpers
{
    public class MobilPayAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public MobilPayAssetsRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}