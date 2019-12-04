using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Entities.Abstractions.Constants;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Configurations;
using GR.PageRender.Abstractions.Models.ViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.PageRender
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Move BLL from controller to service")]
    public class ViewModelService : IViewModelService
    {
        #region Injectable
        /// <summary>
        /// Inject context
        /// </summary>
        public IDynamicPagesContext PagesContext { get; }

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        #endregion

        protected string PrefixKey = "ViewModel";

        public ViewModelService(IDynamicPagesContext pagesContext, ICacheService cacheService)
        {
            PagesContext = pagesContext;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get view model by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<ViewModel>> GetViewModelByIdAsync([Required]Guid? id)
        {
            if (id == null) return new InvalidParametersResultModel<ViewModel>();
            var cacheGet = await _cacheService.GetAsync<ViewModel>($"{PrefixKey}_{id}");
            if (cacheGet != null) return new SuccessResultModel<ViewModel>(cacheGet);
            var viewModel = await PagesContext.ViewModels
                .AsNoTracking()
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.Configurations)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (viewModel == null) return new NotFoundResultModel<ViewModel>();
            await _cacheService.SetAsync($"{PrefixKey}_{id}", viewModel);
            return new SuccessResultModel<ViewModel>(viewModel);
        }

        /// <summary>
        /// Get viewmodel field by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<ViewModelFields>> GetViewModelFieldByIdAsync([Required]Guid? id)
        {
            if (id == null) return new InvalidParametersResultModel<ViewModelFields>();
            var viewModelField = await PagesContext.ViewModelFields
                .Include(x => x.ViewModel)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (viewModelField == null) return new NotFoundResultModel<ViewModelFields>();
            return new SuccessResultModel<ViewModelFields>(viewModelField);
        }


        /// <summary>
        /// Add many to many configuration
        /// </summary>
        /// <param name="referenceEntity"></param>
        /// <param name="storageEntity"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SaveManyToManyConfigurationsAsync(Guid? referenceEntity, Guid? storageEntity, Guid? fieldId)
        {
            var rs = new ResultModel();
            if (referenceEntity == null || storageEntity == null || fieldId == null)
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Invalid parameters, all are required!"));
                return rs;
            }

            var field = await PagesContext.ViewModelFields
                .Include(x => x.ViewModel)
                .ThenInclude(x => x.TableModel)
                .Include(x => x.Configurations)
                .Include(x => x.TableModelFields)
                .FirstOrDefaultAsync(x => x.Id.Equals(fieldId));

            if (field == null)
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Invalid data"));
                return rs;
            }

            var viewModelTable = field.ViewModel?.TableModel;
            if (field.TableModelFields != null)
            {
                rs.Errors.Add(new ErrorModel("error", "This viewmodel field can't be used on a many to many relation, because he has a reference, remove reference and try again!"));
                return rs;
            }

            var refEntity = await PagesContext.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .FirstOrDefaultAsync(x => x.Id.Equals(referenceEntity));

            var stEntity = await PagesContext.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .FirstOrDefaultAsync(x => x.Id.Equals(storageEntity));

            if (refEntity == null || stEntity == null)
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Invalid data"));
                return rs;
            }

            string propertyName = null;
            string refPropertyName = null;

            foreach (var tableField in stEntity.TableFields)
            {
                if (tableField.DataType != TableFieldDataType.Guid) continue;
                var configs = tableField.TableFieldConfigValues;
                var table = configs.FirstOrDefault(x =>
                    x.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingTable);
                var schema = configs.FirstOrDefault(x =>
                    x.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingSchemaTable);
                if (table == null || schema == null) continue;
                if (table.Value == refEntity.Name)
                {
                    refPropertyName = tableField.Name;
                }

                if (table.Value == viewModelTable?.Name)
                {
                    propertyName = tableField.Name;
                }
            }

            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(refPropertyName))
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Incompatible choose entities!"));
                return rs;
            }

            field.VirtualDataType = ViewModelVirtualDataType.ManyToMany;

            field.Configurations = new List<ViewModelFieldConfiguration>
            {
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyReferenceEntityName,
                    ViewModelField = field,
                    Value = refEntity.Name
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyReferenceEntitySchema,
                    ViewModelField = field,
                    Value = refEntity.EntityType
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyStorageEntityName,
                    ViewModelField = field,
                    Value = stEntity.Name
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyStorageEntitySchema,
                    ViewModelField = field,
                    Value = stEntity.EntityType
                },

                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyReferencePropertyName,
                    ViewModelField = field,
                    Value = propertyName
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyStorageSenderPropertyName,
                    ViewModelField = field,
                    Value = refPropertyName
                },
            };

            PagesContext.ViewModelFields.Update(field);
            var rdb = await PagesContext.PushAsync();
            if (rdb.IsSuccess)
            {
                rs.IsSuccess = true;
            }
            else
            {
                rs.Errors = rdb.Errors;
            }

            return rs;
        }

        /// <summary>
        /// Change translate text
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="translatedKey"></param>
        /// <returns></returns>
        public async Task<ResultModel> ChangeViewModelFieldTranslateTextAsync([Required] Guid fieldId, [Required] string translatedKey)
        {
            var response = new ResultModel();
            var field = await PagesContext.ViewModelFields.FirstOrDefaultAsync(x => x.Id == fieldId);
            if (field == null)
            {
                response.Errors.Add(new ErrorModel("not_found", "Field not found!"));
                return response;
            }

            field.Translate = translatedKey;
            PagesContext.ViewModelFields.Update(field);
            try
            {
                await PagesContext.SaveChangesAsync();
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                response.Errors.Add(new ErrorModel("throw", e.Message));
            }

            return response;
        }

        /// <summary>
        /// Field mapping
        /// </summary>
        /// <param name="viewModelFieldId"></param>
        /// <param name="tableFieldId"></param>
        /// <returns></returns>
        public async Task<ResultModel> SetFieldsMappingAsync([Required] Guid viewModelFieldId, Guid tableFieldId)
        {
            var result = new ResultModel();
            var model = await PagesContext.ViewModelFields.FirstOrDefaultAsync(x => x.Id == viewModelFieldId);
            if (model == null) return result;
            model.TableModelFieldsId = tableFieldId;
            try
            {
                PagesContext.Update(model);
                await PagesContext.SaveChangesAsync();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return result;
        }

        /// <summary>
        /// Remove viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object> RemoveViewModelAsync(Guid? id)
        {
            if (id == null) return new { message = "Fail to delete view model!", success = false };
            var page = await PagesContext.ViewModels.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (page == null) return new { message = "Fail to delete view model!", success = false };

            try
            {
                PagesContext.ViewModels.Remove(page);
                await PagesContext.SaveChangesAsync();
                return new { message = "View model was delete with success!", success = true };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new { message = "Fail to delete view model!", success = false };
        }

        /// <summary>
        /// Load view models with pagination
        /// </summary>
        /// <param name="param"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public DTResult<object> LoadViewModelsWithPagination(DTParameters param, Guid entityId)
        {
            var filtered = PagesContext.FilterAbstractContext<ViewModel>(param.Search.Value, param.SortOrder,
                param.Start,
                param.Length,
                out var totalCount,
                x => (entityId != Guid.Empty && x.TableModelId == entityId) || entityId == Guid.Empty);


            var sel = filtered.Select(x => new
            {
                x.Author,
                x.Changed,
                x.Created,
                x.Id,
                x.IsDeleted,
                x.ModifiedBy,
                x.Name,
                Table = PagesContext.Table.FirstOrDefault(y => y.Id.Equals(x.TableModelId))?.Name
            }).Adapt<IEnumerable<object>>();

            var finalResult = new DTResult<object>
            {
                Draw = param.Draw,
                Data = sel.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count()
            };
            return finalResult;
        }


        /// <summary>
        /// Update items
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateItemsAsync<TItem>(IList<TItem> items) where TItem : ViewModelFields
        {
            var result = new ResultModel();
            if (!items.Any())
            {
                result.IsSuccess = true;
                return result;
            }

            var viewModel = items.First().ViewModelId;
            var fields = PagesContext.SetEntity<TItem>().Where(x => x.ViewModelId.Equals(viewModel)).ToList();

            foreach (var prev in fields)
            {
                var up = items.FirstOrDefault(x => x.Id.Equals(prev.Id));
                if (up == null)
                {
                    PagesContext.SetEntity<TItem>().Remove(prev);
                }
                else if (prev.Order != up.Order || prev.Name != up.Name)
                {
                    prev.Name = up.Name;
                    prev.Order = up.Order;
                    PagesContext.SetEntity<TItem>().Update(prev);
                }
            }

            var news = items.Where(x => x.Id == Guid.Empty).Select(x => new
            {
                ViewModelId = viewModel,
                x.Name,
                x.Order
            }).Adapt<IEnumerable<TItem>>().ToList();

            if (news.Any())
            {
                PagesContext.SetEntity<TItem>().AddRange(news);
            }

            await _cacheService.RemoveAsync($"{PrefixKey}_{viewModel}");

            return await PagesContext.PushAsync();
        }

        /// <summary>
        /// Update view model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateViewModelAsync([Required] ViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel();
            if (model.Id == Guid.Empty) return new InvalidParametersResultModel();
            var dataModel = PagesContext.ViewModels.FirstOrDefault(x => x.Id.Equals(model.Id));
            if (dataModel == null) return new NotFoundResultModel();

            dataModel.Name = model.Name;
            PagesContext.ViewModels.Update(dataModel);
            var dbRequest = await PagesContext.PushAsync();
            if (dbRequest.IsSuccess) await _cacheService.RemoveAsync($"{PrefixKey}_{model.Id}");
            return dbRequest;
        }

        /// <summary>
        /// Update field template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> UpdateViewModelFieldTemplateAsync(ViewModelFields model)
        {
            if (model == null || model.Id == Guid.Empty) return new InvalidParametersResultModel<Guid>();
            var dataModel = PagesContext.ViewModelFields.Include(x => x.ViewModel).FirstOrDefault(x => x.Id.Equals(model.Id));
            if (dataModel == null) return new NotFoundResultModel<Guid>();

            dataModel.Template = model.Template;
            PagesContext.ViewModelFields.Update(dataModel);
            var dbRequest = await PagesContext.PushAsync();
            return dbRequest.Map(dataModel.ViewModelId);
        }
    }
}
