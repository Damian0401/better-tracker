FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend.slnx ./
COPY src/BetterTracker/BetterTracker.csproj src/BetterTracker/
COPY src/BetterTracker.Api/BetterTracker.Api.csproj src/BetterTracker.Api/
COPY src/BetterTracker.Common/BetterTracker.Common.csproj src/BetterTracker.Common/
COPY src/BetterTracker.Contracts/BetterTracker.Contracts.csproj src/BetterTracker.Contracts/
COPY src/BetterTracker.Core/BetterTracker.Core.csproj src/BetterTracker.Core/
COPY src/BetterTracker.Data/BetterTracker.Data.csproj src/BetterTracker.Data/
COPY src/BetterTracker.Data.Migrations/BetterTracker.Data.Migrations.csproj src/BetterTracker.Data.Migrations/

RUN dotnet restore src/BetterTracker/BetterTracker.csproj

COPY . .
RUN dotnet publish src/BetterTracker/BetterTracker.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/data && chown -R "$APP_UID" /app/data

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "BetterTracker.dll"]
