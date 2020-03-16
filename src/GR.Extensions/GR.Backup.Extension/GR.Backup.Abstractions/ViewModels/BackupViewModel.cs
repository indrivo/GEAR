using System;

namespace GR.Backup.Abstractions.ViewModels
{
    public sealed class BackupViewModel
    {
        /// <summary>
        /// File name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Physic file path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Backup extension
        /// </summary>
        public string Extension { get; set; }
    }
}