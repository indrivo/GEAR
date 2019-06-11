﻿using System;
using ST.PageRender.Abstractions.Models.Pages;

namespace ST.PageRender.Abstractions.Models.PagesACL
{
    public class RolePagesAcl
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

        /// <summary>
        /// Allow Access
        /// </summary>
        public bool AllowAccess { get; set; }
    }
}
