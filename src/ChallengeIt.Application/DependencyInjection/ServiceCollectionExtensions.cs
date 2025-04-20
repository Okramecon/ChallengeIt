using ChallengeIt.Application.Features.Common.Behaviors;
using ChallengeIt.Application.Settings;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using ChallengeIt.Application.BackgroundJobs;

namespace ChallengeIt.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Validations
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensions));

        // Settigns
        services.AddOptions<GoogleSettings>()
            .Bind(configuration.GetSection("Google"))
            .ValidateOnStart();

        services.AddBackgroundJobs(configuration);

        return services;
    }

    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(options =>
            options.UsePostgreSqlStorage(options 
                => options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

        services.AddHangfireServer();

        services.AddScoped<CheckInsJobScheduler>();

        return services;
    }

    public static void UseChallengeItHangfireDashboard(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var scheduler = scope.ServiceProvider.GetRequiredService<CheckInsJobScheduler>();
            scheduler.ShceduleJobsForAllTimeZones();
        }

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            DashboardTitle = "Challenge It background jobs"
        });
    }
}