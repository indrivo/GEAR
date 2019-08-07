using System;

namespace ST.Backup.Abstractions.Exceptions
{
    public class BackupSettingsNotRegisteredException: Exception
    {
        public BackupSettingsNotRegisteredException() : base("BackUp settings are not registered in appsettings")
        {

        }
    }
}
