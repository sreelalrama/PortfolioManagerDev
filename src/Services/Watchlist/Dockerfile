FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Watchlist/StockTradePro.WatchlistService.API.csproj", "Services/Watchlist/"]
COPY ["Services/Shared/StockTradePro.Shared/StockTradePro.Shared.csproj", "Services/Shared/StockTradePro.Shared/"]
RUN dotnet restore "./Services/Watchlist/StockTradePro.WatchlistService.API.csproj"
COPY . .
WORKDIR "/src/Services/Watchlist"
RUN dotnet build "./StockTradePro.WatchlistService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StockTradePro.WatchlistService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockTradePro.WatchlistService.API.dll"]
