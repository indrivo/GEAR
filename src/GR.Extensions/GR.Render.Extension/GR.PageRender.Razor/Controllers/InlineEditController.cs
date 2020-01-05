using GR.Core;
using GR.Core.Attributes;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.Core.Helpers.Filters.Enums;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Entities.Abstractions.Constants;
using GR.Identity.Abstractions;
using GR.PageRender.Abstractions;
using GR.PageRender.Razor.ViewModels.TableColumnsViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GR.PageRender.Razor.Controllers
{
    [Authorize]
    public class InlineEditController : Controller
    {
        /// <summary>
        /// Inject page context
        /// </summary>
        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
        /// Inject user manager
        /// </summary>

        private readonly UserManager<GearUser> _userManager;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Inject Data Service
        /// </summary>
        private readonly IDynamicService _service;

        public InlineEditController(IDynamicPagesContext pagesContext, IDynamicService service, UserManager<GearUser> userManager)
        {
            _pagesContext = pagesContext;
            _service = service;
            _userManager = userManager;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>
        /// Get view model column type for inline table edit
        /// </summary>
        /// <param name="viewModelId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetViewModelColumnTypes(Guid? viewModelId)
        {
            var result = new ResultModel();
            if (viewModelId == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Not specified view model id"));
                return Json(result);
            }

            var viewModel = await _pagesContext.ViewModels
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.Configurations)
                .FirstOrDefaultAsync(x => x.Id == viewModelId);
            if (viewModel == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "ViewModel not found"));
                return Json(result);
            }

            var tableFields = _pagesContext.TableFields.Where(x => x.TableId == viewModel.TableModelId).ToList();
            if (!tableFields.Any()) return Json(result);
            var entityFields = tableFields.Select(field =>
            {
                var obj = field.Adapt<TableColumnData>();
                obj.ColumnId = viewModel.ViewModelFields?.FirstOrDefault(x => x.TableModelFieldsId == field.Id)?.Id;
                return obj;
            });

            result.IsSuccess = true;
            result.Result = new ViewModelColumnDefinitionsViewModel
            {
                EntityFields = entityFields,
                ViewModelFields = viewModel.ViewModelFields
            };

            return new JsonResult(result, _jsonSerializerSettings);
        }

        /// <summary>
        /// Save table cell
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="propertyId"></param>
        /// <param name="rowId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public virtual async Task<JsonResult> SaveTableCellData(Guid? entityId, Guid? propertyId, Guid? rowId, string value)
        {
            var result = new ResultModel();
            if (entityId == null || propertyId == null || rowId == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Not specified data"));
                return Json(result);
            }

            var entity = await _pagesContext.Table.Include(x => x.TableFields).FirstOrDefaultAsync(x => x.Id == entityId);
            if (entity == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Entity not found"));
                return Json(result);
            }

            if (entity.IsSystem || entity.IsPartOfDbContext)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "The system entity can not be edited"));
                return Json(result);
            }

            var property = entity.TableFields.First(x => x.Id == propertyId);
            if (property == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Not found entity column"));
                return Json(result);
            }

            var row = await _service.GetById(entity.Name, rowId.Value);
            if (!row.IsSuccess)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Entry Not found"));
                return Json(result);
            }

            if (row.Result.ContainsKey(property.Name))
            {
                switch (property.DataType)
                {
                    case TableFieldDataType.Guid:
                        {
                            Guid.TryParse(value, out var parsed);
                            row.Result[property.Name] = parsed;
                        }
                        break;

                    case TableFieldDataType.Boolean:
                        {
                            bool.TryParse(value, out var val);
                            row.Result[property.Name] = val;
                        }
                        break;

                    case TableFieldDataType.Int:
                        {
                            try
                            {
                                row.Result[property.Name] = Convert.ToInt32(value);
                            }
                            catch
                            {
                                row.Result[property.Name] = value;
                            }
                        }
                        break;

                    case TableFieldDataType.Decimal:
                        {
                            try
                            {
                                row.Result[property.Name] = Convert.ToDecimal(value);
                            }
                            catch
                            {
                                row.Result[property.Name] = value;
                            }
                        }
                        break;

                    case TableFieldDataType.Date:
                    case TableFieldDataType.DateTime:
                        {
                            DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                                out var parsed);
                            row.Result[property.Name] = parsed;
                        }
                        break;

                    default:
                        row.Result[property.Name] = value;
                        break;
                }
            }

            if (row.Result.ContainsKey(nameof(BaseModel.Changed)))
            {
                row.Result[nameof(BaseModel.Changed)] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }

            var req = await _service.Update(entity.Name, row.Result);
            if (!req.IsSuccess)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Fail to save data"));
                return Json(result);
            }

            result.IsSuccess = true;
            return Json(result);
        }

        /// <summary>
        /// Tenant id
        /// </summary>
        private Guid? CurrentUserTenantId
        {
            get
            {
                var tenantId = User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value?.ToGuid();
                if (tenantId != null) return tenantId;
                var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                if (user == null) return null;
                _userManager.AddClaimAsync(user, new Claim("tenant", user.TenantId.ToString())).GetAwaiter()
                    .GetResult();
                return user.TenantId;
            }
        }

        /// <summary>
        /// Get row select references
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [AjaxOnly]
        [HttpGet]
        public virtual async Task<JsonResult> GetRowReferences([Required] Guid entityId, [Required] Guid propertyId)
        {
            var response = new ResultModel();
            var refProp = _pagesContext.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .FirstOrDefault(x => x.Id == entityId)?
                .TableFields?.FirstOrDefault(x => x.Id == propertyId);

            if (refProp == null)
            {
                response.Errors = new List<IErrorModel>
                    {
                        new ErrorModel("fail", "Property reference not found")
                    };
                return Json(response);
            }

            var entityRefName = refProp.TableFieldConfigValues
                .FirstOrDefault(x => x.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingTable);
            if (entityRefName == null)
            {
                response.Errors = new List<IErrorModel>
                    {
                        new ErrorModel("fail", "Property reference not found")
                    };
                return Json(response);
            }

            var displayFormat = refProp.TableFieldConfigValues
                .FirstOrDefault(x => x.TableFieldConfig.Code == TableFieldConfigCode.Reference.DisplayFormat);
            var filters = new List<Filter>
                {
                    new Filter( nameof(BaseModel.IsDeleted), false)
                };

            if (entityRefName.Value == "Users")
            {
                filters.Add(new Filter
                {
                    Value = CurrentUserTenantId,
                    Criteria = Criteria.Equals,
                    Parameter = nameof(BaseModel.TenantId)
                });
            }

            var res = await _service.GetAll(entityRefName.Value, filters: filters);
            if (res.IsSuccess)
            {
                if (displayFormat != null)
                {
                    if (!string.IsNullOrEmpty(displayFormat.Value))
                    {
                        res.Result = res.Result.Select(x =>
                        {
                            var format = displayFormat.Value.Inject(x);
                            if (x.ContainsKey("Name"))
                                x["Name"] = format;
                            else
                                x.Add("Name", format);
                            return x;
                        });
                    }
                }

                try
                {
                    res.Result = res.Result.OrderBy(x => x.ContainsKey("Name") ? x["Name"] : x);
                }
                catch
                {
                    //Ignore
                }
            }

            response.IsSuccess = res.IsSuccess;
            response.Result = new RowReferenceViewModel
            {
                Data = res.Result?.ToList(),
                EntityName = entityRefName.Value
            };

            return Json(response, _jsonSerializerSettings);
        }

        /// <summary>
        /// Delete row id
        /// </summary>
        /// <param name="viewModelId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        [AjaxOnly]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<JsonResult> DeleteItemFromDynamicEntity(Guid viewModelId, string id)
        {
            var result = new ResultModel();
            if (string.IsNullOrEmpty(id) || viewModelId == Guid.Empty)
            {
                result.Errors.Add(new ErrorModel("", "Fail to delete!"));
                return Json(result);
            }
            var viewModel = await _pagesContext.ViewModels.Include(x => x.TableModel).FirstOrDefaultAsync(x => x.Id.Equals(viewModelId));
            if (viewModel == null) return Json(result);
            var response = await _service.Table(viewModel.TableModel.Name).Delete<object>(Guid.Parse(id));
            if (!response.IsSuccess) return Json(result);
            result.IsSuccess = true;
            return Json(result);
        }

        /// <summary>
        /// Delete row id
        /// </summary>
        /// <param name="viewModelId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        [AjaxOnly]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<JsonResult> DeleteItemForeverFromDynamicEntity(Guid viewModelId, string id)
        {
            var result = new ResultModel();
            if (string.IsNullOrEmpty(id) || viewModelId == Guid.Empty)
            {
                result.Errors.Add(new ErrorModel("", "Fail to delete!"));
                return Json(result);
            }
            var viewModel = await _pagesContext.ViewModels.Include(x => x.TableModel).FirstOrDefaultAsync(x => x.Id.Equals(viewModelId));
            if (viewModel == null) return Json(result);
            var response = await _service.Table(viewModel.TableModel.Name).DeletePermanent<object>(Guid.Parse(id));
            return Json(response);
        }
    }
}