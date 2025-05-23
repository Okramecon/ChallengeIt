﻿using System.Data;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Errors;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using ChallengeIt.Infrastructure.Persistence.Repositories;
using ChallengeIt.Infrastructure.Security.CurrentUserProvider;
using ChallengeIt.Infrastructure.Security.Identity;
using ChallengeIt.Infrastructure.Security.TokenGenerator;
using ChallengeIt.Infrastructure.Security.TokenValidation;
using ChallengeIt.Infrastructure.Utils;
using Dapper;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

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
            throw new InvalidOperationException("The connection string 'DefaultConnection' is missing or empty.");
        }


        // EF Core registration - IMPORTANT PART
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString)
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

        // Dapper-related registrations
        var dapperContextOptions = new DapperContextOptions(connectionString);
        services.AddSingleton(dapperContextOptions);

        DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));


        // Other registrations
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISqlDbContext, DapperContext>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IChallengesRepository, ChallengesRepository>();
        services.AddScoped<ICheckInsRepository, CheckInsRepository>();
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
            .AddJwtBearer()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Google:ClientId"] ?? throw new NullReferenceException(ApplicationErrors.MissingApplicationConfig + nameof(GoogleOptions.ClientId));
                options.ClientSecret = configuration["Google:ClientSecret"]?? throw new NullReferenceException(ApplicationErrors.MissingApplicationConfig + nameof(GoogleOptions.ClientSecret));
            });
    }
}