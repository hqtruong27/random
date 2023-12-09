#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/Spending/Spending.Api/Spending.Api.csproj", "Microservices/Spending/Spending.Api/"]
COPY ["Microservices/Spending/Spending.Infrastructure/Spending.Infrastructure.csproj", "Microservices/Spending/Spending.Infrastructure/"]
COPY ["Libraries/Common/Common.csproj", "Libraries/Common/"]
COPY ["Libraries/Models/Models.csproj", "Libraries/Models/"]
RUN dotnet restore "./Microservices/Spending/Spending.API/./Spending.Api.csproj"
COPY . .
WORKDIR "/src/Microservices/Spending/Spending.API"
RUN dotnet build "./Spending.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Spending.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spending.Api.dll"]