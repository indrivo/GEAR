dotnet restore
dotnet build
rmdir /Q /S ./dist
dotnet publish --no-restore -c Release -o ./dist

PAUSE