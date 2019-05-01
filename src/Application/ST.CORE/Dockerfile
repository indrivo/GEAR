FROM microsoft/dotnet:latest
COPY ./ /app
WORKDIR /app

RUN dotnet restore
RUN dotnet build

EXPOSE 9099/tcp
ENV ASPNETCORE_URLS http://*:9099
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT dotnet run
