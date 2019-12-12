using GR.Files.Abstraction.Models.Dto;

namespace GR.Files.Models
{
    public sealed class FileStorageDto : FileDto
    {
        /// <summary>
        /// Byte array of file
        /// </summary>
        public byte[] EncryptedFile { get; set; }
    }
}
