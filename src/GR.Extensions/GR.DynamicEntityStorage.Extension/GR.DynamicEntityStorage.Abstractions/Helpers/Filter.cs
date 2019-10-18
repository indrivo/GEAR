using System;
using GR.Core.Extensions;
using GR.DynamicEntityStorage.Abstractions.Enums;

namespace GR.DynamicEntityStorage.Abstractions.Helpers
{
    public class Filter
    {
        public Filter()
        {

        }

        public Filter(string parameter, object value, Criteria criteria = Criteria.Equals)
        {
            this.Parameter = parameter;
            this.Value = value;
            this.Criteria = criteria;
        }

        public Criteria Criteria { get; set; } = Criteria.Equals;
        public string Parameter { get; set; }
        public object Value { get; set; }

        /// <summary>
        /// Adapt types
        /// </summary>
        public void AdaptTypes()
        {
            if (Value != null)
            {
                if (Value.ToString().IsGuid())
                {
                    Guid.TryParse(Value?.ToString(), out var val);
                    Value = val;
                }
            }
        }
    }
}
