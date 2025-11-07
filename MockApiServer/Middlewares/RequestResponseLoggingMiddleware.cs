using Serilog;
using System.Text.Json;

namespace MockApiServer.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            // Ignore favicon & static files
            if (path.StartsWith("/css") || path.StartsWith("/js"))
            {
                await _next(context);
                return;
            }

            var requestId = context.TraceIdentifier;

            // Capture request body
            context.Request.EnableBuffering();
            var requestBody = "";
            if (context.Request.ContentLength > 0)
            {
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            Log.Information("[{RequestId}] ==> [Request : {Method} ==> {Path}] | Body: {Body}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                MinifyJson(requestBody));

            // Capture response
            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next(context);

            // Override 304 for mock purposes
            if (context.Response.StatusCode == StatusCodes.Status304NotModified)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
            }

            newBody.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(newBody).ReadToEndAsync();
            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originalBody);

            Log.Information("[{RequestId}] <== [Response : {StatusCode}] | Body: {Body}",
                requestId,
                context.Response.StatusCode,
                MinifyJson(responseBody));
        }

        private static string MinifyJson(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return body;
            try
            {
                using var doc = JsonDocument.Parse(body);
                return JsonSerializer.Serialize(doc);
            }
            catch
            {
                return body; // not JSON, keep raw
            }
        }
    }
}
