using System;
using System.Collections.Generic;
using System.Text;
using ST.Core;

namespace ST.Files.Abstraction.Models
{
   public class File : BaseModel
    {
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
        public byte[] Hash { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; set; }


    }
}
