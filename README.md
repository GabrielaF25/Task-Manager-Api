# Task Manager Api

A simple, layered RESTful API for managing projects and tasks built with .NET 8. The solution follows a clean architecture: API, Application, and Infrastructure layers. It includes JWT authentication, EF Core for data access, global exception handling middleware, and Swagger for API documentation.

## Features
- User registration and JWT-based authentication
- CRUD operations for projects (and related task features)
- EF Core-backed persistence (migrations supported)
- Global exception middleware
- OpenAPI/Swagger UI with Bearer token support
- Dependency injection and modular services

## Tech stack
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- JWT (Microsoft.IdentityModel.Tokens)
- Swagger / Swashbuckle

## Repository layout (high level)
- `Task Manager Api/` — API project (entry point, controllers, middlewares)
- `TaskManager.Application/` — application layer (commands, handlers, abstractions)
- `TaskManager.Infrastructure/` — infrastructure (DbContexts, DI, implementations)
- Other supporting projects and folders follow Clean Architecture patterns

## Prerequisites
- .NET 8 SDK
- Visual Studio 2022 (recommended) or equivalent IDE
- SQL Server
