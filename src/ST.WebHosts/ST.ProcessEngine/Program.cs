using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ST.ProcessEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Procesess.ProcessEngine engine = new Procesess.ProcessEngine();
            engine.Start();
            CreateWebHostBuilder(args).Build().Run();
            engine.Shutdown();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
