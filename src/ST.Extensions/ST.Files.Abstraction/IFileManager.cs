using ST.Core.Helpers;
using System;
using ST.Files.Abstraction.Models.ViewModels;

namespace ST.Files.Abstraction
{
    public interface IFileManager
    {
        ResultModel<DownloadFileViewModel> GetFileById(Guid id);
        ResultModel<Guid> AddFile(UploadFileViewModel dto);
        ResultModel<Guid> DeleteFile(Guid id);
        ResultModel<Guid> DeleteFilePermanent(Guid id);
        ResultModel<Guid> RestoreFile(Guid id);
    }
}