using ST.Files.Abstraction.Models.Dto;

namespace ST.Files.Models
{
    public class FileStorageDto : FileDto
    {
        /// <summary>
        /// Byte array of file
        /// </summary>
        public byte[] EncryptedFile { get; set; }
    }
}
