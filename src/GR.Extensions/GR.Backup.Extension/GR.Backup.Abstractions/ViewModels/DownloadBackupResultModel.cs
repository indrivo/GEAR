using GR.Core.Helpers;

namespace GR.Backup.Abstractions.ViewModels
{
    public sealed class DownloadBackupResultModel : ResultModel<byte[]>
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
    }
}
