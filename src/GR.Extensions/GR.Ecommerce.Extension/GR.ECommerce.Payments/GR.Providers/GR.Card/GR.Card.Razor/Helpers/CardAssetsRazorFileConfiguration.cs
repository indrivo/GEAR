using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Card.Razor.Helpers
{
    public class CardAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public CardAssetsRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}