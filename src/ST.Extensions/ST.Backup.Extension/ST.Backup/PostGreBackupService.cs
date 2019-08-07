using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ST.Backup.Abstractions;

namespace ST.Backup.PostGresSql
{
    public class PostGreBackupService : IBackupService<PostGreSqlBackupSettings>
    {
        /// <summary>
        /// Start backup database
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="directoryPath"></param>
        public void Backup(PostGreSqlBackupSettings settings, string directoryPath)
        {
            var currentDate = DateTime.Now;
            var outputFile = Path.Combine(directoryPath,
                $"backup {currentDate.Day}.{currentDate.Month}.{currentDate.Year} {currentDate.Hour}.{currentDate.Minute}.{settings.FileExtension}");
            var dumpCommand = "\"" + settings.PgDumpPath + "\"" + " -Fc" + " -h " + settings.Host + " -p " +
                              settings.Port + " -d " + settings.Database + " -U " + settings.User + "";
            var passFileContent = "" + settings.Host + ":" + settings.Port + ":" + settings.Database + ":" +
                                  settings.User + ":" + settings.Password + "";

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

        public string GetProviderName()
        {
            return nameof(PostGreBackupService);
        }
    }
}
