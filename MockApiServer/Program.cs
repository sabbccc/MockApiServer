using MockApiServer.Extensions;
using MockApiServer.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Service registrations
builder.Services
    .AddMvcServices(builder.Environment)
    .AddCookieAuthentication()
    .AddAuthorization()
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Pipeline
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCorrelationId();
app.UseRequestResponseLogging();

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
