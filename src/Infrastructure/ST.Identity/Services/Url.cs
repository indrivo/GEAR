using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ST.Identity.Abstractions;

namespace ST.Identity.Services
{
    public class Url : IUrl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Url(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        private IHostingEnvironment HostingEnvironment { get; }

        private IConfiguration Configuration { get; }
        /// <summary>
        /// Gets url of authority
        /// </summary>
        /// <returns>url</returns>
        public string GetUrl()
        {
            var stage = GetStage();

            var Url = Configuration.GetSection($"Authorization:Authority:{ stage }:uri").Value;

            return Url;
        }
        /// <summary>
        /// Determinates actual kind of stage 
        /// </summary>
        /// <returns>stage</returns>
        public  string GetStage()
        {
            var envName = HostingEnvironment.EnvironmentName;
            switch (envName)
            {
                case "Development":
                    return "Dev";
                case "Production":
                    return "Prod";
                case "Stage":
                    return "Stage";
                default: return default;

            }
        }
    }
}
