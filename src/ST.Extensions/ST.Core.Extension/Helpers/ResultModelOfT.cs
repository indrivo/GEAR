using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ST.Core.Helpers
{
    public class ResultModel<T> : IResultModel<T>
    {
        public ResultModel() =>
            Errors = new List<IErrorModel>();

        /// <summary>
        /// Bool indicating that the request resulted with success.
        /// If False than the <see cref="ErrorModel"/> will 
        /// contain a Error message that produced this error.
        /// </summary>
        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// This property will contain error keys if any.
        /// </summary>
        [JsonProperty("error_keys")]
        public ICollection<IErrorModel> Errors { get; set; }

        /// <summary>
        /// The result of the response, if there is no errors.
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonIgnore]
        public Guid? KeyEntity { get; set; }
    }
}