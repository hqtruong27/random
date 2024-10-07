#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/Discord/Discord.Bot/Discord.Bot.csproj", "Microservices/Discord/Discord.Bot/"]
COPY ["Libraries/Common/Common.csproj", "Libraries/Common/"]
COPY ["Libraries/Models/Models.csproj", "Libraries/Models/"]
COPY ["Libraries/Dispatcher/Dispatcher.csproj", "Libraries/Dispatcher/"]
COPY ["Microservices/Hoyoverse/Hoyoverse.Infrastructure/Hoyoverse.Infrastructure.csproj", "Microservices/Hoyoverse/Hoyoverse.Infrastructure/"]
RUN dotnet restore "./Microservices/Discord/Discord.Bot/Discord.Bot.csproj"
COPY . .
WORKDIR "/src/Microservices/Discord/Discord.Bot"
RUN dotnet build "./Discord.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Discord.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discord.Bot.dll"]