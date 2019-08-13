using ST.Core.Helpers;
using ST.Files.Abstraction.ViewModels;
using System;
using System.Collections.Generic;

namespace ST.Files.Abstraction
{
    public interface IFileService
    {
        ResultModel<DownloadFileViewModel> GetFileById(Guid id);
        ResultModel<Guid> AddFile(UploadFileViewModel dto);
        ResultModel<Guid> DeleteFile(Guid id);
        ResultModel<Guid> DeleteFilePermanent(Guid id);

    }
}