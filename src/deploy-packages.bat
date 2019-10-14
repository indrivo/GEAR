::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Configuration-------------------------------------------------
::-----------------------------------------------------------------------------------------------------------

SET pushKey="oy2cdyotfbd5rk7thnias2paqkriagtuxahd5t2n3woexy"
SET pushHost="https://www.nuget.org"



::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Pack projects-------------------------------------------------
::-----------------------------------------------------------------------------------------------------------

dotnet build ../ST.GEAR.sln

::Pack Core projects
dotnet pack ./ST.Extensions/ST.Core.Extension/ST.Core/ST.Core.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Core.Extension/ST.Core.Razor/ST.Core.Razor.csproj -o ../../../nupkgs

::Pack Cache projects
dotnet pack ./ST.Extensions/ST.Cache.Extension/ST.Cache.Abstractions/ST.Cache.Abstractions.csproj -o ../../../nupkgs

::Pack calendar projects
dotnet pack ./ST.Extensions/ST.Calendar.Extensions/ST.Calendar.Abstraction/ST.Calendar.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Calendar.Extensions/ST.Calendar/ST.Calendar.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Calendar.Extensions/ST.Calendar.Razor/ST.Calendar.Razor.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Calendar.Extensions/ST.Calendar.Abstractions.ExternalProviders/ST.Calendar.Abstractions.ExternalProviders.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Calendar.Extensions/ST.Calendar.Providers.Google/ST.Calendar.Providers.Google.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Calendar.Extensions/ST.Calendar.Providers.Outlook/ST.Calendar.Providers.Outlook.csproj -o ../../../nupkgs

:: Pack Audit project
dotnet pack ./ST.Extensions/ST.Audit.Extension/ST.Audit.Abstractions/ST.Audit.Abstractions.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Audit.Extension/ST.Audit/ST.Audit.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Audit.Extension/ST.Audit.Razor/ST.Audit.Razor.csproj -o ../../../nupkgs

::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Push projects-------------------------------------------------
::-----------------------------------------------------------------------------------------------------------

cd ./nupkgs

:: Push Core projects
dotnet nuget push -k %pushKey% -s %pushHost% ST.Core*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Core.Razor*

:: Push cache projects
dotnet nuget push -k %pushKey% -s %pushHost% ST.Cache.Abstractions*


:: Push calendar projects
dotnet nuget push -k %pushKey% -s %pushHost% ST.Calendar.Abstraction*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Calendar*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Calendar.Razor*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Calendar.Abstractions.ExternalProviders*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Calendar.Providers.Google*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Calendar.Providers.Outlook*

::Push audit projects
dotnet nuget push -k %pushKey% -s %pushHost% ST.Audit.Abstractions*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Audit*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Audit.Razor*

::-----------------------------------------------------------------------------------------------------------
::---------------------------------------------Clean directories---------------------------------------------
::-----------------------------------------------------------------------------------------------------------
cd ..
rmdir /q /s "nupkgs"
PAUSE