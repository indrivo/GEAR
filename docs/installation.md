# Installation

The app can be started using 3 environments:
- `Development` - is used for development purpose
- `Stage` - is used on pre- production
- `Production` - is used for clients, on server side

`NOTE`: For development use Development env
`NOTE`: Each configuration corresponds to a configuration file, like this: `appsettings.{Env}.json`

Structure of appssetings file: 
```json
{
  "SystemConfig": {
    "MachineIdentifier": ".GR.Prod"
  },
  "ConnectionStrings": {
    "Provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
    "ConnectionString": "Host=127.0.0.1;Port=5432;Username=postgres;Password=Gear2019;Persist Security Info=true;Database=GEAR.PROD;MaxPoolSize=1000;"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Error"
    }
  },
  "HealthCheck": {
    "Timeout": 3,
    "Path": "/health"
  },
  "IsConfigured": true,
  "LdapSettings": {
    "ServerName": "",
    "ServerPort": 389,
    "UseSSL": false,
    "Credentials": {
      "DomainUserName": "",
      "Password": ""
    },
    "SearchBase": "",
    "ContainerName": "",
    "DomainName": "",
    "DomainDistinguishedName": ""
  },
  "WebClients": {
    "CORE": {
      "uri": "http://159.69.195.160:6969"
    },
    "BPMApi": {
      "uri": "http://159.69.195.160:6969"
    }
  },
  "RedisConnection": {
    "Host": "127.0.0.1",
    "Port": "6379"
  },
  "BackupSettings": {
    "Enabled": false,
    "UsePostGreSql": false,
    "UseMsSql": false,
    "BackupFolder": "AppName",
    "Interval": "24",
    "PostGreSqlBackupSettings": {
      "PgDumpPath": "C:\\Program Files\\PostgreSQL\\11\\bin\\pg_dump.exe",
      "Host": "localhost",
      "Port": "5432",
      "User": "postgres",
      "Password": "1111",
      "Database": "AppName.PROD",
      "FileExtension": "pgbackup"
    },
    "MsSqlBackupSettings": {
    }
  },
  "EmailSettings": {
    "Enabled": true,
    "Host": "smtp.office365.com",
    "Port": 587,
    "Timeout": 5000,
    "EnableSsl": true,
    "NetworkCredential": {
      "Email": "",
      "Password": ""
    }
  },
  "Sentry": {
    "Dsn": "https://a898fb5130514f2485704835f8109591@sentry.io/1547729",
    "IncludeRequestPayload": true,
    "SendDefaultPii": true,
    "MinimumBreadcrumbLevel": "Debug",
    "MinimumEventLevel": "Warning",
    "AttachStackTrace": true,
    "Debug": true,
    "DiagnosticsLevel": "Error"
  }
}
```

### Explanation of appsettings blocks: 
- `SystemConfig` - represent a general section that provide some global info:
    - MachineIdentifier - is used for identify the app id, if is installed multiple GEAR apps, after app installation it is ovverided by generated string
- `ConnectionStrings` - represent databases providers configuration
    - Provider - default is postgres
        - Npgsql.EntityFrameworkCore.PostgreSQL - postgres ,enabled and default
        - Microsoft.EntityFrameworkCore.SqlServer - enabled
        - Microsoft.EntityFrameworkCore.Sqlite - for the future
        - Microsoft.EntityFrameworkCore.InMemory - for the future
        - Microsoft.EntityFrameworkCore.Cosmos - for the future
        - Pomelo.EntityFrameworkCore.MySql - for the future
        - Pomelo.EntityFrameworkCore.MyCat - for the future
        - EntityFrameworkCore.SqlServerCompact40 - for the future
        - EntityFrameworkCore.SqlServerCompact35 - for the future
        - EntityFrameworkCore.Jet - for the future
        - MySql.Data.EntityFrameworkCore - for the future
        - FirebirdSql.EntityFrameworkCore.Firebird - for the future
        - EntityFrameworkCore.FirebirdSql - for the future
        - IBM.EntityFrameworkCore - for the future
        - EntityFrameworkCore.OpenEdge - for the future
- [Logging] - see microsoft docs
- `LocalizationConfig` - language configurations
- `IsConfigured` - This property determines whether the app has been installed or not, if set to true then the configurations set in the database are taken, otherwise when accessing any page, it will be redirected to the installer
- `LdapSettings` - This involves configuring the AD mode
- `RedisConnection` - configurations for distributed cache
    - Host - represent the ip address of redis connection
    - Port - represent the port where is bind redis service, default: 6379
- `BackupSettings` - this section is used for backup module, now is developed only for postgres provider
- `EmailSettings` - this section is used for email client
    - Enabled - set active or inactivity of service
    - Host - the smptp host
    - Port - the port of smtp
    - Timeout - represents the time allowed for the service to wait for the message to be successfully sent
    - EnableSsl - represent usage of smtp with ssl
    - NetworkCredential
        - Email - existent smtp email
        - Password - the password of smtp email
- `Sentry` - consult [sentry documentation](https://docs.sentry.io/platforms/dotnet/aspnetcore/) for .net core 

## App run
To start the app, you need:
1. Restore ui packages on all razor projects (is optional step because, they are restored on build)
    ```shell
    libman restore
    ```
2. Restore C# nuget packages by typing 
    ```shell
    dotnet restore
    ```
3. Build. To build, you must navigate the explorer to the path: `./src/GR.WebHosts/GR.Cms` or 
    ```shell
    cd ./src/GR.WebHosts/GR.Cms
    ```
    after it execute the following command: 
    ```shell
    dotnet build
    ```
4. If build has run successfully, it is the green wave to start the project
    ```shell
    dotnet run 
    ```
    `optional` for change exposed port 
    ```shell
    dotnet run --urls=http://localhost:5001/ 
    ```

## Install steps
`Note`: Be sure that in appsettings{Env}.json, the IsConfigured property is set to false
1. Start the application
You will be met by the following message describing the platform
![Welcome board](https://i.ibb.co/5GWdW6N/welcome-gear.png)
Click on `Go to installation`

2. Configure admin profile
![Profile tab](https://i.ibb.co/nQw9kHK/profile-gear.png)
Settings:
- `User Name` - adminstrator user name
- `Email` - your email address to receive emails on system events
- `Password` and `Confirm Password` - the administrator password
- `First Name` - admin first name
- `Last Name` - admin last name
- `Organization Name` - represent the default organization name
3. Set up database provider
![Configuration of database provider](https://i.ibb.co/hMnP7y6/db-gear.png)
`Note`: Use postgres default, because MsSql has not been tested for a long time, we plan support for other providers
`Connection String example`: Host=127.0.0.1;Port=5432;Username=postgres;Password=Gear2020;Persist Security Info=true;Database=Gear.PROD;MaxPoolSize=1000;
4. Press `Install` button and wait until the system is installed