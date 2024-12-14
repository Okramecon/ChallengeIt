using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Infrastructure.Persistence.Repositories;
using ChallengeIt.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengeIt.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return services;
    } 
}