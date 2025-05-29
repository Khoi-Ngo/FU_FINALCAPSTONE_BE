# AISEA Backend aka FU CAPSTONE (SUMMER 2025)

This repository contains the backend services for the **AISEA** project, developed using **.NET 8.0**. The solution is split into two main components:

- **AISEA.ApiService**: An ASP.NET Core Web API providing RESTful endpoints.
- **AISEA.BgService**: A .NET Worker Service for background processing.

Both services are containerized using Docker and orchestrated with Docker Compose, enabling a consistent development environment with support for code updates via volume mounts.

---

## Table of Contents

- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Running the Application](#running-the-application)

---

## Project Structure

The backend is organized into two solutions under the `Back-end` directory:

```
Back-end/
├── AISEA.ApiService/
│   ├── AISEA.ApiService.WebApi/        # Web API startup project
│   │   └── Dockerfile                  # Dockerfile for WebApi
│   ├── AISEA.ApiService.BAL/           # Business logic layer
│   ├── AISEA.ApiService.DAL/           # Data access layer
│   ├── AISEA.ApiService.SHARED/        # Shared utilities/models
│   └── AISEA.ApiService.sln            # Solution file
├── AISEA.BgService/
│   ├── AISEA.BgService.Worker/         # Worker Service startup project
│   │   └── Dockerfile                  # Dockerfile for Worker
│   ├── AISEA.BgService.SHARED/         # Shared utilities/models
│   └── AISEA.BgService.sln             # Solution file
├── docker-compose.yml                  # Docker Compose configuration
├── .gitignore                          # Git ignore file
└── README.md                           # This file
```

- **AISEA.ApiService**: Contains the Web API (`AISEA.ApiService.WebApi`), business logic (BAL), data access (DAL), and shared code (SHARED).
- **AISEA.BgService**: Contains the Worker Service (`AISEA.BgService.Worker`) and shared code (SHARED).

---

## Prerequisites

- **Docker**: Install [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes Docker Compose).
- **.NET SDK 8.0**: Required for local development or building outside Docker. [Download from Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- **Git**: For cloning the repository.
- **IDE**: Visual Studio 2022, Rider, or VS Code for editing code.

---

## Setup Instructions

**Clone the Repository:**
```sh
git clone <repository-url>
cd Back-end
```

**Verify Directory Structure:**  
Ensure the directory structure matches the one described above. Key files:

- `AISEA.ApiService\AISEA.ApiService.WebApi\Dockerfile`
- `AISEA.BgService\AISEA.BgService.Worker\Dockerfile`
- `docker-compose.yml`

---

## Install Docker

- Ensure Docker Desktop is running.

**Verify installation:**
```sh
docker --version
docker-compose --version
```

---

## Running the Application

The services are containerized and orchestrated using Docker Compose.

**Build and Start Containers:**  
Navigate to the `Back-end` directory and run:
```sh
docker-compose up --build
```

- This builds the Docker images for `AISEA.ApiService.WebApi` and `AISEA.BgService.Worker`.
- The Web API will be available at [http://localhost:5000](http://localhost:5000) (adjust if a different port is configured in `appsettings.json` or `Program.cs`).
- The Worker Service runs in the background and does not expose ports.

**Stop Containers:**  
To stop the services:
```sh
docker-compose down
```

---

## Access the API

- Open a browser or use a tool like Postman to access [http://localhost:5000](http://localhost:5000) (e.g., [http://localhost:5000/swagger](http://localhost:5000/swagger) if Swagger is enabled).
- Verify the Worker Service is running by checking logs:
```sh
docker-compose logs aisea-worker
```




