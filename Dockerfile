FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore FIWAREHub.Web/*.csproj
RUN dotnet publish FIWAREHub.Web/*.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/FIWAREHub.Web/out ./
ENTRYPOINT ["dotnet", "FIWAREHub.Web.dll"]