#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FIWAREHub.Web/FIWAREHub.Web.csproj", "FIWAREHub.Web/"]
COPY ["FIWAREHub.Parsers/FIWAREHub.Parsers.csproj", "FIWAREHub.Parsers/"]
COPY ["FIWAREHub.Datasets/FIWAREHub.Datasets.csproj", "FIWAREHub.Datasets/"]
COPY ["FIWAREHub.Models/FIWAREHub.Models.csproj", "FIWAREHub.Models/"]
RUN dotnet restore "FIWAREHub.Web/FIWAREHub.Web.csproj"
COPY . .
WORKDIR "/src/FIWAREHub.Web"
RUN dotnet build "FIWAREHub.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FIWAREHub.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FIWAREHub.Web.dll"]