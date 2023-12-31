#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api/GenshinImpact.Api.csproj", "Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api/"]
COPY ["Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Services/GenshinImpact.Services.csproj", "Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Services/"]
COPY ["Libraries/Models/Models.csproj", "Libraries/Models/"]
COPY ["Libraries/Common/Common.csproj", "Libraries/Common/"]
COPY ["Libraries/Dispatcher/Dispatcher.csproj", "Libraries/Dispatcher/"]
COPY ["Microservices/Hoyoverse/Hoyoverse.Infrastructure/Hoyoverse.Infrastructure.csproj", "Microservices/Hoyoverse/Hoyoverse.Infrastructure/"]
RUN dotnet restore "./Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api/GenshinImpact.Api.csproj"
COPY . .
WORKDIR "/src/Microservices/Hoyoverse/GenshinImpact/GenshinImpact.Api"
RUN dotnet build "./GenshinImpact.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GenshinImpact.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GenshinImpact.Api.dll"]