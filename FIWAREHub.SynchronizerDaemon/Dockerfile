#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FIWAREHub.SynchronizerDaemon/FIWAREHub.SynchronizerDaemon.csproj", "FIWAREHub.SynchronizerDaemon/"]
COPY ["FIWAREHub.Models/FIWAREHub.Models.csproj", "FIWAREHub.Models/"]
RUN dotnet restore "FIWAREHub.SynchronizerDaemon/FIWAREHub.SynchronizerDaemon.csproj"
COPY . .
WORKDIR "/src/FIWAREHub.SynchronizerDaemon"
RUN dotnet build "FIWAREHub.SynchronizerDaemon.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FIWAREHub.SynchronizerDaemon.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FIWAREHub.SynchronizerDaemon.dll"]