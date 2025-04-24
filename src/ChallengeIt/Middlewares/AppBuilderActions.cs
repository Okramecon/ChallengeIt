using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

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
                Log.Error(exception, exception.Message);
                var result = JsonSerializer.Serialize(new { error = exception.Message });
                await context.Response.WriteAsync(result);
            }
        });
    }
}