#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

ENV TZ=Asia/Shanghai

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["ProxyPool.WorkerService/ProxyPool.WorkerService.csproj", "ProxyPool.WorkerService/"]
COPY ["ProxyPool.Core/ProxyPool.Core.csproj", "ProxyPool.Core/"]
COPY ["ProxyPool.Service/ProxyPool.Service.csproj", "ProxyPool.Service/"]
COPY ["ProxyPool.Data.EntityFramework/ProxyPool.Data.EntityFramework.csproj", "ProxyPool.Data.EntityFramework/"]
RUN dotnet restore "ProxyPool.WorkerService/ProxyPool.WorkerService.csproj"
COPY . .
WORKDIR "/src/ProxyPool.WorkerService"
RUN dotnet build "ProxyPool.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProxyPool.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProxyPool.WorkerService.dll"]