using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.Entities.Models.Forms;
using ST.Entities.ViewModels.Form;

namespace ST.Entities.Services.Abstraction
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

        JsonResult GetReferenceFields(Guid? entityId, Guid? entityFieldId);

        JsonResult GetTableFields(Guid tableId);

        Task<FormCreateDetailsViewModel> GenerateFormByEntity(Guid entityId, string name, string redirectUrl,
            string headerName);
    }
}