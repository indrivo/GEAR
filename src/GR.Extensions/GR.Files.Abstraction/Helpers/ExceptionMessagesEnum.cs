using System.ComponentModel;

namespace GR.Files.Abstraction.Helpers
{
    /// <summary>
    /// File module exception messages
    /// </summary>
    public enum ExceptionMessagesEnum
    {
        [Description("File Not Found")] FileNotFound,

        [Description("IFormFile is null")] NullIFormFile,

        [Description("Parameter is null")] NullParameter,

        [Description("There was a error on saving file ")]
        FileNotSaved,

        [Description("File extension is not accepted")]
        InvalidExtension,

        [Description("File size is not accepted")]
        InvalidFileSize,
    }
}
