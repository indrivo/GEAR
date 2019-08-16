using System.ComponentModel;

namespace ST.Files.Abstraction.Utils
{
    /// <summary>
    /// File module exception messages
    /// </summary>
    public enum ExceptionMessagesEnum
    {
        [Description("File Not Found")] FileNotFound = 0,

        [Description("IFormFile is null")] NullIFormFile = 0,
    }
}
