namespace GR.Core.Abstractions
{
    public interface IGearResourceProvider
    {
        /// <summary>
        /// Get app settings file path
        /// </summary>
        /// <returns></returns>
        string AppSettingsFilepath();
    }
}