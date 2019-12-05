using GR.Files.Abstraction.Models;

namespace GR.Files.Box.Abstraction.Models
{
    public sealed class FileBox : File
    {
        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; }
    }
}
