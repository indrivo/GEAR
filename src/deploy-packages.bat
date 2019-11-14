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


::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Push projects-------------------------------------------------
::-----------------------------------------------------------------------------------------------------------

cd ./nupkgs

:: Push core modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Core*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Core.Razor*

:: Push cache modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Cache.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Cache*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Cache.Razor*


:: Push calendar modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Abstraction*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Abstractions.ExternalProviders*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Providers.Google*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.Providers.Outlook*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Calendar.NetCore.Api.GraphQL*

::Push audit modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Audit.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Audit*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Audit.Razor*


::Push audit modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Notifications.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Notifications*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Notifications.Razor*

::Push localization modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Localization.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Localization*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Localization.Razor*

::Push task manager modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.TaskManager.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.TaskManager*
dotnet nuget push -k %pushKey% -s %pushHost% GR.TaskManager.Razor*

::Push commerce modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.ECommerce.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.ECommerce.Products*
dotnet nuget push -k %pushKey% -s %pushHost% GR.ECommerce.Razor*

::Push dashboard modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Dashboard.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Dashboard*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Dashboard.Razor*

::Push report modules
dotnet nuget push -k %pushKey% -s %pushHost% GR.Report.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Report.Dynamic*
dotnet nuget push -k %pushKey% -s %pushHost% GR.Report.Razor*

::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Clean directories---------------------------------------------
::-----------------------------------------------------------------------------------------------------------
cd ..
rmdir /q /s "nupkgs"
PAUSE