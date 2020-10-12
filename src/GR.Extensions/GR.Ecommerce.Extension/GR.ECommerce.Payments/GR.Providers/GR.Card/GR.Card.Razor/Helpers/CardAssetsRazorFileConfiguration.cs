using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Card.Razor.Helpers
{
    public class CardAssetsRazorFileConfiguration : StaticFileConfiguration
    {
        public CardAssetsRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}