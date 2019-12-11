using GR.Files.Abstraction.Models.ViewModels;

namespace GR.Files.Box.Abstraction.Models.ViewModels
{
    public sealed class FileBoxSettingsViewModel : FileSettingsViewModel
    {
        /// <summary>
        /// Directory for saving files
        /// </summary>
        public string Path { get; set; }
    }
}
