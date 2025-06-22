FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_WATCH_SUPPRESS_LAUNCH=true

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the solution and project files
COPY ["AISEA.ApiService.WebApi/AISEA.ApiService.WebApi.csproj", "AISEA.ApiService.WebApi/"]
COPY ["AISEA.ApiService.BAL/AISEA.ApiService.BAL.csproj", "AISEA.ApiService.BAL/"]
COPY ["AISEA.ApiService.DAL/AISEA.ApiService.DAL.csproj", "AISEA.ApiService.DAL/"]
COPY ["AISEA.ApiService.SHARED/AISEA.ApiService.SHARED.csproj", "AISEA.ApiService.SHARED/"]
COPY ["AISEA.ApiService.sln", "./"]

# Restore dependencies
RUN dotnet restore "AISEA.ApiService.sln"

# Copy the entire source code
COPY . .

# Build the solution
WORKDIR "/src/AISEA.ApiService.WebApi"
RUN dotnet build -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Enable hot-reloading with dotnet watch
ENTRYPOINT ["dotnet", "AISEA.ApiService.WebApi.dll"]