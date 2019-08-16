using ST.Core.Helpers;
using System;
using ST.Files.Abstraction.Models.ViewModels;

namespace ST.Files.Abstraction
{
    public interface IFileManager
    {
        /// <summary>
        /// Get file by uniqueidentifier id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultModel<DownloadFileViewModel> GetFileById(Guid id);

        /// <summary>
        /// Add or update file into a repository
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        ResultModel<Guid> AddFile(UploadFileViewModel dto);

        /// <summary>
        /// Remove file logical
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultModel<Guid> DeleteFile(Guid id);

        /// <summary>
        /// Remove file permanent
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultModel<Guid> DeleteFilePermanent(Guid id);

        /// <summary>
        /// Restore file (only if is deleted logical)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultModel<Guid> RestoreFile(Guid id);
    }
}