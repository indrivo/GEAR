using GR.Files.Abstraction.Models.Dto;

namespace GR.Files.Box.Models
{
    public sealed class FileBoxDto : FileDto
    {
        /// <summary>
        /// String path
        /// </summary>
        public string Path { get; set; }
    }
}
