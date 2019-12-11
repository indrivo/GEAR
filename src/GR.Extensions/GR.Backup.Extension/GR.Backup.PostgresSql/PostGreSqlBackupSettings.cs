using GR.Backup.Abstractions.Models;

namespace GR.Backup.PostGresSql
{
    public sealed class PostGreSqlBackupSettings : BackupSettings
    {
        /// <summary>
        /// pg_dump.exe path
        /// </summary>
        public string PgDumpPath { get; set; }

        /// <summary>
        /// PostgresSql host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public string Port { get; set; } = "5432";

        /// <summary>
        /// User
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Chose database
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        public string FileExtension { get; set; } = "pgbackup";
    }
}
