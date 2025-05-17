namespace ContentService.Middlewares
{
    public class LanguageIdMiddleware
    {
        private readonly RequestDelegate _next;

        public LanguageIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("LanguageId", out var value) &&
                int.TryParse(value, out var languageId))
            {
                context.Items["LanguageId"] = languageId;
            }

            await _next(context);
        }
    }
}
