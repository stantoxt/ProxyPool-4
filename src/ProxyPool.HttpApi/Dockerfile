#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

ENV TZ=Asia/Shanghai

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["ProxyPool.HttpApi/ProxyPool.HttpApi.csproj", "ProxyPool.HttpApi/"]
COPY ["ProxyPool.Service/ProxyPool.Service.csproj", "ProxyPool.Service/"]
COPY ["ProxyPool.Core/ProxyPool.Core.csproj", "ProxyPool.Core/"]
COPY ["ProxyPool.Data.EntityFramework/ProxyPool.Data.EntityFramework.csproj", "ProxyPool.Data.EntityFramework/"]
RUN dotnet restore "ProxyPool.HttpApi/ProxyPool.HttpApi.csproj"
COPY . .
WORKDIR "/src/ProxyPool.HttpApi"
RUN dotnet build "ProxyPool.HttpApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProxyPool.HttpApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProxyPool.HttpApi.dll"]