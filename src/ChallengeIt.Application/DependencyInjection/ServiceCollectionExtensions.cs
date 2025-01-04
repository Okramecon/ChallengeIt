using ChallengeIt.Application.Features.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengeIt.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensions));
        return services;
    }
}