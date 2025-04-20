using ChallengeIt.Application.DependencyInjection;
using ChallengeIt.Endpoints;
using ChallengeIt.Infrastructure.DependencyInjection;
using ChallengeIt.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddApplication(builder.Configuration)
        .AddInfrastructure(builder.Configuration);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.File(
            "logs/challege-it.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 20)
        );
}

var app = builder.Build();

try
{
    Log.Information("Starting web application");

    app.UseExceptionHandler(AppBuilderActions.HandleException);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseChallengeItHangfireDashboard();

    app.UseHttpsRedirection();
    app.UseApplicationEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}