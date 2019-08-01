using ST.Backup.Abstractions.Models;

namespace ST.Backup.Abstractions
{
    public interface IBackupService<TSettings> where TSettings : BackupSettings
    {
        void Backup(TSettings settings, string directoryPath);
        string GetProviderName();
    }
}