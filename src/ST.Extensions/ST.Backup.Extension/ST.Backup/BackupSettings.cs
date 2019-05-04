namespace ST.Backup
{
    public sealed class BackupSettings
    {
        /// <summary>
        /// Is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Use PostGreSql
        /// </summary>
        public bool UsePostGreSql { get; set; }

        /// <summary>
        /// Use Ms Sql
        /// </summary>
        public bool UseMsSql { get; set; }

        /// <summary>
        /// This folder will be created on User Profile folder in backup folder
        /// </summary>
        public string BackupFolder { get; set; }

        /// <summary>
        /// Is measured in hours
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Postgres sql settings
        /// </summary>
        public PostGreSqlBackupSettings PostGreSqlBackupSettings { get; set; }

        /// <summary>
        /// Ms sql backup settings
        /// </summary>
        public MsSqlBackupSettings MsSqlBackupSettings { get; set; }
    }

    public sealed class PostGreSqlBackupSettings
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

    public sealed class MsSqlBackupSettings
    {

    }
}
