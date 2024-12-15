using System.Text;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Infrastructure.Persistence.Repositories;
using ChallengeIt.Infrastructure.Security;
using ChallengeIt.Infrastructure.Security.TokenGenerator;
using ChallengeIt.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ChallengeIt.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(configuration);
        
        services.AddScoped<IUsersRepository, UsersRepository>();
        
        services.AddSingleton<IDateTimeProvider, DateTImeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return services;
    } 
    
    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        services.AddSingleton<ITokenProvider, TokenProvider>();

        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() 
                          ?? throw new ArgumentException("JWT settings are required.");

        if (string.IsNullOrEmpty(jwtSettings.Issuer) || string.IsNullOrEmpty(jwtSettings.Audience) || string.IsNullOrEmpty(jwtSettings.Secret))
        {
            throw new ArgumentException("JWT issuer, audience, and secret are required.");
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        return services;
    }
}