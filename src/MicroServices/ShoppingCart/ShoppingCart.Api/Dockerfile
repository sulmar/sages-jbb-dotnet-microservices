# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MicroServices/ShoppingCart/ShoppingCart.Api/ShoppingCart.Api.csproj", "MicroServices/ShoppingCart/ShoppingCart.Api/"]
COPY ["MicroServices/Shared/Shared.Domain/Shared.Domain.csproj", "MicroServices/Shared/Shared.Domain/"]
COPY ["MicroServices/ShoppingCart/ShoppingCart.Domain/ShoppingCart.Domain.csproj", "MicroServices/ShoppingCart/ShoppingCart.Domain/"]
COPY ["MicroServices/ShoppingCart/ShoppingCart.Infrastructure/ShoppingCart.Infrastructure.csproj", "MicroServices/ShoppingCart/ShoppingCart.Infrastructure/"]
RUN dotnet restore "./MicroServices/ShoppingCart/ShoppingCart.Api/ShoppingCart.Api.csproj"
COPY . .
WORKDIR "/src/MicroServices/ShoppingCart/ShoppingCart.Api"
RUN dotnet build "./ShoppingCart.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShoppingCart.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCart.Api.dll"]