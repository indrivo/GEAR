SET pushKey="oy2cdyotfbd5rk7thnias2paqkriagtuxahd5t2n3woexy"
SET pushHost="https://www.nuget.org"

dotnet build ../ST.GEAR.sln

dotnet pack ./ST.Extensions/ST.Core.Extension/ST.Core/ST.Core.csproj -o ../../../nupkgs
dotnet pack ./ST.Extensions/ST.Cache.Extension/ST.Cache.Abstractions/ST.Cache.Abstractions.csproj -o ../../../nupkgs

cd ./nupkgs

dotnet nuget push -k %pushKey% -s %pushHost% ST.Core*
dotnet nuget push -k %pushKey% -s %pushHost% ST.Cache.Abstractions*

PAUSE