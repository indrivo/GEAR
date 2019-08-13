using System;
using System.Collections.Generic;
using System.Text;

namespace ST.Files.Abstraction.ViewModels
{
    public class DownloadFileViewModel
    {
        /// <summary>
        /// FileExtension
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// FileExtension
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Byte array of file
        /// </summary>
        public byte[] EncryptedFile { get; set; }
    }
}
