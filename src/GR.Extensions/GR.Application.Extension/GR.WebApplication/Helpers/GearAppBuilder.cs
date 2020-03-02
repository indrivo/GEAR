using System;
using Microsoft.AspNetCore.Builder;

namespace GR.WebApplication.Helpers
{
    public interface IGearAppBuilder : IApplicationBuilder
    {

    }

    public class GearAppBuilder : ApplicationBuilder, IGearAppBuilder
    {
        public GearAppBuilder(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public GearAppBuilder(IServiceProvider serviceProvider, object server) : base(serviceProvider, server)
        {

        }
    }
}
