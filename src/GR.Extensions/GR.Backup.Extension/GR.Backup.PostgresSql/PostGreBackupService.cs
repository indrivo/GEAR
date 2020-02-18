using GR.Backup.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GR.Backup.PostGresSql
{
    //Special folders https://developers.redhat.com/blog/2018/11/07/dotnet-special-folder-api-linux/
    [Author(Authors.LUPEI_NICOLAE)]
    public class PostGreBackupService : IBackupService<PostGreSqlBackupSettings>
    {
        #region Injectable

        /// <summary>
        /// Inject options
        /// </summary>
        private readonly IOptions<PostGreSqlBackupSettings> _options;

        #endregion

        public PostGreBackupService(IOptions<PostGreSqlBackupSettings> options)
        {
            _options = options;
        }

        /// <summary>
        /// Start backup database
        /// </summary>
        public virtual void Backup()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ExecuteWindowsDump();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ExecuteLinuxDump();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                //TODO: Backup on mac os env
            }
        }

        /// <summary>
        /// Build dump output file
        /// </summary>
        /// <returns></returns>
        protected string BuildDumpOutputFile()
        {
            var directoryPath = GetDirectoryPath();
            var currentDate = DateTime.Now;
            var outputFile = Path.Combine(directoryPath,
                $"{_options.Value.Database}-backup {currentDate.Day}.{currentDate.Month}.{currentDate.Year} {currentDate.Hour}.{currentDate.Minute}.{_options.Value.FileExtension}");
            return outputFile;
        }

        /// <summary>
        /// Execute dump in windows
        /// </summary>
        protected void ExecuteWindowsDump()
        {
            var outputFile = BuildDumpOutputFile();
            var dumpCommand = "\"" + _options.Value.PgDumpPath + "\"" + " -Fc" + " -h " + _options.Value.Host + " -p " +
                              _options.Value.Port + " -d " + _options.Value.Database + " -U " + _options.Value.User + "";
            var passFileContent = "" + _options.Value.Host + ":" + _options.Value.Port + ":" + _options.Value.Database + ":" +
                                  _options.Value.User + ":" + _options.Value.Password + "";

            var batFilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + ".bat");

            var passFilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + ".conf");

            try
            {
                var batchContent = "";
                batchContent += "@" + "set PGPASSFILE=" + passFilePath + "\n";
                batchContent += "@" + dumpCommand + "  > " + "\"" + outputFile + "\"" + "\n";

                File.WriteAllText(
                    passFilePath,
                    passFileContent,
                    Encoding.ASCII);

                File.WriteAllText(
                    batFilePath,
                    batchContent,
                    Encoding.ASCII);

                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                var oInfo = new ProcessStartInfo(batFilePath)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(oInfo))
                {
                    if (proc == null) return;
                    proc.WaitForExit();
                    proc.Close();
                }
            }
            finally
            {
                if (File.Exists(batFilePath))
                    File.Delete(batFilePath);

                if (File.Exists(passFilePath))
                    File.Delete(passFilePath);
            }
        }

        /// <summary>
        /// Execute linux dump file backup
        /// </summary>
        protected void ExecuteLinuxDump()
        {
            var outputFile = BuildDumpOutputFile();
            var command = $"PGPASSWORD=\"{_options.Value.Password}\" pg_dump -h {_options.Value.Host}  -p {_options.Value.Port} -U {_options.Value.User} -F c -b -v -f \"{outputFile}\" {_options.Value.Database}";

            var result = "";
            using (var proc = new Process())
            {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();

                result += proc.StandardOutput.ReadToEnd();
                result += proc.StandardError.ReadToEnd();

                proc.WaitForExit();
            }
            Debug.WriteLine(result);
        }

        /// <summary>
        /// Get provider name
        /// </summary>
        /// <returns></returns>
        public virtual string GetProviderName() => GetType().Name;

        /// <summary>
        /// Get backups
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetBackups()
        {
            var files = Directory.GetFiles(GetDirectoryPath()).ToList();
            return files;
        }

        /// <summary>
        /// Get directory path
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDirectoryPath()
        {
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var directoryPath = Path.Combine(userProfilePath, $"backup\\{_options.Value.BackupFolder}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
    }
}