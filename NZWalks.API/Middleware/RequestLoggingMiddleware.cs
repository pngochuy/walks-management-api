namespace NZWalks.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Handling request: {Method} {Url}", context.Request.Method, context.Request.Path);

            await _next(context);

            _logger.LogInformation("Handled request: {StatusCode}", context.Response.StatusCode);
        }
    }
}
