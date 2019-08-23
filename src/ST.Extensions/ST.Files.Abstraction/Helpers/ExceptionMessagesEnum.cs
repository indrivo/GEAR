using System.ComponentModel;

namespace ST.Files.Abstraction.Helpers
{
    /// <summary>
    /// File module exception messages
    /// </summary>
    public enum ExceptionMessagesEnum
    {
        [Description("File Not Found")] FileNotFound,

        [Description("IFormFile is null")] NullIFormFile,

        [Description("There was a error on saving file ")]
        FileNotSaved,
    }
}
