using System;
using ST.BaseBusinessRepository;
using ST.Entities.ViewModels.DynamicEntities;

namespace ST.Files.Abstraction
{
    public interface IFileProvider
    {
        ResultModel<Guid> Create(EntityViewModel model);
        ResultModel<EntityViewModel> ListFilesByKey(EntityViewModel model, Guid key);
        ResultModel<EntityViewModel> GetFileContent(EntityViewModel model);
        ResultModel<EntityViewModel> AddFile(EntityViewModel model, Guid fileRef);
        ResultModel<EntityViewModel> DeleteFiles(EntityViewModel model);
    }
}
