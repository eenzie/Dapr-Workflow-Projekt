# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WarehouseService/OrderSystem.WarehouseService.Api/OrderSystem.WarehouseService.Api.csproj", "WarehouseService/OrderSystem.WarehouseService.Api/"]
COPY ["WarehouseService/OrderSystem.WarehouseService.Application/OrderSystem.WarehouseService.Application.csproj", "WarehouseService/OrderSystem.WarehouseService.Application/"]
COPY ["WarehouseService/OrderSystem.WarehouseService.Domain/OrderSystem.WarehouseService.Domain.csproj", "WarehouseService/OrderSystem.WarehouseService.Domain/"]
COPY ["WarehouseService/OrderSystem.WarehouseService.Infrastructure/OrderSystem.WarehouseService.Infrastructure.csproj", "WarehouseService/OrderSystem.WarehouseService.Infrastructure/"]
COPY ["WarehouseService/OrderSystem.WarehouseService.DatabaseMigration/OrderSystem.WarehouseService.DatabaseMigration.csproj", "WarehouseService/OrderSystem.WarehouseService.DatabaseMigration/"]
RUN dotnet restore "./WarehouseService/OrderSystem.WarehouseService.Api/OrderSystem.WarehouseService.Api.csproj"
COPY . .
WORKDIR "/src/WarehouseService/OrderSystem.WarehouseService.Api"
RUN dotnet build "./OrderSystem.WarehouseService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderSystem.WarehouseService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderSystem.WarehouseService.Api.dll"]