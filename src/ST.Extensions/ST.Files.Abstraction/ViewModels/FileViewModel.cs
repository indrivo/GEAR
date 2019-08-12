using System;
using Microsoft.AspNetCore.Http;

namespace ST.Files.Abstraction.ViewModels
{
    public class FileViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Hash
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// FileExtension
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; set; }
    }
}