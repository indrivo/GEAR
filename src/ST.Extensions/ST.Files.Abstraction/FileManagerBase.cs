using System;
using ST.Core.Helpers;
using ST.Files.Abstraction.Models.ViewModels;

namespace ST.Files.Abstraction
{
    public abstract class FileManagerBase : IFileManager
    {
        public abstract ResultModel<DownloadFileViewModel> GetFileById(Guid id);


        public abstract ResultModel<Guid> AddFile(UploadFileViewModel dto, Guid tenantId);


        public abstract ResultModel<Guid> DeleteFile(Guid id);


        public abstract ResultModel<Guid> DeleteFilePermanent(Guid id);


        public abstract ResultModel<Guid> RestoreFile(Guid id);


        public abstract ResultModel ChangeSettings<TSettingsViewModel>(TSettingsViewModel newSettings)
            where TSettingsViewModel : FileSettingsViewModel;
    }
}
