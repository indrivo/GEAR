using System.Collections.Generic;
using GR.Backup.Abstractions.ViewModels;
using GR.Core.Helpers;

namespace GR.Backup.Abstractions
{
    public interface IBackupService
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
        IEnumerable<BackupViewModel> GetBackups();

        /// <summary>
        /// Download backup
        /// </summary>
        /// <param name="backupName"></param>
        /// <returns></returns>
        DownloadBackupResultModel DownloadBackup(string backupName);

        /// <summary>
        /// Clear backups
        /// </summary>
        /// <returns></returns>
        ResultModel Clear();
    }
}