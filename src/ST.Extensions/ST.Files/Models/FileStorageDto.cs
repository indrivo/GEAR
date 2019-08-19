using ST.Files.Abstraction.Models.Dto;

namespace ST.Files.Models
{
    public sealed class FileStorageDto : FileDto
    {
        /// <summary>
        /// Byte array of file
        /// </summary>
        public byte[] EncryptedFile { get; set; }
    }
}
