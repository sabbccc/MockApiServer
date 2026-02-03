using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MockApiServer.Data;
using MockApiServer.Repositories;
using MockApiServer.Services;

namespace MockApiServer.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCookieAuthentication(
           this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                    options.LogoutPath = "/User/Logout";
                    options.AccessDeniedPath = "/User/AccessDenied";

                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;

                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            var hasAuthCookie =
                                context.Request.Cookies.ContainsKey(".AspNetCore.Cookies");

                            if (hasAuthCookie &&
                                !context.Request.Path.StartsWithSegments(options.LoginPath))
                            {
                                context.Response.Redirect(
                                    $"{options.LoginPath}?timeout=true");
                            }
                            else
                            {
                                context.Response.Redirect(options.LoginPath);
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        public static IServiceCollection AddDatabase(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("MySqlConnection");

            services.AddDbContext<MockApiServerDbContext>(options =>
            {
                options.UseMySql(cs, ServerVersion.AutoDetect(cs));
            });

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IMockRepository, MockRepository>();
            services.AddScoped<IMockScenarioRepository, MockScenarioRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IMockService, MockService>();
            services.AddScoped<IMockScenarioService, MockScenarioService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMockRequestsService, MockRequestsService>();

            return services;
        }

        public static IServiceCollection AddMvcServices(
            this IServiceCollection services,
            IHostEnvironment environment)
        {
        #if DEBUG
            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();
        #else
        services.AddControllersWithViews();

        #endif
            return services;
        }
    }
}
