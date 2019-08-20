namespace ST.Files.Abstraction.Models.ViewModels
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
        /// FileExtension
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Byte array of file
        /// </summary>
        public byte[] EncryptedFile { get; set; }
    }
}
