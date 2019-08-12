using ST.Core.Helpers;
using ST.Files.Abstraction.ViewModels;
using System;
using System.Collections.Generic;

namespace ST.Files.Abstraction
{
    public interface IFileService
    {
        ResultModel<FileViewModel> GetFileById(Guid id);
        ResultModel<Guid> AddFile(FileViewModel dto);
        ResultModel<Guid> DeleteFile(Guid id);
        ResultModel<Guid> DeleteFilePermanent(Guid id);
        ResultModel<List<FileViewModel>> GetFilesByIds(List<Guid> idsList);
    }
}