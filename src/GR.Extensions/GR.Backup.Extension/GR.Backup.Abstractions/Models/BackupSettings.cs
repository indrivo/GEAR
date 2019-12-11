namespace GR.Backup.Abstractions.Models
{
    public class BackupSettings
    {
        /// <summary>
        /// Is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// This folder will be created on User Profile folder in backup folder
        /// </summary>
        public string BackupFolder { get; set; }

        /// <summary>
        /// Is measured in hours
        /// </summary>
        public int Interval { get; set; }
    }
}
