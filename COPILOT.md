# MockApiServer - Project Context

## What is this?
A lightweight .NET 8 web application that serves as a configurable mock REST API server. It allows developers and testers to simulate backend API behavior with custom responses, status codes, and scenarios through a web-based admin UI.

## Tech Stack
- **Framework**: ASP.NET Core 8.0 (Razor Pages MVC pattern)
- **Database**: MySQL with Entity Framework Core 9.0 (Pomelo provider)
- **Logging**: Serilog with correlation ID tracking
- **Authentication**: Cookie-based authentication
- **Excel Processing**: ClosedXML for bulk import/export
- **Containerization**: Docker support

## Project Structure

```
MockApiServer/
├── Controllers/          # MVC controllers (admin UI + API endpoints)
│   ├── HomeController.cs           # Dashboard (Program.cs:40)
│   ├── ApplicationsController.cs   # Application management
│   ├── MocksController.cs          # Mock endpoint management
│   ├── MockScenariosController.cs  # Scenario management
│   ├── MockRequestsController.cs   # Catch-all mock API handler (line 7-8)
│   ├── BulkUploadController.cs     # Excel bulk import/export
│   └── UserController.cs           # Authentication
├── Services/             # Business logic layer
│   ├── Interfaces/      # Service contracts
│   └── Services/        # Service implementations
├── Repositories/         # Data access layer
│   ├── Interfaces/      # Repository contracts
│   └── Repositories/    # Repository implementations
├── Data/
│   ├── Entities/        # EF Core entity models
│   └── MockApiServerDbContext.cs  # DbContext (line 8-22)
├── Models/
│   └── ViewModels/      # DTOs for UI layer
├── Middlewares/         # Custom middleware
│   ├── CorrelationIdMiddleware.cs      # Request tracking (line 7-24)
│   └── RequestResponseLoggingMiddleware.cs  # Structured logging (line 7-65)
├── Extensions/          # Extension methods
│   └── ServiceExtensions.cs  # DI registration (line 10-100)
├── Views/               # Razor views
│   └── Shared/          # Layouts and partials
├── wwwroot/             # Static files (CSS, JS, images)
└── Program.cs           # Application entry point (line 1-40)
```

## Key Concepts

### Mock Request Flow
1. Client sends request to any path not matching admin routes
2. `MockRequestsController` (line 7-8) catches via regex route pattern
3. `X-Mock-Scenario` header determines which scenario to return
4. Service layer builds response with configured status code, headers, and JSON body
5. Middleware logs request/response with correlation ID

### Database Schema
- **Applications**: Top-level grouping for mocks
- **Mocks**: API endpoint definitions (path + HTTP method)
- **MockScenarios**: Multiple response scenarios per mock (controlled by scenario key)
- **Users**: Admin authentication

## Essential Commands

### Development
```bash
# Restore and build
dotnet restore
dotnet build

# Run locally
dotnet run --project MockApiServer/MockApiServer.csproj
# App runs at: http://localhost:5000

# Run with hot reload (Development)
dotnet watch --project MockApiServer/MockApiServer.csproj
```

### Database
```bash
# Add migration
dotnet ef migrations add <MigrationName> --project MockApiServer

# Update database
dotnet ef database update --project MockApiServer
```

### Docker
```bash
# Build and run
docker-compose up -d

# View logs
docker-compose logs -f api
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~TestClassName"
```

## Configuration Files
- `appsettings.json` - Serilog config, connection strings (line 1-37)
- `appsettings.Development.json` - Development overrides
- `docker-compose.yml` - Container orchestration (line 1-14)

## Additional Documentation
When working on specific aspects, consult:
- `.claude/docs/architectural_patterns.md` - Design patterns, DI, conventions

## Quick Start for New Contributors
1. Ensure MySQL is running (local or Docker)
2. Update connection string in `appsettings.json` (line 35)
3. Run `dotnet ef database update` to create schema
4. Run `dotnet watch` for hot reload development
5. Navigate to `/User/Login` (default credentials in seed data)
6. **Use Bulk Upload** (`/BulkUpload`) to quickly import sample data:
   - Download the pre-filled Excel template
   - Contains 3 Applications, 6 Mocks, and 9 Scenarios
   - Upload the file to populate your database instantly

## Bulk Upload Feature
Navigate to **Bulk Upload** in the sidebar to quickly set up mock data:
- **Download Sample Excel**: Pre-filled template with Applications, Mocks, and Scenarios
- **Three sheets**: Applications → Mocks → MockScenarios (hierarchical)
- **Smart Import**: Skips duplicates, links data automatically
- **Example Data**: Payments, User Management, Notifications with realistic responses
- **BulkUploadController.cs**: Uses ClosedXML to generate/parse Excel files (line 24-393)

## API Usage Example
```bash
# Get mock response for scenario "success"
curl -H "X-Mock-Scenario: success" http://localhost:5000/api/users/123

# Get error scenario
curl -H "X-Mock-Scenario: not-found" http://localhost:5000/api/users/999
```

## Important Notes
- Cookie authentication expires after 20 minutes with sliding expiration (ServiceExtensions.cs:21)
- All entities include audit fields: `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- Correlation IDs are auto-generated if not provided by client
- Static file requests (`/css`, `/js`) skip request logging
- Health check available at `/health`
