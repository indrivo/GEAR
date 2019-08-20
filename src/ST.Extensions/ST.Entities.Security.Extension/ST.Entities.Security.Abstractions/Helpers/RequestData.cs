using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions.Helpers;

namespace ST.Entities.Security.Abstractions.Helpers
{
    public class RequestData
    {
        public string EntityName { get; set; }
        public IEnumerable<Filter> Filters { get; set; } = new List<Filter>();
        public string Object { get; set; }
        public static JsonResult InvalidRequest => new JsonResult(new ResultModel
        {
            Errors = new List<IErrorModel>
            {
                new ErrorModel("invalid_data", "Invalid data!")
            }
        });
    }
}
