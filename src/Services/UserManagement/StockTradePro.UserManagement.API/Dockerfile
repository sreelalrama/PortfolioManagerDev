FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/UserManagement/StockTradePro.UserManagement.API/StockTradePro.UserManagement.API.csproj", "Services/UserManagement/StockTradePro.UserManagement.API/"]
COPY ["Services/Shared/StockTradePro.Shared/StockTradePro.Shared.csproj", "Services/Shared/StockTradePro.Shared/"]
RUN dotnet restore "./Services/UserManagement/StockTradePro.UserManagement.API/StockTradePro.UserManagement.API.csproj"
COPY . .
WORKDIR "/src/Services/UserManagement/StockTradePro.UserManagement.API"
RUN dotnet build "./StockTradePro.UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StockTradePro.UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockTradePro.UserManagement.API.dll"]
