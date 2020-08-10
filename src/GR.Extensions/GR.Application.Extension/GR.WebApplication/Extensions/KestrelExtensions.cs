using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.WebApplication.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GR.WebApplication.Extensions
{
    /// <summary>
    /// This class represent an extension for kestrel server options
    /// For get a *.pfx certificate from Let's Encrypt service (pem)
    /// run: openssl pkcs12 -export -out certificate.pfx -inkey privkey.pem -in cert.pem -certfile chain.pem
    ///
    /// This class is a inspired from this article: https://devblogs.microsoft.com/aspnet/configuring-https-in-asp-net-core-across-different-platforms/
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public static class KestrelExtensions
    {
        /// <summary>
        /// Configure kestrel end points
        /// </summary>
        /// <param name="options"></param>
        public static void ConfigureEndpoints(this KestrelServerOptions options)
        {
            var configuration = options.ApplicationServices.GetRequiredService<IConfiguration>();
            var environment = options.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var enabled = configuration.GetSection("HttpServer").GetValue<bool>("Enabled");
            if (!enabled) return;
            var endpoints = configuration.GetSection("HttpServer:Endpoints")
                .GetChildren()
                .ToDictionary(section => section.Key, section =>
                {
                    var endpoint = new EndpointConfiguration();
                    section.Bind(endpoint);
                    return endpoint;
                });

            foreach (var endpoint in endpoints)
            {
                var config = endpoint.Value;
                var port = config.Port ?? (config.Scheme == "https" ? 443 : 80);

                var ipAddresses = new List<IPAddress>();
                if (config.Host == "localhost")
                {
                    ipAddresses.Add(IPAddress.IPv6Loopback);
                    ipAddresses.Add(IPAddress.Loopback);
                }
                else if (IPAddress.TryParse(config.Host, out var address))
                {
                    ipAddresses.Add(address);
                }
                else
                {
                    ipAddresses.Add(IPAddress.IPv6Any);
                }

                foreach (var address in ipAddresses)
                {
                    options.Listen(address, port,
                        listenOptions =>
                        {
                            if (config.Scheme == "https")
                            {
                                var certificate = LoadCertificate(config, environment);
                                listenOptions.UseHttps(certificate);
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Load certificate
        /// </summary>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        private static X509Certificate2 LoadCertificate(EndpointConfiguration config, IWebHostEnvironment environment)
        {
            if (config.StoreName != null && config.StoreLocation != null)
            {
                using var store = new X509Store(config.StoreName, Enum.Parse<StoreLocation>(config.StoreLocation));
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates.Find(
                    X509FindType.FindBySubjectName,
                    config.Host,
                    validOnly: !environment.IsDevelopment());

                if (certificate.Count == 0)
                {
                    throw new InvalidOperationException($"Certificate not found for {config.Host}.");
                }

                return certificate[0];
            }

            var certificateRoot = Path.Combine(AppContext.BaseDirectory, GlobalResources.Paths.CertificatesPath);
            var filePath = Path.Combine(certificateRoot, config.FilePath);
            if (config.FilePath != null && config.Password != null && File.Exists(filePath))
            {
                return new X509Certificate2(filePath, config.Password);
            }

            throw new InvalidOperationException("No valid certificate configuration found for the current endpoint.");
        }
    }
}