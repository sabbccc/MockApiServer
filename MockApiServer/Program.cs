using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MockApiServer.Data;
using MockApiServer.Extensions;
using MockApiServer.Middlewares;
using MockApiServer.Repositories;
using MockApiServer.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

#if DEBUG
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
#else
builder.Services.AddControllersWithViews();
#endif

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Authentication with Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";          // redirect if not authenticated
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/User/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // optional

        // Default session timeout (if RememberMe is false)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;

        // Intercept redirect when cookie expires or user is unauthenticated
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                var hasAuthCookie = context.Request.Cookies.ContainsKey(".AspNetCore.Cookies");

                if (hasAuthCookie && !context.Request.Path.StartsWithSegments(options.LoginPath))
                {
                    // Only show timeout message if user had a valid cookie before
                    var redirectUri = $"{options.LoginPath}?timeout=true";
                    context.Response.Redirect(redirectUri);
                }
                else
                {
                    // If no cookie, just go to login page normally
                    context.Response.Redirect(options.LoginPath);
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<MockApiServerDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("MySqlConnection");
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// Register repositories & services
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IMockRepository, MockRepository>();
builder.Services.AddScoped<IMockScenarioRepository, MockScenarioRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IMockService, MockService>();
builder.Services.AddScoped<IMockScenarioService, MockScenarioService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMockRequestsService, MockRequestsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mock API Server V1");
        c.RoutePrefix = "swagger";
    });
}

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
