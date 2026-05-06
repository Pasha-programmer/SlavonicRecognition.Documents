# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release# Значение по умолчанию, если не передано
# Устанавливаем переменную окружения на основе аргумента сборки
ENV BUILD_CONFIGURATION=${BUILD_CONFIGURATION}
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

FROM build AS publish
# Объявляем аргумент сборки
ARG BUILD_CONFIGURATION=release # Значение по умолчанию, если не передано
# Устанавливаем переменную окружения на основе аргумента сборки
ENV BUILD_CONFIGURATION=${BUILD_CONFIGURATION}
RUN dotnet publish "./Documents.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#Полные права на чтение, запись и выполнение внутри контейнера
RUN chmod -R 777 /app/publish

FROM base AS final
WORKDIR /app
ARG ASPNETCORE_ENVIRONMENT=Production
ARG ASPNETCORE_URLS="http://*:8080"
ARG BUILD_CONFIGURATION=release
ENV BUILD_CONFIGURATION=${BUILD_CONFIGURATION}
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_URLS=$ASPNETCORE_URLS
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Documents.WebApi.dll"]