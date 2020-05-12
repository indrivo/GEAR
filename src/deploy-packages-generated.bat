SET pushKey=""
SET pushHost="https://www.nuget.org"



:: Pack projects
dotnet build ../GR.sln
dotnet pack ./GR.Extensions/GR.Application.Extension/GR.WebApplication/GR.WebApplication.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Audit.Extension/GR.Audit/GR.Audit.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Audit.Extension/GR.Audit.Abstractions/GR.Audit.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Audit.Extension/GR.Audit.Razor/GR.Audit.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Backup.Extension/GR.Backup.Abstractions/GR.Backup.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Backup.Extension/GR.Backup.PostgresSql/GR.Backup.PostgresSql.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Backup.Extension/GR.Backup.Razor/GR.Backup.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Cache.Extension/GR.Cache/GR.Cache.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Cache.Extension/GR.Cache.Abstractions/GR.Cache.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Cache.Extension/GR.Cache.Razor/GR.Cache.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar/GR.Calendar.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Abstraction/GR.Calendar.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Abstractions.ExternalProviders/GR.Calendar.Abstractions.ExternalProviders.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.NetCore.Api.GraphQL/GR.Calendar.NetCore.Api.GraphQL.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Providers.Google/GR.Calendar.Providers.Google.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Providers.Outlook/GR.Calendar.Providers.Outlook.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Calendar.Extensions/GR.Calendar.Razor/GR.Calendar.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Core.Extension/GR.Core/GR.Core.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Core.Extension/GR.Core.Razor/GR.Core.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Core.Extension/GR.Core.UI.Razor.DefaultTheme/GR.Core.UI.Razor.DefaultTheme.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Dashboard.Extension/GR.Dashboard/GR.Dashboard.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Dashboard.Extension/GR.Dashboard.Abstractions/GR.Dashboard.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Dashboard.Extension/GR.Dashboard.Razor/GR.Dashboard.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Documents.Extension/GR.Documents/GR.Documents.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Documents.Extension/GR.Documents.Abstractions/GR.Documents.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Documents.Extension/GR.Documents.Razor/GR.Documents.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.DynamicEntityStorage.Extension/GR.DynamicEntityStorage/GR.DynamicEntityStorage.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.DynamicEntityStorage.Extension/GR.DynamicEntityStorage.Abstractions/GR.DynamicEntityStorage.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Abstractions/GR.ECommerce.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Infrastructure/GR.ECommerce.Infrastructure.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Orders/GR.Orders/GR.Orders.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Orders/GR.Orders.Abstractions/GR.Orders.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Orders/GR.Orders.Razor/GR.Orders.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.ECommerce.Payments.Abstractions/GR.ECommerce.Payments.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.Braintree/GR.Braintree/GR.Braintree.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.Braintree/GR.Braintree.Abstractions/GR.Braintree.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.Braintree/GR.Braintree.Razor/GR.Braintree.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.MobilPay/GR.MobilPay/GR.MobilPay.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.MobilPay/GR.MobilPay.Abstractions/GR.MobilPay.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.MobilPay/GR.MobilPay.Razor/GR.MobilPay.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.Paypal/GR.Paypal/GR.Paypal.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.Paypal/GR.Paypal.Abstractions/GR.Paypal.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Payments/GR.Providers/GR.ECommerce.Paypal/GR.Paypal.Razor/GR.Paypal.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Razor/GR.ECommerce.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Subscription/GR.Subscription/GR.Subscriptions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Subscription/GR.Subscription.Abstractions/GR.Subscriptions.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Ecommerce.Extension/GR.ECommerce.Subscription/GR.Subscriptions.Razor/GR.Subscriptions.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Email.Extension/GR.Email/GR.Email.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Email.Extension/GR.Email.Abstractions/GR.Email.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Email.Extension/GR.Email.Razor/GR.Email.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities/GR.Entities.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.Abstractions/GR.Entities.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.EntityBuilder.MsSql/GR.Entities.EntityBuilder.MsSql.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.EntityBuilder.Postgres/GR.Entities.EntityBuilder.Postgres.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Extension/GR.Entities.Razor/GR.Entities.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Security.Extension/GR.Entities.Security/GR.Entities.Security.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Security.Extension/GR.Entities.Security.Abstractions/GR.Entities.Security.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Entities.Security.Extension/GR.Entities.Security.Razor/GR.Entities.Security.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Files/GR.Files.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Files.Abstraction/GR.Files.Abstraction.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Files.Box/GR.Files.Box.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Files.Box.Abstraction/GR.Files.Box.Abstraction.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Files.Razor/GR.Files.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Forms.Extension/GR.Forms/GR.Forms.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Forms.Extension/GR.Forms.Abstractions/GR.Forms.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Forms.Extension/GR.Forms.Razor/GR.Forms.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.AccountActivity.Abstractions/GR.AccountActivity.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.AccountActivity.Impl/GR.AccountActivity.Impl.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.AccountActivity.Razor/GR.AccountActivity.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity/GR.Identity.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Abstractions/GR.Identity.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Clients.Abstractions/GR.Identity.Clients.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Clients.Infrastructure/GR.Identity.Clients.Infrastructure.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Clients.Razor/GR.Identity.Clients.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Groups.Abstractions/GR.Identity.Groups.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Groups.Api/GR.Identity.Groups.Api.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Groups.Infrastructure/GR.Identity.Groups.Infrastructure.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Groups.Razor/GR.Identity.Groups.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.LdapAuth/GR.Identity.LdapAuth.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.LdapAuth.Abstractions/GR.Identity.LdapAuth.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Permissions/GR.Identity.Permissions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Permissions.Abstractions/GR.Identity.Permissions.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.PhoneVerification.Abstractions/GR.Identity.PhoneVerification.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.PhoneVerification.Api/GR.Identity.PhoneVerification.Api.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.PhoneVerification.Infrastructure/GR.Identity.PhoneVerification.Infrastructure.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Profile/GR.Identity.Profile.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Profile.Abstractions/GR.Identity.Profile.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Profile.Api/GR.Identity.Profile.Api.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Profile.Razor/GR.Identity.Profile.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Razor/GR.Identity.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Roles.Razor/GR.Identity.Roles.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.Identity.Users.Razor/GR.Identity.Users.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.UserPreferences.Abstractions/GR.UserPreferences.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.UserPreferences.Api/GR.UserPreferences.Api.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Identity.Extension/GR.UserPreferences.Impl/GR.UserPreferences.Impl.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Install.Extension/GR.Install/GR.Install.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Install.Extension/GR.Install.Abstractions/GR.Install.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Install.Extension/GR.Install.Razor/GR.Install.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Localization.Extension/GR.Localization/GR.Localization.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Localization.Extension/GR.Localization.Abstractions/GR.Localization.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Localization.Extension/GR.Localization.Api/GR.Localization.Api.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Localization.Extension/GR.Localization.Razor/GR.Localization.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Logger.Extension/GR.Logger/GR.Logger.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Logger.Extension/GR.Logger.Abstractions/GR.Logger.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Logger.Extension/GR.Logger.Razor/GR.Logger.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Modules.Extension/GR.Modules.Abstractions/GR.Modules.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Modules.Extension/GR.Modules.Api/GR.Modules.Api.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Modules.Extension/GR.Modules.Infrastructure/GR.Modules.Infrastructure.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.MultiTenant.Extension/GR.MultiTenant/GR.MultiTenant.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.MultiTenant.Extension/GR.MultiTenant.Abstractions/GR.MultiTenant.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.MultiTenant.Extension/GR.MultiTenant.Razor/GR.MultiTenant.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Notifications.Extension/GR.Notifications/GR.Notifications.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Notifications.Extension/GR.Notifications.Abstractions/GR.Notifications.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Notifications.Extension/GR.Notifications.Hub/GR.Notifications.Hub.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Notifications.Extension/GR.Notifications.Razor/GR.Notifications.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Processes.Extension/GR.Process.Razor/GR.Process.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Processes.Extension/GR.Processes/GR.Procesess.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Processes.Extension/GR.Processes.Abstractions/GR.Processes.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Render.Extension/GR.PageRender/GR.PageRender.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Render.Extension/GR.PageRender.Abstractions/GR.PageRender.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Render.Extension/GR.PageRender.Razor/GR.PageRender.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Report.Extension/GR.Report.Abstractions/GR.Report.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Report.Extension/GR.Report.Dynamic/GR.Report.Dynamic.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.Report.Extension/GR.Report.Dynamic.Razor/GR.Report.Dynamic.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.TaskManager.Extension/GR.TaskManager/GR.TaskManager.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.TaskManager.Extension/GR.TaskManager.Abstractions/GR.TaskManager.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.TaskManager.Extension/GR.TaskManager.Razor/GR.TaskManager.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.UI.Menu.Extension/GR.UI.Menu/GR.UI.Menu.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.UI.Menu.Extension/GR.UI.Menu.Abstractions/GR.UI.Menu.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.UI.Menu.Extension/GR.UI.Menu.Razor/GR.UI.Menu.Razor.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.WorkFlow.Extension/GR.WorkFlows/GR.WorkFlows.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.WorkFlow.Extension/GR.WorkFlows.Abstractions/GR.WorkFlows.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.WorkFlow.Extension/GR.WorkFlows.Api/GR.WorkFlows.Api.csproj -o ../../../nupkgs
dotnet pack ./GR.Extensions/GR.WorkFlow.Extension/GR.WorkFlows.Razor/GR.WorkFlows.Razor.csproj -o ../../../nupkgs



:: Push projects
cd ./nupkgs
dotnet nuget push -k %pushKey% -s %pushHost% GR.WebApplication*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Audit*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Audit.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Audit.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Backup.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Backup.PostgresSql*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Backup.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Cache*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Cache.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Cache.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Abstractions.ExternalProviders*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.NetCore.Api.GraphQL*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Providers.Google*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Providers.Outlook*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Core*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Core.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Core.UI.Razor.DefaultTheme*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Dashboard*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Dashboard.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Dashboard.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Documents*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Documents.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Documents.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.DynamicEntityStorage*
dotnet nuget push -k %pushKey% -s %pushHost% GR.DynamicEntityStorage.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.ECommerce.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.ECommerce.Infrastructure*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Orders*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Orders.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Orders.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.ECommerce.Payments.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Braintree*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Braintree.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Braintree.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.MobilPay*
dotnet nuget push -k %pushKey% -s %pushHost% GR.MobilPay.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.MobilPay.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Paypal*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Paypal.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Paypal.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.ECommerce.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Subscriptions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Subscriptions.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Subscriptions.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Email*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Email.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Email.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities.EntityBuilder.MsSql*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities.EntityBuilder.Postgres*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities.Security*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities.Security.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Entities.Security.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Files*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Files.Abstraction*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Files.Box*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Files.Box.Abstraction*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Files.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Forms*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Forms.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Forms.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.AccountActivity.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.AccountActivity.Impl*
dotnet nuget push -k %pushKey% -s %pushHost% GR.AccountActivity.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Clients.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Clients.Infrastructure*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Clients.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Groups.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Groups.Api*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Groups.Infrastructure*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Groups.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.LdapAuth*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.LdapAuth.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Permissions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Permissions.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.PhoneVerification.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.PhoneVerification.Api*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.PhoneVerification.Infrastructure*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Profile*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Profile.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Profile.Api*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Profile.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Roles.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Identity.Users.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.UserPreferences.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.UserPreferences.Api*
dotnet nuget push -k %pushKey% -s %pushHost% GR.UserPreferences.Impl*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Install*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Install.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Install.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Localization*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Localization.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Localization.Api*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Localization.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Logger*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Logger.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Logger.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Modules.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Modules.Api*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Modules.Infrastructure*
dotnet nuget push -k %pushKey% -s %pushHost% GR.MultiTenant*
dotnet nuget push -k %pushKey% -s %pushHost% GR.MultiTenant.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.MultiTenant.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Notifications*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Notifications.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Notifications.Hub*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Notifications.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Process.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Procesess*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Processes.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.PageRender*
dotnet nuget push -k %pushKey% -s %pushHost% GR.PageRender.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.PageRender.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Report.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Report.Dynamic*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Report.Dynamic.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.TaskManager*
dotnet nuget push -k %pushKey% -s %pushHost% GR.TaskManager.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.TaskManager.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.UI.Menu*
dotnet nuget push -k %pushKey% -s %pushHost% GR.UI.Menu.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.UI.Menu.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.WorkFlows*
dotnet nuget push -k %pushKey% -s %pushHost% GR.WorkFlows.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.WorkFlows.Api*
dotnet nuget push -k %pushKey% -s %pushHost% GR.WorkFlows.Razor*



::Clean
cd ..
rmdir /q /s "nupkgs"
PAUSE
