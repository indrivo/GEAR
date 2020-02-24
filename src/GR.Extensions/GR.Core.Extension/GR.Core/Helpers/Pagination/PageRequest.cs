
using System.Collections.Generic;
using GR.Core.Helpers.Filters.Enums;

namespace GR.Core.Helpers.Pagination
{
    public class PageRequest
    {
        /// <summary>
        /// Get deleted or not
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Page
        /// </summary>
        public virtual int Page { get; set; } = 1;

        /// <summary>
        /// Items per page
        /// </summary>
        public virtual int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort direction
        /// </summary>
        public virtual bool Descending { get; set; }

        /// <summary>
        /// Sort attribute
        /// </summary>
        public virtual string Attribute { get; set; } = nameof(BaseModel.Created);

        /// <summary>
        /// Filters
        /// </summary>
        public virtual IEnumerable<PageRequestFilter> PageRequestFilters { get; set; } = new List<PageRequestFilter>();

        /// <summary>
        /// Search on items on each object by propriety value
        /// </summary>
        public virtual string GSearch { get; set; }

        /// <summary>
        /// Search on items on each object by propriety value that match regex expression
        /// </summary>
        public virtual string RegexExpression { get; set; }
    }

    public class PageRequestFilter
    {
        /// <summary>
        /// Object propriety name
        /// </summary>
        public virtual string Propriety { get; set; }

        /// <summary>
        /// Propriety value
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// Operator
        /// </summary>
        public virtual Criteria Operator { get; set; } = Criteria.Equals;

        /// <summary>
        /// Operator for next filter
        /// </summary>
        public FilterNextOperator NextOperator { get; set; } = FilterNextOperator.And;
    }
}
