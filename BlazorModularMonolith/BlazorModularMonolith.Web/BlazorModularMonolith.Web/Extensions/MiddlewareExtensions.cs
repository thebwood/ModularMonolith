using BlazorModularMonolith.Web.Middleware;

namespace BlazorModularMonolith.Web.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        return app;
    }
}
