# Compliance System Prototype

Compliance System Prototype is a .NET 8 Web API prototype for managing enterprise compliance cases. It models the core workflow for creating, reviewing, resolving, closing, auditing, escalating, and reporting on compliance cases with role-based access control.

## Relation to the master thesis

This repository contains the practical prototype evaluated in the master thesis "Evaluacija primjene CQRS i Clean Architecture principa na primjeru enterprise sistema za upravljanje compliance slučajevima".

- Author: Eris Sutković
- Institution: University of Donja Gorica (UDG), 2026
- Evaluated version: `thesis-evaluation-v1.0`
- Release URL: https://github.com/eris00/compliance-system-prototype/releases/tag/thesis-evaluation-v1.0

The repository is intended to serve as the public, versioned evaluation artifact for the implementation discussed in the thesis.

## Implemented Functionality

- Compliance case management: create cases, list and filter cases, view case details, start review, resolve cases, and close resolved cases.
- Role-based access control: Analyst, Supervisor, and Auditor roles are enforced through ASP.NET Core authorization.
- Audit history: case lifecycle actions are persisted as audit entries and exposed through the case audit endpoint.
- Automatic escalation: a hosted background service periodically escalates overdue active cases.
- Reporting and read models: dashboard summary queries expose case counts, active severity distribution, escalated case counts, analyst workload, and average resolution time.

## Architecture

The system is implemented as a modular monolith using Clean Architecture principles and a logical CQRS separation.

The solution contains four projects:

- `src/ComplianceSystem.Domain`: domain entities, enums, and domain exceptions.
- `src/ComplianceSystem.Application`: commands, queries, DTOs, use-case handlers, interfaces, role constants, and MediatR registration.
- `src/ComplianceSystem.Infrastructure`: EF Core persistence, ASP.NET Core Identity integration, JWT token creation, and the automatic escalation background service.
- `src/ComplianceSystem.Api`: ASP.NET Core Web API controllers, Swagger/OpenAPI setup, runtime composition, and current-user adapter.

Dependency direction follows the solution references:

- `Domain` has no project dependency.
- `Application` references `Domain`.
- `Infrastructure` references `Application` and `Domain`.
- `Api` references `Application` and `Infrastructure`.

Commands and queries are handled through MediatR. Write-side operations change case lifecycle state and append audit records, while read-side queries return DTO-based case and dashboard models.

## Technologies

The prototype uses the following technologies as configured in the project files and runtime configuration:

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- Entity Framework Core SQL Server provider
- ASP.NET Core Identity with Entity Framework Core stores
- JWT Bearer authentication
- MediatR 12.5
- Swashbuckle.AspNetCore for Swagger/OpenAPI UI
- SQL Server 2022 container image
- Docker Compose

## Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- EF Core CLI tool (`dotnet-ef`) for applying migrations
- A local development SQL Server password stored outside Git

## Running Locally

1. Clone the repository and enter the project directory:

   ```bash
   git clone https://github.com/eris00/compliance-system-prototype.git
   cd compliance-system-prototype
   ```

2. Create a local `.env` file for Docker Compose. Use a strong local-only password and do not commit this file:

   ```text
   MSSQL_SA_PASSWORD=<your-local-sa-password>
   ```

3. Start SQL Server through Docker Compose:

   ```bash
   docker compose up -d sqlserver
   ```

4. Configure local application secrets for the API project:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=ComplianceDb;User Id=sa;Password=<your-local-sa-password>;TrustServerCertificate=True;" --project src/ComplianceSystem.Api
   dotnet user-secrets set "Jwt:Key" "<your-local-development-jwt-signing-key-at-least-32-characters>" --project src/ComplianceSystem.Api
   ```

   `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpirationMinutes`, and `Escalation:CheckIntervalSeconds` are configured in `src/ComplianceSystem.Api/appsettings.json`.

5. Apply EF Core migrations:

   ```bash
   dotnet ef database update --project src/ComplianceSystem.Infrastructure --startup-project src/ComplianceSystem.Api
   ```

6. Build the solution:

   ```bash
   dotnet build ComplianceSystem.sln
   ```

7. Run the API with the HTTP launch profile:

   ```bash
   dotnet run --project src/ComplianceSystem.Api --launch-profile http
   ```

8. Open Swagger/OpenAPI UI:

   ```text
   http://localhost:5144/swagger
   ```

The Docker Compose file also defines an `api` service exposed on port `8080` when the full Compose stack is started with `docker compose up --build`.

## User Roles

- `Analyst`: works with assigned cases, starts review, and resolves cases.
- `Supervisor`: can create cases for analysts, has global case visibility, and closes resolved cases.
- `Auditor`: has read access to cases, audit history, and dashboard reporting.

The API seeds development roles and local demo users at startup. Do not reuse prototype credentials or seeded accounts for production systems.

## Disclaimer

This project is an academic prototype created for thesis evaluation. It is not intended for production use.
