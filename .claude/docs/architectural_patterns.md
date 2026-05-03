# Architectural Patterns & Design Decisions

## Architecture Overview
MockApiServer follows a **clean architecture** approach with clear separation of concerns across layers:

```
Presentation (Controllers/Views) → Services → Repositories → Data Access (EF Core)
```

All layers interact through interfaces to enable testability and maintainability.

---

## Dependency Injection Patterns

### Service Registration (ServiceExtensions.cs)
The application uses **extension methods** to organize DI registrations by concern:

```csharp
// ServiceExtensions.cs:10-100
builder.Services
    .AddMvcServices(builder.Environment)      // MVC with conditional Razor compilation
    .AddCookieAuthentication()                // Cookie auth configuration
    .AddAuthorization()
    .AddDatabase(builder.Configuration)       // EF Core DbContext
    .AddRepositories()                        // Data access layer
    .AddApplicationServices();                // Business logic layer
```

**Pattern**: Fluent chainable extensions for clean, modular startup configuration.

### Scoped Lifetime
All services and repositories use **scoped lifetime** to align with EF Core DbContext lifecycle:

```csharp
// ServiceExtensions.cs:64-81
services.AddScoped<IApplicationRepository, ApplicationRepository>();
services.AddScoped<IApplicationService, ApplicationService>();
```

**Why**: Ensures one DbContext instance per HTTP request, preventing concurrency issues.

---

## Repository Pattern

### Interface-Based Data Access
All data operations go through repository interfaces:

```csharp
// IApplicationRepository defines contract
public interface IApplicationRepository
{
    Task<List<ApplicationViewModel>> GetAllAsync();
    Task<ApplicationViewModel?> GetByIdAsync(int id);
    Task AddAsync(ApplicationViewModel model);
    Task UpdateAsync(ApplicationViewModel model);
    Task DeleteAsync(int id);
}

// ApplicationRepository.cs:8-75 implements using EF Core
```

**Benefits**:
- Abstracts EF Core implementation details from services
- Enables easy mocking for unit tests
- Centralizes query logic

### ViewModel Projection
Repositories return **ViewModels** instead of entities to avoid:
- Over-fetching data
- Exposing navigation properties unintentionally
- Lazy loading issues

```csharp
// ApplicationRepository.cs:19-26
return await _context.Applications
    .Select(a => new ApplicationViewModel
    {
        Id = a.Id,
        Name = a.Name,
        IsActive = a.IsActive
    }).ToListAsync();
```

---

## Service Layer Pattern

### Business Logic Separation
Services contain business rules and orchestrate repository calls:

```csharp
// MockRequestsService.cs:8-60
public class MockRequestsService : IMockRequestsService
{
    private readonly IMockRepository _repository;

    // Validates scenario header, fetches mock, builds response
    public async Task<(MockScenario? scenario, string? error)> GetScenarioAsync(...)
    {
        if (string.IsNullOrWhiteSpace(scenarioKey))
            return (null, "Missing X-Mock-Scenario header");

        var mock = await _repository.GetMockWithScenariosAsync(path, method);
        // ... business validation logic
    }
}
```

**Pattern**: Services depend on repository interfaces, never DbContext directly.

---

## Middleware Patterns

### Correlation ID Tracking
Custom middleware injects/reads correlation IDs for distributed tracing:

```csharp
// CorrelationIdMiddleware.cs:7-44
public async Task InvokeAsync(HttpContext context)
{
    // Generate if missing, add to request and response headers
    if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

    context.Response.OnStarting(() => {
        context.Response.Headers[CorrelationIdHeader] = correlationId.ToString();
        return Task.CompletedTask;
    });
}
```

**Integration**: Serilog enricher adds correlation ID to all logs (appsettings.json:17).

### Request/Response Logging
Middleware captures and logs full HTTP context:

```csharp
// RequestResponseLoggingMiddleware.cs:7-80
public async Task InvokeAsync(HttpContext context)
{
    // Enable buffering to read body multiple times
    context.Request.EnableBuffering();

    // Capture request body
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    context.Request.Body.Position = 0; // Reset for next middleware

    // Swap response stream to capture output
    var originalBody = context.Response.Body;
    using var newBody = new MemoryStream();
    context.Response.Body = newBody;

    await _next(context);

    // Log minified JSON (not pretty-printed)
    Log.Information("[{RequestId}] ==> [Request : {Method} ==> {Path}] | Body: {Body}", ...);
}
```

**Performance Note**: Skips logging for static files (`/css`, `/js`).

---

## Controller Patterns

### Catch-All Route for Mock API
Uses **regex route constraint** to handle any path not matching admin routes:

```csharp
// MockRequestsController.cs:7-8
[Route("{*path:regex(^(?!swagger|health|home|applications|mocks|mockscenarios|bulkupload|user).*$)}")]
[AllowAnonymous]
public class MockRequestsController : ControllerBase
{
    [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
    public async Task<IActionResult> HandleAsync(string path) { ... }
}
```

**Pattern**: Negative lookahead regex ensures admin routes take precedence.

### AJAX Partial Views
Admin controllers return **partial view HTML** for dynamic updates:

```csharp
// ApplicationsController.cs:28-39
[HttpPost]
public async Task<IActionResult> Create(ApplicationViewModel model)
{
    await _service.AddAsync(model);
    var apps = await _service.GetAllAsync();

    var html = await this.RenderViewAsync("_ViewAll", apps, true);
    return Json(new { success = true, message = "...", html });
}
```

**Frontend**: JavaScript replaces DOM elements without full page reload.

---

## Authentication Pattern

### Cookie-Based Authentication
Configured with sliding expiration and custom redirect logic:

```csharp
// ServiceExtensions.cs:12-46
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true; // Extends on activity

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // Custom logic: detect session timeout vs. initial login
                var hasAuthCookie = context.Request.Cookies.ContainsKey(".AspNetCore.Cookies");
                if (hasAuthCookie)
                    context.Response.Redirect($"{options.LoginPath}?timeout=true");
                // ...
            }
        };
    });
```

**All admin controllers** use `[Authorize]` attribute; API controller uses `[AllowAnonymous]`.

---

## Entity Framework Patterns

### DbContext Configuration
Database-first approach using Pomelo MySQL provider:

```csharp
// ServiceExtensions.cs:49-61
public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
{
    var cs = configuration.GetConnectionString("MySqlConnection");

    services.AddDbContext<MockApiServerDbContext>(options =>
    {
        options.UseMySql(cs, ServerVersion.AutoDetect(cs));
    });

    return services;
}
```

### Entity Conventions
All entities follow consistent audit field pattern:

```csharp
// Application.cs:6-23
public partial class Application
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }        // Auto-set by DB
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }       // Auto-updated by DB
    public string? UpdatedBy { get; set; }
    public bool? IsActive { get; set; }

    public virtual ICollection<Mock> Mocks { get; set; } = new List<Mock>();
}
```

**Pattern**: Navigation properties use `virtual` for lazy loading (if enabled).

---

## Conditional Compilation

### Razor Runtime Compilation
Development environment enables hot reload for views:

```csharp
// ServiceExtensions.cs:88-95
public static IServiceCollection AddMvcServices(this IServiceCollection services, IHostEnvironment environment)
{
#if DEBUG
    services.AddControllersWithViews().AddRazorRuntimeCompilation();
#else
    services.AddControllersWithViews();
#endif
    return services;
}
```

**Production**: Compiles views at build time for performance.

---

## Error Handling Pattern

### Tuple-Based Error Responses
Services return tuples to avoid exceptions for expected failures:

```csharp
// IMockRequestsService.cs
Task<(MockScenario? scenario, string? error)> GetScenarioAsync(...);

// Usage in controller:
var (scenario, error) = await _mockRequestsService.GetScenarioAsync(...);
if (error != null)
    return BadRequest(new { error });
```

**Benefits**:
- Explicit error handling
- No performance penalty of try-catch
- Clear success/failure states

---

## Logging Conventions

### Structured Logging with Serilog
All logs use **structured logging** with named properties:

```csharp
// MockRequestsController.cs:28
Serilog.Log.Information("[Final Response : {StatusCode}] | Body: {RawJson}", 
    scenario!.StatusCode, rawJson);

// RequestResponseLoggingMiddleware.cs:38-41
Log.Information("[{RequestId}] ==> [Request : {Method} ==> {Path}] | Body: {Body}",
    requestId, context.Request.Method, context.Request.Path, MinifyJson(requestBody));
```

**Pattern**: Always log minified JSON (not pretty-printed) for efficient storage and querying.

---

## Bulk Operations Pattern

### Excel Import/Export with ClosedXML
Provides template download and bulk data import:

```csharp
// BulkUploadController.cs:24-51
[HttpGet]
public IActionResult Sample()
{
    using var workbook = new XLWorkbook();

    var appsSheet = workbook.Worksheets.Add("Applications");
    appsSheet.Cell(1, 1).Value = "Name";
    appsSheet.Cell(1, 2).Value = "IsActive";
    // ... populate sample data

    appsSheet.Columns().AdjustToContents();
    // Return Excel file for download
}
```

**Use Case**: Quickly populate mock data from spreadsheets for testing scenarios.

---

## Key Design Decisions

| Decision | Rationale | Reference |
|----------|-----------|-----------|
| Repository pattern over direct DbContext | Testability and abstraction | ServiceExtensions.cs:64-70 |
| ViewModels in repositories | Avoid entity tracking/lazy load issues | ApplicationRepository.cs:19-26 |
| Scoped DI lifetime | Aligns with EF Core best practices | ServiceExtensions.cs:64-81 |
| Tuple error returns | Explicit error handling without exceptions | IMockRequestsService.cs |
| Minified JSON logging | Efficient log storage and parsing | RequestResponseLoggingMiddleware.cs:67-78 |
| Regex route constraint | Catch-all route without breaking admin | MockRequestsController.cs:7 |
| Correlation ID middleware | Distributed tracing support | CorrelationIdMiddleware.cs:7-44 |
| Cookie sliding expiration | Better UX for active sessions | ServiceExtensions.cs:21-22 |
