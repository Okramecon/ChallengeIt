using ChallengeIt.Application.Features.Common.Behaviors;
using ChallengeIt.Application.Settings;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            .Bind(configuration.GetSection(nameof(GoogleSettings)))
            .ValidateOnStart();

        return services;
    }
}