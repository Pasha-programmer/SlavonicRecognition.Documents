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
COPY ["Documents.WebApi/Documents.WebApi.csproj", "Documents.WebApi/"]
COPY ["Documents.EndPoints/Documents.EndPoints.csproj", "Documents.EndPoints/"]
COPY ["Documents.Contract/Documents.Contract.csproj", "Documents.Contract/"]
COPY ["Documents.Infrastructure/Documents.Infrastructure.csproj", "Documents.Infrastructure/"]
COPY ["Documents.Database/Documents.Database.csproj", "Documents.Database/"]
RUN dotnet restore "./Documents.WebApi/Documents.WebApi.csproj"
COPY . .
WORKDIR "/src/Documents.WebApi"
RUN dotnet build "./Documents.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Documents.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
USER root
RUN mkdir -p /app/data && chown $APP_UID /app/data
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Documents.WebApi.dll"]