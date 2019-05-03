using System;

namespace ST.Backup.Exceptions
{
    public class BackupSettingsNotRegisteredException: Exception
    {
        public BackupSettingsNotRegisteredException() : base("BackUp settings are not registered in appsettings")
        {

        }
    }
}
