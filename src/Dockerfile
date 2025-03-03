#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine-amd64 AS base
WORKDIR /app
EXPOSE 5000

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=Asia/Bangkok

RUN apk add --no-cache tzdata icu-data-full icu-libs
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SlipIntelligence.API/SlipIntelligence.Api.csproj", "SlipIntelligence.API/"]
COPY ["SlipIntelligence.Application/SlipIntelligence.Application.csproj", "SlipIntelligence.Application/"]
COPY ["SlipIntelligence.Infrastructure/SlipIntelligence.Infrastructure.csproj", "SlipIntelligence.Infrastructure/"]
RUN dotnet restore "./SlipIntelligence.API/SlipIntelligence.Api.csproj"
COPY . .
WORKDIR "/src/SlipIntelligence.API"
RUN dotnet build "./SlipIntelligence.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SlipIntelligence.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "SlipIntelligence.Api.dll"]