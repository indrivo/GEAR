using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Options;
using GR.Backup.Abstractions;

namespace GR.Backup.PostGresSql
{
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
            //TODO: Backup on mac os and linux envs
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;
            var directoryPath = GetDirectoryPath();
            var currentDate = DateTime.Now;
            var outputFile = Path.Combine(directoryPath,
                $"backup {currentDate.Day}.{currentDate.Month}.{currentDate.Year} {currentDate.Hour}.{currentDate.Minute}.{_options.Value.FileExtension}");
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
        /// Get provider name
        /// </summary>
        /// <returns></returns>
        public virtual string GetProviderName()
        {
            return nameof(PostGreBackupService);
        }

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
