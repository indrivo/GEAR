SET pushKey="ODExNDE3Mjk5NGVkNGMzZWI1MzNmZWU2NjhhNTI4MGE1MDZkODRmM2UwMWI0MjZmOTNiZWEzOTBkNTU2MDAxYw=="
SET pushHost="http://192.168.1.70"

dotnet build ../ST.GEAR.sln

dotnet pack ./ST.Extensions/ST.Core.Extension/ST.Core/ST.Core.csproj -o ../../../nupkgs


rm ./*.nupkg
PAUSE