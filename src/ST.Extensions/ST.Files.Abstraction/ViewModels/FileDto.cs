using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ST.Files.Abstraction.ViewModels
{
    public class FileDto
    {
        /// <summary>
        /// IFormFile file
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

        /// <summary>
        /// Byte array of file
        /// </summary>
        public byte[] EncryptedFile { get; set; }
    }
}
