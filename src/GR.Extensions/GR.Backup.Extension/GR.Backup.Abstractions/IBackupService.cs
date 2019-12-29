using System.Collections.Generic;
using GR.Backup.Abstractions.Models;

namespace GR.Backup.Abstractions
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