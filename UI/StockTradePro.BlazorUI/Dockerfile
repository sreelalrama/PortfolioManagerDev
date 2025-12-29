FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UI/StockTradePro.BlazorUI/StockTradePro.BlazorUI.csproj", "UI/StockTradePro.BlazorUI/"]
COPY ["Services/Shared/StockTradePro.Shared/StockTradePro.Shared.csproj", "Services/Shared/StockTradePro.Shared/"]
RUN dotnet restore "./UI/StockTradePro.BlazorUI/StockTradePro.BlazorUI.csproj"
COPY . .
WORKDIR "/src/UI/StockTradePro.BlazorUI"
RUN dotnet build "./StockTradePro.BlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StockTradePro.BlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockTradePro.BlazorUI.dll"]
