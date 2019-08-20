using ST.Files.Abstraction.Models.Dto;

namespace ST.Files.Box.Models
{
    public sealed class FileBoxDto : FileDto
    {
        /// <summary>
        /// String path
        /// </summary>
        public string Path { get; set; }
    }
}
