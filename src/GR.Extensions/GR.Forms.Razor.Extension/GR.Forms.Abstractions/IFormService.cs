using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GR.Core.Helpers;
using GR.Forms.Abstractions.Models.FormModels;
using GR.Forms.Abstractions.ViewModels.FormViewModels;

namespace GR.Forms.Abstractions
{
    public interface IFormService
    {
        /// <summary>
        /// Get form by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see cref="ResultModel"/></returns>
        /// Type of <see cref="FormViewModel"/>
        ResultModel<FormViewModel> GetFormById(Guid id);

        /// <summary>
        /// Create a form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel<Guid> CreateForm(FormCreateDetailsViewModel model);

        /// <summary>
        /// Delete form by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultModel DeleteForm(Guid id);

        /// <summary>
        /// Get FormType By FormId
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        FormType GetTypeByFormId(Guid formId);

        /// <summary>
        /// Entity fields
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        JsonResult GetEntityFields(Guid tableId);

        /// <summary>
        /// Get entity reference fields
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entitySchema"></param>
        /// <returns></returns>
        JsonResult GetEntityReferenceFields(string entityName, string entitySchema);

        /// <summary>
        /// Get reference fields
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityFieldId"></param>
        /// <returns></returns>
        JsonResult GetReferenceFields(Guid? entityId, Guid? entityFieldId);

        /// <summary>
        /// Get table fields as json
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        JsonResult GetTableFields(Guid tableId);

        /// <summary>
        /// Generate form by entity and params
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="name"></param>
        /// <param name="redirectUrl"></param>
        /// <param name="headerName"></param>
        /// <returns></returns>
        Task<FormCreateDetailsViewModel> GenerateFormByEntity(Guid entityId, string name, string redirectUrl,
            string headerName);

        /// <summary>
        /// Get values for form fields 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="objDict"></param>
        /// <returns></returns>
        ResultModel<IDictionary<string, string>> GetValuesForEditForm(Form form, IDictionary<string, object> objDict);
    }
}