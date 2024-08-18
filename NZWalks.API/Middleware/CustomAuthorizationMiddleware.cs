namespace NZWalks.API.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        message = "Unauthorized - Access Denied!",
                        errorCode = 401,
                        timestamp = DateTime.Now,
                        info = "You are not log in."
                    }));
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        message = "Forbidden - Access Denied!",
                        errorCode = 403,
                        timestamp = DateTime.Now,
                        info = "You do not have the necessary permissions to access this resource."
                    }));
            }
        }
    }
}
