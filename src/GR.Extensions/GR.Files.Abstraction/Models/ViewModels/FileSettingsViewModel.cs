using System;
using System.Collections.Generic;
using GR.Files.Abstraction.Helpers;

namespace GR.Files.Abstraction.Models.ViewModels
{
    public class FileSettingsViewModel
    {
        /// <summary>
        /// Tenant Id
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// Max file size (mb)
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// File extensions allowed
        /// </summary>
        public string[] Extensions { get; set; }
    }
}
