using System.ComponentModel;

namespace GR.Report.Abstractions.Helpers
{
    /// <summary>
    /// Report module result messages
    /// </summary>
    public enum ResultMessagesEnum
    {
        [Description("Data saving successfully")] SaveSuccess,

        [Description("Data deleting successfully")] DeleteSuccess,

        [Description("Folder Not Found")] FolderNotFound,

        [Description("Folder is not empty")] FolderNotEmpty,

        [Description("Folder name cannot be empty")] FolderNameNullOrEmpty,

        [Description("There was a error on saving folder")] FolderNotSaved,

        [Description("There was a error on deleting folder")] FolderNotDeleted,

        [Description("Report Not Found")] ReportNotFound,

        [Description("There was a error on saving report")] ReportNotSaved,

        [Description("There was a error on deleting report")] ReportNotDeleted,

        [Description("There is no data")] EmptyResult,
    }
}
