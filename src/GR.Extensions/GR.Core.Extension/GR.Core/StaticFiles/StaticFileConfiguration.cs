using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace GR.Core.StaticFiles
{
    public class StaticFileConfiguration : IPostConfigureOptions<StaticFileOptions>
    {
        public StaticFileConfiguration(IHostingEnvironment environment)
        {
            Environment = environment;
        }
        public IHostingEnvironment Environment { get; }

        public void PostConfigure(string name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
            if (options.FileProvider == null && Environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider = options.FileProvider ?? Environment.WebRootFileProvider;

            const string basePath = "wwwroot";

            IFileProvider fileProvider = null;

            if (Debugger.IsAttached && GearApplication.IsDevelopment())
            {
                var root = AppContext.BaseDirectory.GetParentDirectory("src");
                var assembly = Assembly.GetAssembly(GetType());
                var projectDirectory = Path.GetFileNameWithoutExtension(assembly.Location);
                var directories = Directory.GetDirectories(root, projectDirectory, SearchOption.AllDirectories);
                if (directories.Any())
                {
                    var directory = directories[0];
                    var path = Path.Combine(directory, basePath);
                    if (Directory.Exists(path))
                        fileProvider = new PhysicalFileProvider(path);
                }
            }

            try
            {
                if (fileProvider == null)
                    fileProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, basePath);

                options.FileProvider = new CompositeFileProvider(options.FileProvider, fileProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}