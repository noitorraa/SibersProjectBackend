# ASP.NET Core Web App Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY sibersProject/sibersProject.csproj .
RUN dotnet restore sibersProject.csproj
COPY sibersProject/ .
RUN dotnet publish sibersProject.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install sqlite3 for init script
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
RUN mkdir -p /app/Database
COPY sibersProject/Database/script.sql ./Database/script.sql
RUN mkdir -p /docker-entrypoint
COPY docker-entrypoint/init-db.sh /docker-entrypoint/init-db.sh
RUN chmod +x /docker-entrypoint/init-db.sh
ENTRYPOINT ["/docker-entrypoint/init-db.sh"]
CMD ["dotnet", "sibersProject.dll"]
