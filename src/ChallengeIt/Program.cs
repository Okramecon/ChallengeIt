using ChallengeIt.Application.DependencyInjection;
using ChallengeIt.Endpoints;
using ChallengeIt.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddApplication()
        .AddInfrastructure();
}

var app = builder.Build();
{
    app.UseExceptionHandler();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.UseHttpsRedirection();
    app.UseApplicationEndpoints();

    app.Run();
}
