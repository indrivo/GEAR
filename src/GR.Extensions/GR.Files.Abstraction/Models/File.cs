using GR.Core;

namespace GR.Files.Abstraction.Models
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
        public string FileExtension { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public long Size { get; set; }


    }
}
