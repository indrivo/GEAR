using ST.Files.Abstraction.Models.ViewModels;

namespace ST.Files.Box.Abstraction.Models.ViewModels
{
    public sealed class FileBoxSettingsViewModel : FileSettingsViewModel
    {
        /// <summary>
        /// Directory for saving files
        /// </summary>
        public string Path { get; set; }
    }
}
