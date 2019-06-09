using System;
using ST.Core;
using ST.PageRender.Abstractions.Models.Pages;

namespace ST.PageRender.Abstractions.Models.PagesACL
{
    public class RolePagesAcl : BaseModel
    {
        /// <summary>
        /// Page reference
        /// </summary>
        public Page Page { get; set; }
        public Guid PageId { get; set; }

        /// <summary>
        /// Role id
        /// </summary>
        public Guid RoleId { get; set; }
    }
}
