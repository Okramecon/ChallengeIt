using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using ChallengeIt.Infrastructure.Persistence.Repositories;
using ChallengeIt.Infrastructure.Security.CurrentUserProvider;
using ChallengeIt.Infrastructure.Security.Identity;
using ChallengeIt.Infrastructure.Security.TokenGenerator;
using ChallengeIt.Infrastructure.Security.TokenValidation;
using ChallengeIt.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengeIt.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(configuration);
        services.AddAuthorization();

        services.AddPersistence(configuration);
        
        services.AddSingleton<IDateTimeProvider, DateTImeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("The connection string 'DefaultConnection' is missing or empty. Ensure it is properly configured in appsettings.json or environment variables.");
        }

        var dapperContextOptions = new DapperContextOptions(connectionString);
        services.AddSingleton(dapperContextOptions);

        services.AddSingleton<IDapperContext, DapperContext>(_ => 
            new DapperContext(dapperContextOptions));

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IChallengesRepository, ChallengesRepository>();
    }
    
    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<ICurrentUserProvider, CurrentUserProvider>();
        
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(nameof(JwtSettings)))
            .ValidateOnStart();

        services.AddSingleton<ITokenProvider, TokenProvider>();
        
        services
            .ConfigureOptions<JwtBearerTokenValidationConfiguration>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
    }
}