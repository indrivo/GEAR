# Audit Module

## Description
This module is used to log any data changes of database records. It is custom used for each entity: entities can be registered, can chose  that fields to track and that to ignore. 
`Note`: Audit is compatible only with models that inherit from `BaseModel` class or implement `IBase<T>`
This module has 2 tables that store your data changes:
1. `public DbSet<TrackAudit> TrackAudits { get; set; }`
2. `public DbSet<TrackAuditDetails> TrackAuditDetails { get; set; }`

## Installation
In your startup class, register the service:
```csharp
using GR.Audit;
using GR.Audit.Abstractions.Extensions;
...
//----------------------------------------Audit Module-------------------------------------
            config.GearServices.AddAuditModule<AuditManager>();
...
```

## Usage
For use this module is need to install GR.Audit nuget package or download the library from github.
In a few steps we can log our data changes.
1. Your module DbContext must inherit from `TrackerDbContext`
Example of usage:
```csharp
using GR.Audit.Contexts;
...
 public class GroupsDbContext : TrackerDbContext, IGroupContext
 {
 }
```
2. Your model must have attached the `TrackEntity` attribute like in an examples below:
Track all fields:
```csharp
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
...

[TrackEntity(Option = TrackEntityOption.AllFields)]
    public class GearUser : IdentityUser<Guid>, IBase<Guid>
```  

Track only selected fields:
```csharp
TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class GearUser : IdentityUser<Guid>, IBase<Guid>
    {
        /// <summary>
        /// Stores user first name
        /// </summary>
        [MaxLength(50)]
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Stores user last name
        /// </summary>
        [MaxLength(50)]
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual string LastName { get; set; }
```
3. Register the context in your `IServiceCollection` extensions
Example: 
```csharp
using GR.Audit.Abstractions.Extensions;
...
services.RegisterAuditFor<IIdentityContext>("Identity module");
```

If your context has early created migrations, after use the audit module, is needed to create new migrations. For docs on how to create migrations with EF Core, you can find [here](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli))

For extend `IdentityContext` you can use the `TrackerIdentityDbContext` context
Example:
```csharp
 public class GearIdentityDbContext : TrackerIdentityDbContext<GearUser, GearRole, Guid>, IPermissionsContext
 {}
```

Thanks of `BaseModel` we can update fields of record:
- `Created` - DateTime - the date of  record creation
- `Changed` - DateTime - the date of record modification
- `Author` - user name of user that has created this record
- `ModifiedBy` - string - the user name of user
- `Version` - int - the version of record, is incremented on each change of record

Example on how to use the `BaseModel` class on your entity
```csharp
  [DebuggerDisplay(@"\{{" + nameof(Name) + @",nq}\}")]
    [TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class Group : BaseModel
    {
    }
```

The audit is saved only after called SaveChanges(), SaveChangesAsync or Push(), PushAsync() context methods.

To ignore save audit of record, set to true the `DisableAuditTracking` field, this field is not saved and not serialized.
Example:
```csharp
var user = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
user.LastLogin = DateTime.Now;
user.DisableAuditTracking = true;
await _userManager.UserManager.UpdateAsync(user);
```
In showed example save audit on change last login date for user is disabled.
