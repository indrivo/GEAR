using System;
using ST.BaseBusinessRepository;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Files.Abstraction;

namespace ST.Files.Services
{
    public class FileManager
    {
        private readonly IFileProvider _fileProvider;

        public FileManager(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        /// <summary>
        ///     Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<Guid> Create(EntityViewModel model)
        {
            return _fileProvider.Create(model);
        }

        /// <summary>
        ///     List of files by key
        /// </summary>
        /// <param name="model"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> ListFilesByKey(EntityViewModel model, Guid key)
        {
            return _fileProvider.ListFilesByKey(model, key);
        }

        /// <summary>
        ///     Get file content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> GetFileContent(EntityViewModel model)
        {
            return _fileProvider.GetFileContent(model);
        }

        /// <summary>
        ///     Add file
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> AddFile(EntityViewModel model, Guid fileRef)
        {
            return _fileProvider.AddFile(model, fileRef);
        }

        /// <summary>
        ///     Delete file
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> DeleteFiles(EntityViewModel model)
        {
            return _fileProvider.DeleteFiles(model);
        }
    }
}