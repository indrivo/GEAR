using System;
using GR.Core.Extensions;
using GR.Core.Helpers.Filters.Enums;

namespace GR.Core.Helpers.Filters
{
    public class Filter
    {
        public Filter()
        {

        }

        public Filter(string parameter, object value, Criteria criteria = Criteria.Equals)
        {
            Parameter = parameter;
            Value = value;
            Criteria = criteria;
            SearchValue = value?.ToString();
        }

        public Criteria Criteria { get; set; } = Criteria.Equals;
        public FilterNextOperator NextOperator { get; set; } = FilterNextOperator.And;
        public string Parameter { get; set; }
        public object Value { get; set; }
        public string SearchValue { get; set; }

        /// <summary>
        /// Adapt types
        /// </summary>
        public void AdaptTypes()
        {
            if (Value == null) return;
            if (!Value.ToString().IsGuid()) return;
            Guid.TryParse(Value?.ToString(), out var val);
            Value = val;
        }

        public void SetValue()
        {
            Value = SearchValue;
        }
    }
}
