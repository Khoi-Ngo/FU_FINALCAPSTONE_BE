# AISEA Backend  FPTU CAPSTONE SU25

This repository contains the backend services for the **AISEA** project, developed using **.NET 8.0**. The solution is split into two main components:

- **AISEA.ApiService**: An ASP.NET Core Web API providing RESTful endpoints and some background services via worker service (Trigger CI CD)



---

## Table of Contents

- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Running the Application](#running-the-application)

---

## Project Structure


```
Back-end/
├── AISEA.ApiService/
│   ├── AISEA.ApiService.WebApi/        # Web API startup project
│   │   └── Dockerfile                  # Dockerfile for WebApi
│   ├── AISEA.ApiService.BAL/           # Business logic layer
│   ├── AISEA.ApiService.DAL/           # Data access layer
│   ├── AISEA.ApiService.SHARED/        # Shared utilities/models
│   └── AISEA.ApiService.sln            # Solution file
├── docker-compose.yml                  # Docker Compose configuration
├── .gitignore                          # Git ignore file
└── README.md                           # This file
```

- **AISEA.ApiService**: Contains the Web API (`AISEA.ApiService.WebApi`), business logic (BAL), data access (DAL), and shared code (SHARED).

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

- This builds the Docker images for `AISEA.ApiService.WebApi` 
- The Web API will be available at [http://localhost:5000](http://localhost:5000)

**Stop Containers:**  
To stop the services:
```sh
docker-compose down
```

---

## Access the API

- Open a browser or use a tool like Postman to access [http://localhost:5000](http://localhost:5000) (e.g., [http://localhost:5000/swagger](http://localhost:5000/swagger) if Swagger is enabled).

## Implementation Notes

### ApiService

#### Package Structure

**Reference System Architecture:**  
![System Architecture Diagram](assets/01.png)

- The project uses a 3-layer architecture in .NET Core Web API:
  - **Controller** (API)
  - **BAL** (Business Logic)
  - **DAL** (Data Access)
- The **SHARED** folder should only include:
  - Utility properties and methods
  - Third-party interfaces (DAL will implement via "Service Agents")
  - **Do not** include application-specific business logic here
- Endpoints are secured by default. Use the `[AllowAnonymous]` attribute to allow unauthenticated access for specific endpoints.
- Middleware handles exceptions (e.g., `InvalidToken`, `NullReferenceException`) and returns HTTP error codes in a consistent JSON format.

#### Error Handling

- For errors caused by invalid user input or business logic violations:
  - **Do not** return a generic 5XX error.
  - Instead, define a custom exception type in the **SHARED** project.
  - Use the `MiddlewareException` class to return a well-formatted, expected error response.



