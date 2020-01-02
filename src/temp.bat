::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Configuration-------------------------------------------------
::-----------------------------------------------------------------------------------------------------------

SET pushKey="oy2cdyotfbd5rk7thnias2paqkriagtuxahd5t2n3woexy"
SET pushHost="https://www.nuget.org"



::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Pack projects-------------------------------------------------
::-----------------------------------------------------------------------------------------------------------

dotnet build ../GR.sln

::Pack core modules
dotnet pack ./GR.Extensions/GR.Core.Extension/GR.Core/GR.Core.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Core.Extension/GR.Core.Razor/GR.Core.Razor.csproj -o ../../../nupkgs

::Pack application modules
dotnet pack ./GR.Extensions/GR.Application.Extension/GR.WebApplication/GR.WebApplication.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Application.Extension/GR.Application.Middleware/GR.Application.Middleware.csproj -o ../../../nupkgs

::Pack cache modules
dotnet pack ./GR.Extensions/GR.Cache.Extension/GR.Cache.Abstractions/GR.Cache.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Cache.Extension/GR.Cache/GR.Cache.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Cache.Extension/GR.Cache.Razor/GR.Cache.Razor.csproj -o ../../../nupkgs

::Pack calendar modules
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Abstraction/GR.Calendar.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar/GR.Calendar.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Razor/GR.Calendar.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.NetCore.Api.GraphQL/GR.Calendar.NetCore.Api.GraphQL.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Abstractions.ExternalProviders/GR.Calendar.Abstractions.ExternalProviders.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Providers.Google/GR.Calendar.Providers.Google.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Providers.Outlook/GR.Calendar.Providers.Outlook.csproj -o ../../../nupkgs

:: Pack audit modules
dotnet pack ./GR.Extensions/GR.Audit.Extension/GR.Audit.Abstractions/GR.Audit.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Audit.Extension/GR.Audit/GR.Audit.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Audit.Extension/GR.Audit.Razor/GR.Audit.Razor.csproj -o ../../../nupkgs


:: Pack notification modules
dotnet pack ./GR.Extensions/GR.Notifications.Extension/GR.Notifications.Abstractions/GR.Notifications.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Notifications.Extension/GR.Notifications/GR.Notifications.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Notifications.Extension/GR.Notifications.Razor/GR.Notifications.Razor.csproj -o ../../../nupkgs


:: Pack localization modules
dotnet pack ./GR.Extensions/GR.Localization.Extension/GR.Localization.Abstractions/GR.Localization.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Localization.Extension/GR.Localization.Razor/GR.Localization.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Localization.Extension/GR.Localization/GR.Localization.csproj -o ../../../nupkgs


:: Pack task manager modules
dotnet pack ./GR.Extensions/GR.TaskManager.Extension/GR.TaskManager.Abstractions/GR.TaskManager.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.TaskManager.Extension/GR.TaskManager.Razor/GR.TaskManager.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.TaskManager.Extension/GR.TaskManager/GR.TaskManager.csproj -o ../../../nupkgs

:: Pack commerce modules
dotnet pack ./GR.Extensions/GR.ECommerce.Extension/GR.ECommerce.Abstractions/GR.ECommerce.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.ECommerce.Extension/GR.ECommerce.Products/GR.ECommerce.Products.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.ECommerce.Extension/GR.ECommerce.Razor/GR.ECommerce.Razor.csproj -o ../../../nupkgs

:: Pack dashboard modules
dotnet pack ./GR.Extensions/GR.Dashboard.Extension/GR.Dashboard.Abstractions/GR.Dashboard.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Dashboard.Extension/GR.Dashboard/GR.Dashboard.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Dashboard.Extension/GR.Dashboard.Razor/GR.Dashboard.Razor.csproj -o ../../../nupkgs

:: Pack report modules
dotnet pack ./GR.Extensions/GR.Report.Extension/GR.Report.Abstractions/GR.Report.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Report.Extension/GR.Report.Dynamic/GR.Report.Dynamic.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Report.Extension/GR.Report.Dynamic.Razor/GR.Report.Dynamic.Razor.csproj -o ../../../nupkgs


:: Pack mail modules
dotnet pack ./GR.Extensions/GR.Email.Extension/GR.Email.Abstractions/GR.Email.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Email.Extension/GR.Email/GR.Email.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Email.Extension/GR.Email.Razor/GR.Email.Razor.csproj -o ../../../nupkgs


:: Pack multi-tenant modules
dotnet pack ./GR.Extensions/GR.MultiTenant.Extension/GR.MultiTenant.Abstractions/GR.MultiTenant.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.MultiTenant.Extension/GR.MultiTenant/GR.MultiTenant.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.MultiTenant.Extension/GR.MultiTenant.Razor/GR.MultiTenant.Razor.csproj -o ../../../nupkgs


:: Pack state machine modules
dotnet pack ./GR.Extensions/GR.WorkFlow.Extension/GR.WorkFlows.Abstractions/GR.WorkFlows.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.WorkFlow.Extension/GR.WorkFlows/GR.WorkFlows.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.WorkFlow.Extension/GR.WorkFlows.Razor/GR.WorkFlows.Razor.csproj -o ../../../nupkgs

:: Pack identity modules
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Abstractions/GR.Identity.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity/GR.Identity.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Groups.Razor/GR.Identity.Groups.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Razor.Users/GR.Identity.Razor.Users.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Roles.Razor/GR.Identity.Roles.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Razor/GR.Identity.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.IdentityServer4/GR.Identity.IdentityServer4.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.LdapAuth/GR.Identity.LdapAuth.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.LdapAuth.Abstractions/GR.Identity.LdapAuth.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Permissions.Abstractions/GR.Identity.Permissions.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Permissions/GR.Identity.Permissions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Profile.Abstractions/GR.Identity.Profile.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Profile/GR.Identity.Profile.csproj -o ../../../nupkgs

:: Pack backup modules
dotnet pack ./GR.Extensions/GR.Backup.Extension/GR.Backup.Abstractions/GR.Backup.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Backup.Extension/GR.Backup.PostgresSql/GR.Backup.PostgresSql.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Backup.Extension/GR.Backup.Razor/GR.Backup.Razor.csproj -o ../../../nupkgs

:: Pack dynamic entities repos modules
dotnet pack ./GR.Extensions/GR.DynamicEntityStorage.Extension/GR.DynamicEntityStorage.Abstractions/GR.DynamicEntityStorage.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.DynamicEntityStorage.Extension/GR.DynamicEntityStorage/GR.DynamicEntityStorage.csproj -o ../../../nupkgs

:: Pack entities modules
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities/GR.Entities.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.Abstractions/GR.Entities.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.Razor/GR.Entities.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.EntityBuilder.MsSql/GR.Entities.EntityBuilder.MsSql.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.EntityBuilder.Postgres/GR.Entities.EntityBuilder.Postgres.csproj -o ../../../nupkgs

:: Pack entities security modules
dotnet pack ./GR.Extensions/GR.Entities.Security.Extension/GR.Entities.Security/GR.Entities.Security.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Security.Extension/GR.Entities.Security.Abstractions/GR.Entities.Security.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Security.Extension/GR.Entities.Security.Razor/GR.Entities.Security.Razor.csproj -o ../../../nupkgs


:: Pack mail modules
dotnet pack ./GR.Extensions/GR.Forms.Extension/GR.Forms.Abstractions/GR.Forms.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Forms.Extension/GR.Forms/GR.Forms.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Forms.Extension/GR.Forms.Razor/GR.Forms.Razor.csproj -o ../../../nupkgs

:: Pack install modules
dotnet pack ./GR.Extensions/GR.Install.Extension/GR.Install.Abstractions/GR.Install.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Install.Extension/GR.Install/GR.Install.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Install.Extension/GR.Install.Razor/GR.Install.Razor.csproj -o ../../../nupkgs

:: Pack proces modules
dotnet pack ./GR.Extensions/GR.Processes.Extension/GR.Processes.Abstractions/GR.Processes.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Processes.Extension/GR.Processes/GR.Procesess.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Processes.Extension/GR.Process.Razor/GR.Process.Razor.csproj -o ../../../nupkgs

:: Pack page render modules
dotnet pack ./GR.Extensions/GR.Render.Extension/GR.PageRender.Abstractions/GR.PageRender.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Render.Extension/GR.PageRender/GR.PageRender.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Render.Extension/GR.PageRender.Razor/GR.PageRender.Razor.csproj -o ../../../nupkgs


PAUSE