FROM mcr.microsoft.com/dotnet/core/sdk:2.2.402 AS build
COPY ./ /app
WORKDIR /app/src/GR.WebHosts/GR.Cms

RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet publish --no-restore -c Release -o dist/

WORKDIR /app/src/GR.WebHosts/GR.Cms/dist
ENTRYPOINT ["dotnet", "GR.Cms.dll"]