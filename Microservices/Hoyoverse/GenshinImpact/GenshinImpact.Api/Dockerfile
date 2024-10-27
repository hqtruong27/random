# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api/GenshinImpact.Api.csproj", "Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api/"]
COPY ["Libraries/Dispatcher/Dispatcher.csproj", "Libraries/Dispatcher/"]
COPY ["Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Infrastructure/GenshinImpact.Infrastructure.csproj", "Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Infrastructure/"]
COPY ["Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Core/GenshinImpact.Core.csproj", "Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Core/"]
COPY ["Microservices/Hoyoverse/Hoyolab/Hoyolab.Api/Hoyolab.Api.csproj", "Microservices/Hoyoverse/Hoyolab/Hoyolab.Api/"]
COPY ["Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Shared/GenshinImpact.Shared.csproj", "Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Shared/"]
RUN dotnet restore "./Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api/GenshinImpact.Api.csproj"
COPY . .
WORKDIR "/src/Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api"
RUN dotnet build "./GenshinImpact.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GenshinImpact.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GenshinImpact.Api.dll"]