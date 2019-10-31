using System.IO;
using RazorLight;
using GR.Core.Attributes.Documentation;

namespace GR.Dashboard.Abstractions.Helpers.Compilers
{
    [Author("Lupei Nicolae", 1.1)]
    [Documentation("Razor template runtime compiler")]
    public static class RazorCompilerEngine
    {
        /// <summary>
        /// Compiler what render razor templates
        /// </summary>
        public static RazorLightEngine Compiler { get; private set; }

        /// <summary>
        /// Read template content from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadTemplateFromFile(string path)
        {
            return !File.Exists(path) ? string.Empty : File.ReadAllText(path);
        }

        /// <summary>
        /// Register engine
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterEngine(RazorLightEngineBuilder builder = null)
        {
            Compiler = (builder ?? new RazorLightEngineBuilder()
                            .UseMemoryCachingProvider()).Build();
        }
    }
}
