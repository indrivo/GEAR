using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GR.Core.Helpers.Responses;

namespace GR.Core.Helpers.Validators
{
    public static class ModelValidator
    {
        /// <summary>
        /// Check if model is valid
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ResultModel IsValid<TModel>(TModel model)
        {
            if (model == null) return new NullReferenceResultModel<object>().ToBase();
            var results = new List<ValidationResult>();
            var response = new ResultModel();

            if (Validator.TryValidateObject(model, new ValidationContext(model, null, null), results, true))
                return new SuccessResultModel<object>().ToBase();
            response.Errors = results.Select(x => new ErrorModel(string.Empty, x.ErrorMessage) as IErrorModel).ToList();
            return response;
        }

        /// <summary>
        /// Check if model is valid
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ResultModel<TOutput> IsValid<TModel, TOutput>(TModel model)
            => IsValid(model).Map<TOutput>();
    }
}