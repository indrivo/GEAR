namespace ST.Files.Abstraction.Models
{
    public sealed class FileStorage : File
    {
        /// <summary>
        /// Hash
        /// </summary>
        public byte[] Hash { get; set; }
    }
}
