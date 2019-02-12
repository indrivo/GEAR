using System;
using System.Collections.Generic;
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
        /// <param name="userId"></param>
        /// <returns></returns>
        ResultModel<Guid> CreateForm(FormCreateDetailsViewModel model, string userId);

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
    }
}