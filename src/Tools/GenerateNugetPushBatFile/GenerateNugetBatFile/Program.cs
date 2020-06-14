using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateNugetBatFile
{
    public class Program
    {
        private static readonly List<string> Projects = new List<string>();
        private static readonly List<string> IgnoreFolders = new List<string>
        {
            "GR.TestMobileAuth",
            "GR.Cms",
            "GR.Core.Tests",
            "GenerateNugetBatFile"
        };

        /// <summary>
        /// Path
        /// </summary>
        private static string _path;

        public static void Main()
        {
            _path = GetParent(AppContext.BaseDirectory, "src");
            ExtractCsProjects(GetDirectories(_path));

            var nugetPub = GenerateNugetProjects();
            var basePath = Path.Combine(_path, "deploy-packages-generated.bat");
            if (!File.Exists(basePath))
            {
                File.Create(basePath);
            }
            File.WriteAllText(basePath, nugetPub);
            Console.ReadKey();
        }

        /// <summary>
        /// Get directories
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path).Where(x => !IgnoreFolders.Contains(new DirectoryInfo(x).Name));
        }

        private static int[] GetRemoveIndexes()
        {
            return new[]
            {
                0, _path.Length + 1
            };
        }

        /// <summary>
        /// Get parent directory path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parentName"></param>
        /// <returns></returns>
        public static string GetParent(string path, string parentName)
        {
            var dir = new DirectoryInfo(path);

            if (dir.Parent == null)
            {
                return null;
            }

            return dir.Parent.Name == parentName
                ? dir.Parent.FullName
                : GetParent(dir.Parent.FullName, parentName);
        }

        /// <summary>
        /// Extract csharp projects from solution 
        /// </summary>
        /// <param name="paths"></param>
        private static void ExtractCsProjects(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var files = Directory.GetFiles(path)
                    .Where(x => x.EndsWith(".csproj"))
                    .Select(x =>
                    {
                        var d = GetRemoveIndexes();
                        return x.Remove(d[0], d[1]);
                    }).ToList();

                if (files.Any())
                {
                    Projects.AddRange(files);
                }

                var dirs = GetDirectories(path);
                ExtractCsProjects(dirs);
            }
        }

        private static string GenerateNugetProjects()
        {
            var builder = new StringBuilder();

            builder.AppendLine("SET pushKey=\"\"");
            builder.AppendLine("SET pushHost=\"https://www.nuget.org\"");

            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine();

            builder.AppendLine(":: Pack projects");
            builder.AppendLine("dotnet build ../GR.sln");

            foreach (var project in Projects)
            {
                var proj = project.Replace("\\", "/");
                builder.AppendLine($"dotnet pack ./{proj} -o ../../../nupkgs");
            }
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine();

            builder.AppendLine(":: Push projects");
            builder.AppendLine("cd ./nupkgs");

            foreach (var project in Projects)
            {
                var projName = Path.GetFileNameWithoutExtension(project);
                builder.AppendLine($"dotnet nuget push -k %pushKey% -s %pushHost% {projName}*");
            }

            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("::Clean");
            builder.AppendLine("cd ..");
            builder.AppendLine("rmdir /q /s \"nupkgs\"");
            builder.AppendLine("PAUSE");
            return builder.ToString();
        }
    }
}