using System.Net;
using System.Text.Json;

namespace MyPinPad.WebApi.Middlewares
{
    public class BadRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public BadRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Temporarily capture the response
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Check for 400 Bad Request
            if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                context.Response.ContentType = "application/json";
                responseBody.Seek(0, SeekOrigin.Begin);

                var originalResponse = await new StreamReader(responseBody).ReadToEndAsync();
                object errors;

                try
                {
                    var modelState = JsonSerializer.Deserialize<Dictionary<string, string[]>>(originalResponse);
                    errors = modelState?.SelectMany(kvp => kvp.Value.Select(msg => new
                    {
                        field = kvp.Key,
                        message = msg
                    }));
                }
                catch
                {
                    errors = new List<object> { originalResponse };
                }

                var customResponse = new
                {
                    status = 400,
                    title = "One or more validation errors occurred.",
                    timestamp = DateTime.UtcNow,
                    traceId = context.TraceIdentifier,
                    errors = errors ?? originalResponse
                };

                context.Response.Body = originalBodyStream;
                context.Response.StatusCode = 400;

                await context.Response.WriteAsync(JsonSerializer.Serialize(customResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }));
            }
            //else
            //{
            //    responseBody.Seek(0, SeekOrigin.Begin);
            //    await responseBody.CopyToAsync(originalBodyStream);
            //}
        }
    }
}
