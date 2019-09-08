using System.Collections.Generic;
using ST.Backup.Abstractions.Models;

namespace ST.Backup.Abstractions
{
    public interface IBackupService<in TSettings> where TSettings : BackupSettings
    {
        /// <summary>
        /// Make backup
        /// </summary>
        void Backup();

        /// <summary>
        /// Get provider name
        /// </summary>
        /// <returns></returns>
        string GetProviderName();

        /// <summary>
        /// List of backups
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetBackups();
    }
}