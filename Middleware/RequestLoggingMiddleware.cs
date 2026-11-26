using System.Diagnostics;

namespace ParcelTrack.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string requestId = Guid.NewGuid().ToString("N").Substring(0, 8);
            context.Response.Headers["X-Request-Id"] = requestId;

            Stopwatch sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            Console.WriteLine($"[{requestId}] {context.Request.Method} {context.Request.Path} -> {context.Response.StatusCode} in {sw.ElapsedMilliseconds} ms");
        }
    }
}
