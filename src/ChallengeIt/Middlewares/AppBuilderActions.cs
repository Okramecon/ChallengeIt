using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace ChallengeIt.Middlewares;

public static class AppBuilderActions
{
    public static void HandleException(IApplicationBuilder appBuilder)
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception != null)
            {
                var result = JsonSerializer.Serialize(new { error = exception.Message });
                await context.Response.WriteAsync(result);
            }
        });
    }
}