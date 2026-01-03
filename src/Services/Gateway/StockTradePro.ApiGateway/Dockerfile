FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Gateway/StockTradePro.ApiGateway/StockTradePro.ApiGateway.csproj", "Services/Gateway/StockTradePro.ApiGateway/"]
RUN dotnet restore "./Services/Gateway/StockTradePro.ApiGateway/StockTradePro.ApiGateway.csproj"
COPY . .
WORKDIR "/src/Services/Gateway/StockTradePro.ApiGateway"
RUN dotnet build "./StockTradePro.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StockTradePro.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockTradePro.ApiGateway.dll"]
