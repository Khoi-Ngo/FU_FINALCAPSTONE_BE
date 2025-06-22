FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_WATCH_SUPPRESS_LAUNCH=true

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the solution and project files
COPY ["AISEA.BgService.Worker/AISEA.BgService.Worker.csproj", "AISEA.BgService.Worker/"]
COPY ["AISEA.BgService.sln", "./"]

# Restore dependencies
RUN dotnet restore "AISEA.BgService.sln"

# Copy the entire source code
COPY . .

# Build the solution
WORKDIR "/src/AISEA.BgService.Worker"
RUN dotnet build -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Enable hot-reloading with dotnet watch
ENTRYPOINT ["dotnet", "AISEA.BgService.Worker.dll"]