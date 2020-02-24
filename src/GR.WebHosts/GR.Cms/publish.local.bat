dotnet restore
dotnet build
rmdir /Q /S ./dist
dotnet publish -c Release -o ./dist

PAUSE