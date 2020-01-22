using System.Collections.Generic;
using System.Linq;

namespace GR.Identity.Abstractions.Models.GroupModels
{
    public class GroupResult
    {
        private readonly List<GroupActionError> _errors = new List<GroupActionError>();

        public static GroupResult Success { get; } = new GroupResult { Succeeded = true };

        private IEnumerable<GroupActionError> Errors => _errors;

        public bool Succeeded { get; private set; }

        public static GroupResult Failed(params GroupActionError[] errors)
        {
            var result = new GroupResult { Succeeded = false };
            if (errors != null)
                result._errors.AddRange(errors);
            return result;
        }

        public override string ToString()
        {
            return Succeeded ?
                   "Succeeded" :
                   string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }
    }
}