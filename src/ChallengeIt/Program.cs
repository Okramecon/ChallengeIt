using System.Text.Json;
using ChallengeIt.Application.DependencyInjection;
using ChallengeIt.Endpoints;
using ChallengeIt.Infrastructure.DependencyInjection;
using ChallengeIt.Middlewares;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    app.UseExceptionHandler(AppBuilderActions.HandleException);
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    
    app.UseHttpsRedirection();
    app.UseApplicationEndpoints();

    app.Run();
}
