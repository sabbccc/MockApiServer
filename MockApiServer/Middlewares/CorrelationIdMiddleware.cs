using System.Diagnostics;

namespace MockApiServer.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeader = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if client sent a correlation ID
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId)
                || string.IsNullOrWhiteSpace(correlationId))
            {
                // Generate one if missing
                correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
                context.Request.Headers[CorrelationIdHeader] = correlationId;
            }

            // Add to response so client can see it too
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId.ToString();
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
