using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Dashboard.Abstractions.Models.Permissions
{
    public class RowWidgetAcl : RowWidgetAclBase
    {
        /// <summary>
        /// Row id
        /// </summary>
        [Required]
        public virtual Guid RowId { get; set; }

        /// <summary>
        /// WidgetId
        /// </summary>
        [Required]
        public virtual Guid WidgetId { get; set; }
    }

    public class RowWidgetAclBase
    {
        /// <summary>
        /// Role id
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Allow
        /// </summary>
        public bool Allow { get; set; }
    }
}
