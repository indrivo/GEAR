using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.PageRender.Abstractions.Models.ViewModels;

namespace GR.PageRender.Abstractions
{
    public interface IViewModelService
    {
        IDynamicPagesContext PagesContext { get; }

        /// <summary>
        /// Get view model by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<ViewModel>> GetViewModelByIdAsync([Required] Guid? id);

        /// <summary>
        /// Get viewmodel field by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<ViewModelFields>> GetViewModelFieldByIdAsync([Required] Guid? id);

        /// <summary>
        /// Add many to many configuration
        /// </summary>
        /// <param name="referenceEntity"></param>
        /// <param name="storageEntity"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        Task<ResultModel> SaveManyToManyConfigurationsAsync(Guid? referenceEntity, Guid? storageEntity, Guid? fieldId);

        /// <summary>
        /// Change translation
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="translatedKey"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeViewModelFieldTranslateTextAsync([Required] Guid fieldId, [Required] string translatedKey);

        /// <summary>
        /// Field mapping
        /// </summary>
        /// <param name="viewModelFieldId"></param>
        /// <param name="tableFieldId"></param>
        /// <returns></returns>
        Task<ResultModel> SetFieldsMappingAsync([Required] Guid viewModelFieldId, Guid tableFieldId);

        /// <summary>
        /// Remove viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<object> RemoveViewModelAsync(Guid? id);

        /// <summary>
        /// Load view models with pagination
        /// </summary>
        /// <param name="param"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        DTResult<object> LoadViewModelsWithPagination(DTParameters param, Guid entityId);

        /// <summary>
        /// Update items
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateItemsAsync<TItem>(IList<TItem> items) where TItem : ViewModelFields;

        /// <summary>
        /// Update viewmodel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateViewModelAsync([Required] ViewModel model);

        /// <summary>
        /// Update field template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> UpdateViewModelFieldTemplateAsync(ViewModelFields model);
    }
}