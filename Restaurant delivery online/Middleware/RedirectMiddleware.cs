namespace Restaurant_delivery_online.Middleware
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                context.Response.Redirect("/Home/NotFoundPage");
            }
            else if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
            {
                context.Response.Redirect("/Home/ServerErrorPage");
            }
        }
    }

    public static class RedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RedirectMiddleware>();
        }
    }
}
