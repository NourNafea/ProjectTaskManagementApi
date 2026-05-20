# Project & Task Management API

A production-quality REST API built with .NET 9, Clean Architecture, CQRS/MediatR, and JWT Authentication.

## Architecture

```
src/
├── ProjectTaskManagement.Domain/         # Entities, Enums, Exceptions (no dependencies)
├── ProjectTaskManagement.Application/    # CQRS handlers, DTOs, Validators, Interfaces
├── ProjectTaskManagement.Infrastructure/ # EF Core, Identity, JWT, SQL Server
└── ProjectTaskManagement.API/            # Controllers, Middleware, Swagger
tests/
└── ProjectTaskManagement.Application.UnitTests/  # 27 unit tests (xUnit + Moq + FluentAssertions)
```

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET 9 / ASP.NET Core |
| Architecture | Clean Architecture + CQRS |
| Mediator | MediatR 12 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server (Azure SQL Edge for ARM64) |
| Auth | JWT Bearer via ASP.NET Core Identity |
| Validation | FluentValidation (pipeline behavior) |
| API Docs | Swagger / OpenAPI |
| Versioning | Asp.Versioning (URL segment `/v1/`) |
| Tests | xUnit + Moq + FluentAssertions (27 tests) |

## Quick Start

### Prerequisites
- .NET 9 SDK
- Docker

### 1. Start SQL Server

```bash
docker run -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=Admin@12345" \
  -p 1433:1433 --name sqlserver2022 \
  --platform linux/arm64 -d mcr.microsoft.com/azure-sql-edge
```

Or with docker-compose:
```bash
docker-compose up sqlserver -d
```

### 2. Run the API

```bash
cd src/ProjectTaskManagement.API
dotnet run
```

The API starts at `https://localhost:7xxx` and applies migrations automatically on startup.

### 3. Access Swagger

Open `https://localhost:7xxx/swagger` — click **Authorize** and paste your JWT token.

## API Endpoints

### Auth (`/api/v1/auth`)
| Method | Path | Description |
|--------|------|-------------|
| POST | `/register` | Register new user |
| POST | `/login` | Login and get JWT token |

### Projects (`/api/v1/projects`) — Requires Auth
| Method | Path | Description |
|--------|------|-------------|
| GET | `/` | Get all my projects |
| GET | `/{id}` | Get project by ID |
| POST | `/` | Create project |
| PUT | `/{id}` | Update project |
| DELETE | `/{id}` | Delete project |

### Tasks (`/api/v1/tasks`) — Requires Auth
| Method | Path | Description |
|--------|------|-------------|
| GET | `/project/{projectId}` | Get tasks by project |
| POST | `/` | Create task |
| PATCH | `/{taskId}/status` | Update task status |
| DELETE | `/{taskId}` | Delete task |

## Sample Requests

### Register
```json
POST /api/v1/auth/register
{
  "fullName": "Nour Nafea",
  "email": "nour@example.com",
  "password": "Pass123"
}
```

### Create Project
```json
POST /api/v1/projects
Authorization: Bearer <token>
{
  "name": "My Project",
  "description": "Project description"
}
```

### Create Task
```json
POST /api/v1/tasks
Authorization: Bearer <token>
{
  "title": "Fix Bug #42",
  "description": "Reproduce and fix the login issue",
  "dueDate": "2026-06-01T00:00:00Z",
  "priority": 2,
  "projectId": "<project-guid>"
}
```

### Update Task Status
```json
PATCH /api/v1/tasks/{taskId}/status
Authorization: Bearer <token>
{
  "status": 2
}
```

> Status: 0=Pending, 1=InProgress, 2=Completed  
> Priority: 0=Low, 1=Medium, 2=High

## Running Tests

```bash
dotnet test
```

Result: **27 tests passed** covering handlers, validators, auth, ownership enforcement.

## Database Migrations

Migrations are applied automatically on startup. To add a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/ProjectTaskManagement.Infrastructure \
  --startup-project src/ProjectTaskManagement.API
```
